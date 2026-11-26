using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Weapons;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Networking
{
    public sealed class VersusSession : MonoBehaviour, INetworkRunnerCallbacks
    {
        static readonly NetworkPrefabRef StatePrefab = NetworkPrefabRef.Parse("c137af85b708448ba1e36c1c617c10b2");
        readonly Dictionary<PlayerRef, int> seats = new();
        LocalGameSession local;
        NetworkRunner runner;
        VersusState match, viewedPlayer;
        VersusRules rules;
        Scene arena;
        GameObject template;
        LocalSpawnPoints[] spawns;
        PickupController[] pickups = Array.Empty<PickupController>();
        KeyGate[] gates = Array.Empty<KeyGate>();
        RouteKey[] routeKeys = Array.Empty<RouteKey>();
        Camera menuCamera;
        AudioListener menuListener;
        Camera view;
        Vector2 aim;
        NetworkButtons buffered;
        VersusMode mode;
        string room = "", status = "";
        int firstMap, teamSize, roundScoreA, roundScoreB, roundsPlayed;
        bool busy, friendlyFire, paused, leaving;
        bool routeCompleted, killLimitReached;
        Canvas roomCanvas;
        RectTransform roomSetup, roomSummary, mapOptions;
        UnityEngine.UI.InputField roomCode;
        UnityEngine.UI.Text roomTitle, roomDetails, roomStatus, mapName;
        UnityEngine.UI.Button primaryAction, leaveAction, fireOption;
        UnityEngine.UI.Image[] modeMarkers, mapMarkers, sizeMarkers;
        UnityEngine.UI.Image fireMarker;
        bool PanelVisible => match == null || match.Phase != VersusPhase.Playing || paused;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            LocalGameSession.OnlineRequested -= Open;
            LocalGameSession.OnlineRequested += Open;
        }

        static void Open(LocalGameSession local)
        {
            if (local.TryGetComponent<VersusSession>(out _)) return;
            var online = local.gameObject.AddComponent<VersusSession>();
            online.local = local;
            online.rules = Resources.Load<VersusRules>("VersusRules");
            if (online.rules == null) online.rules = ScriptableObject.CreateInstance<VersusRules>();
            online.teamSize = online.rules.PlayersPerTeam;
            local.SetOnline(true);
            online.BuildRoomMenu();
        }

        async Task Connect(bool host)
        {
            if (runner != null || busy) return;
            if (!PhotonAppSettings.TryGetGlobal(out var settings) || string.IsNullOrWhiteSpace(settings.AppSettings?.AppIdFusion))
            {
                status = "Configura el App ID de Fusion en PhotonAppSettings local.";
                return;
            }
            string code = room.Trim().ToUpperInvariant();
            if (code.Length is < 4 or > 24 || code.Any(c => !(c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '-')))
            {
                status = "Código de 4 a 24 caracteres: letras, números o guiones.";
                return;
            }
            busy = true;
            status = "Conectando…";
            var root = new GameObject("VersusRunner");
            DontDestroyOnLoad(root);
            runner = root.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
            runner.AddCallbacks(this);
            var sceneManager = root.AddComponent<NetworkSceneManagerDefault>();
            try
            {
                var result = await runner.StartGame(new StartGameArgs
                {
                    GameMode = host ? GameMode.Host : GameMode.Client,
                    SessionName = "MG-" + code,
                    SceneManager = sceneManager,
                    ObjectProvider = root.AddComponent<NetworkObjectProviderDefault>(),
                    PlayerCount = teamSize * 2,
                    IsVisible = mode != VersusMode.Private,
                    EnableClientSessionCreation = false,
                    IsOpen = true
                });
                if (!result.Ok) throw new InvalidOperationException(result.ShutdownReason.ToString());
                room = code;
                if (host)
                    runner.Spawn(StatePrefab, onBeforeSpawned: (_, obj) =>
                    {
                        var state = obj.GetComponent<VersusState>();
                        state.IsMatch = true;
                        state.Mode = mode;
                        state.Phase = VersusPhase.Lobby;
                    });
                status = "";
            }
            catch (Exception error)
            {
                status = "Conexión fallida: " + error.Message;
                if (runner != null) await runner.Shutdown();
                runner = null;
                if (root != null) Destroy(root);
            }
            finally { busy = false; }
        }

        async Task Leave()
        {
            if (leaving) return;
            leaving = true;
            try { if (runner != null) await runner.Shutdown(); }
            finally
            {
                if (arena.IsValid() && arena.isLoaded) await SceneManager.UnloadSceneAsync(arena);
                roomCanvas.gameObject.SetActive(false);
                Destroy(roomCanvas.gameObject);
                local.SetOnline(false);
                Destroy(this);
            }
        }

        void Update()
        {
            if (runner == null || !runner.IsRunning) return;
            if (match == null)
                match = FindObjectsByType<VersusState>()
                    .FirstOrDefault(candidate => candidate.Object != null && candidate.Object.IsValid && candidate.Runner == runner && candidate.IsMatch);
            if (match == null) return;
            var keyboard = Keyboard.current;
            if (match.Phase == VersusPhase.Playing && keyboard?.escapeKey.wasPressedThisFrame == true)
                paused = !paused;
            Cursor.lockState = PanelVisible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = PanelVisible;

            if (runner.TryGetPlayerObject(runner.LocalPlayer, out var obj) && obj != null)
            {
                var player = obj.GetComponent<VersusState>();
                if (viewedPlayer != player)
                {
                    viewedPlayer = player;
                    aim = player.Aim;
                    buffered = default;
                }
                if (!PanelVisible && player.Health > 0)
                {
                    Vector2 delta = Mouse.current?.delta.ReadValue() ?? Vector2.zero;
                    aim.x = Mathf.Repeat(aim.x + delta.x * .1f * local.Sensitivity, 360);
                    aim.y = Mathf.Clamp(aim.y - delta.y * .1f * local.Sensitivity, -89, 89);
                    Latch(VersusButton.Jump, keyboard?.spaceKey.wasPressedThisFrame == true);
                    Latch(VersusButton.Attack, Mouse.current?.leftButton.wasPressedThisFrame == true);
                    Latch(VersusButton.Reload, keyboard?.rKey.wasPressedThisFrame == true);
                    Latch(VersusButton.Melee, keyboard?.vKey.wasPressedThisFrame == true);
                    Latch(VersusButton.Shotgun, keyboard?.digit1Key.wasPressedThisFrame == true);
                    Latch(VersusButton.Rocket, keyboard?.digit2Key.wasPressedThisFrame == true);
                }
            }
            else viewedPlayer = null;

            if (arena.IsValid() && arena.isLoaded && match.Phase == VersusPhase.Playing)
            {
                SyncEnvironment();
                if (runner.IsServer && !busy && !HasBothTeams())
                    EndMatch(seats.Values.Any(seat => seat % 2 == 0) ? 1 : 2);
                else if (runner.IsServer && !busy && (routeCompleted || killLimitReached || match.Deadline.Expired(runner)))
                    _ = Advance(routeCompleted);
            }
        }

        void LateUpdate()
        {
            RefreshRoomMenu();
            if (view == null || viewedPlayer == null) return;
            var player = viewedPlayer;
            var body = player.Body;
            Vector3 position = body != null ? body.transform.position : player.Position;
            view.transform.SetPositionAndRotation(position + Vector3.up * Mathf.Max(.4f, player.EyeHeight),
                Quaternion.Euler(aim.y, aim.x, 0));
        }

        void Latch(VersusButton button, bool pressed)
        {
            if (pressed) buffered.Set(button, true);
        }

        public void OnInput(NetworkRunner current, NetworkInput input)
        {
            var command = new VersusInput { Aim = aim };
            if (!PanelVisible && viewedPlayer != null && viewedPlayer.Health > 0 && Keyboard.current is Keyboard keys)
            {
                command.Move = new Vector2((keys.dKey.isPressed ? 1 : 0) - (keys.aKey.isPressed ? 1 : 0),
                    (keys.wKey.isPressed ? 1 : 0) - (keys.sKey.isPressed ? 1 : 0));
                command.Buttons = buffered;
                if (keys.spaceKey.isPressed) command.Buttons.Set(VersusButton.Jump, true);
                command.Buttons.Set(VersusButton.Sprint, keys.leftShiftKey.isPressed);
                command.Buttons.Set(VersusButton.Crouch, keys.leftCtrlKey.isPressed);
            }
            buffered = default;
            input.Set(command);
        }

        async Task LoadRound(int index)
        {
            if (runner == null || !runner.IsServer || busy) return;
            busy = true;
            match.Phase = VersusPhase.Loading;
            match.Map = index;
            match.Deadline = TickTimer.None;
            roundScoreA = roundScoreB = 0;
            routeCompleted = killLimitReached = false;
            float deadline = Time.realtimeSinceStartup + rules.SceneTimeoutSeconds;
            try
            {
                foreach (var player in seats.Keys.ToArray()) Despawn(player);
                if (arena.IsValid() && arena.isLoaded)
                    await WaitForScene(runner.UnloadScene(SceneRef.FromIndex(arena.buildIndex)), deadline);
                var map = local.Maps[index];
                var sceneRef = runner.GetSceneRef(map.SceneName);
                if (!sceneRef.IsValid) throw new InvalidOperationException("Mapa fuera de Build Settings.");
                await WaitForScene(runner.LoadScene(sceneRef, LoadSceneMode.Additive), deadline);
                PrepareArena(SceneManager.GetSceneByName(map.SceneName));
                foreach (var entry in seats) Spawn(entry.Key, entry.Value);
                SyncEnvironment();
                match.Deadline = TickTimer.CreateFromSeconds(runner, rules.RoundSeconds);
                match.Phase = VersusPhase.Playing;
            }
            catch (Exception error)
            {
                status = "Error de mapa: " + error.Message;
                if (runner != null) await runner.Shutdown();
            }
            finally { busy = false; }
        }

        async Task WaitForScene(NetworkSceneAsyncOp operation, float deadline)
        {
            while (!operation.IsDone)
            {
                if (runner == null || Time.realtimeSinceStartup >= deadline)
                    throw new TimeoutException("Tiempo de carga agotado.");
                await Task.Yield();
            }
            if (operation.Error != null) throw operation.Error;
        }

        void PrepareArena(Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded || (scene == arena && template != null)) return;
            arena = scene;
            SceneManager.SetActiveScene(scene);
            var roots = scene.GetRootGameObjects();
            var bodies = roots.SelectMany(root => root.GetComponentsInChildren<PlayerLife>(true)).ToArray();
            template = bodies.FirstOrDefault()?.gameObject;
            foreach (var body in bodies) body.gameObject.SetActive(false);
            foreach (var target in roots.SelectMany(root => root.GetComponentsInChildren<HealthComponent>(true)))
                if (target.GetComponent<PlayerLife>() == null) target.gameObject.SetActive(false);
            pickups = roots.SelectMany(root => root.GetComponentsInChildren<PickupController>(true)).ToArray();
            gates = roots.SelectMany(root => root.GetComponentsInChildren<KeyGate>(true)).ToArray();
            routeKeys = roots.SelectMany(root => root.GetComponentsInChildren<RouteKey>(true)).ToArray();
            foreach (var item in pickups) item.Replica = !runner.IsServer;
            foreach (var key in routeKeys) key.Replica = !runner.IsServer;
            foreach (var gate in gates)
            {
                gate.Replicated = !runner.IsServer;
                if (runner.IsServer) gate.OnlineExitReached += ExitReached;
            }
            if (!runner.IsServer) return;
            spawns = new LocalSpawnPoints[2];
            for (int role = 0; role < 2; role++)
            {
                string prefix = role == 0 ? "Spawn_Human_" : "Spawn_Monster_";
                var markers = roots.SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                    .Where(marker => marker.name.StartsWith(prefix, StringComparison.Ordinal)).OrderBy(marker => marker.name).ToArray();
                if (markers.Length == 0) throw new InvalidOperationException("Faltan spawns " + prefix);
                var group = new GameObject(prefix + "Network");
                SceneManager.MoveGameObjectToScene(group, scene);
                spawns[role] = group.AddComponent<LocalSpawnPoints>();
                spawns[role].SetPoints(markers);
            }
        }

        void SyncEnvironment()
        {
            for (int i = 0; i < routeKeys.Length && i < match.Keys.Length; i++)
            {
                if (runner.IsServer) match.Keys.Set(i, routeKeys[i].gameObject.activeSelf);
                else if (routeKeys[i].gameObject.activeSelf != (bool)match.Keys[i])
                    routeKeys[i].gameObject.SetActive(match.Keys[i]);
            }
            for (int i = 0; i < pickups.Length && i < match.Pickups.Length; i++)
            {
                if (runner.IsServer) match.Pickups.Set(i, pickups[i].IsAvailable);
                else pickups[i].ShowReplica(match.Pickups[i]);
            }
            for (int i = 0; i < gates.Length && i < match.Gates.Length; i++)
            {
                if (runner.IsServer) match.Gates.Set(i, gates[i].Unlocked);
                else gates[i].RemoteUnlocked = match.Gates[i];
            }
        }

        void Spawn(PlayerRef player, int seat)
        {
            if (template == null) throw new InvalidOperationException("El mapa no tiene plantilla de jugador.");
            int team = seat % 2;
            int role = team ^ match.Half;
            var points = spawns[role].Points;
            var point = points[(seat / 2) % points.Length];
            var offset = seat / 2 >= points.Length ? point.right * 1.25f : Vector3.zero;
            var body = Instantiate(template, point.position + offset, point.rotation);
            SceneManager.MoveGameObjectToScene(body, arena);
            body.name = "VersusPlayer-" + player.PlayerId;
            body.GetComponent<PlayerInputReader>().ExternalInput = true;
            body.GetComponent<PlayerLook>().MenuControlsCursor = true;
            body.GetComponent<HealthComponent>().SetTeam(team + 1);
            body.GetComponent<HealthComponent>().AllowWeapons = role == 0;
            var resolver = body.GetComponent<DamageResolverHost>().Resolver;
            resolver.FriendlyFire = friendlyFire;
            resolver.Killed += AwardKill;
            foreach (var camera in body.GetComponentsInChildren<Camera>(true)) camera.enabled = false;
            foreach (var listener in body.GetComponentsInChildren<AudioListener>(true)) listener.enabled = false;
            foreach (var renderer in body.GetComponentsInChildren<Renderer>(true)) renderer.enabled = false;
            body.GetComponent<CombatHud>().enabled = false;
            body.GetComponent<PlayerLife>().SetSpawnContext(null, spawns[role]);
            body.SetActive(true);
            body.GetComponent<WeaponController>().ClearInventory();
            body.GetComponent<LocalProjectileSpawner>().Spawned += ReplicateProjectile;
            var avatar = runner.Spawn(StatePrefab, point.position, point.rotation, player, (_, obj) =>
            {
                var state = obj.GetComponent<VersusState>();
                state.Team = team;
                state.Monster = role == 1;
            });
            avatar.GetComponent<VersusState>().BindBody(body);
            runner.SetPlayerObject(player, avatar);
        }

        void ReplicateProjectile(RocketProjectile rocket)
        {
            if (match == null || match.Phase != VersusPhase.Playing) return;
            var obj = runner.Spawn(StatePrefab, rocket.transform.position, rocket.transform.rotation, onBeforeSpawned: (_, spawned) =>
            {
                var state = spawned.GetComponent<VersusState>();
                state.IsProjectile = true;
                state.Position = rocket.transform.position;
            });
            if (obj != null) obj.GetComponent<VersusState>().BindProjectile(rocket);
        }

        void Despawn(PlayerRef player)
        {
            if (runner.TryGetPlayerObject(player, out var obj) && obj != null)
            {
                runner.SetPlayerObject(player, null);
                runner.Despawn(obj);
            }
        }

        void AwardKill(DamageInfo hit)
        {
            if (match.Phase != VersusPhase.Playing || hit.AttackerId == hit.Target.StableId) return;
            foreach (var player in seats.Keys)
            {
                if (!runner.TryGetPlayerObject(player, out var obj) || obj == null) continue;
                var attacker = obj.GetComponent<VersusState>();
                if (attacker.Vitality.StableId != hit.AttackerId || attacker.Vitality.TeamId == hit.Target.TeamId) continue;
                attacker.Kills++;
                if (match.Mode == VersusMode.Campaign) return;
                if (attacker.Team == 0) { match.ScoreA++; roundScoreA++; }
                else { match.ScoreB++; roundScoreB++; }
                killLimitReached = Mathf.Max(roundScoreA, roundScoreB) >= rules.ContinuousKillLimit;
                return;
            }
        }

        void ExitReached(HealthComponent body)
        {
            if (!busy && match.Phase == VersusPhase.Playing && body.AllowWeapons)
                routeCompleted = true;
        }

        async Task Advance(bool completedRoute)
        {
            if (match.Mode == VersusMode.Campaign && completedRoute)
            {
                if (match.Half == 0) match.ScoreA++;
                else match.ScoreB++;
            }
            if (match.Mode == VersusMode.Private)
            {
                EndMatch();
                return;
            }
            if (match.Mode == VersusMode.Continuous)
            {
                if (++roundsPlayed < local.Maps.Length)
                    await LoadRound((match.Map + 1) % local.Maps.Length);
                else EndMatch();
                return;
            }
            if (match.Map + 1 < local.Maps.Length)
                await LoadRound(match.Map + 1);
            else if (match.Mode == VersusMode.Campaign && match.Half == 0)
            {
                match.Half = 1;
                await LoadRound(0);
            }
            else EndMatch();
        }

        void EndMatch(int forfeitWinner = 0)
        {
            match.ForfeitWinner = forfeitWinner;
            match.Phase = VersusPhase.Results;
            match.Deadline = TickTimer.None;
            foreach (var player in seats.Keys.ToArray()) Despawn(player);
        }

        void StartMatch()
        {
            if (seats.Count < 2 || busy) return;
            runner.SessionInfo.IsOpen = false;
            match.Half = match.ScoreA = match.ScoreB = 0;
            match.ForfeitWinner = roundsPlayed = 0;
            paused = false;
            _ = LoadRound(match.Mode == VersusMode.Campaign ? 0 : firstMap);
        }

        bool HasBothTeams() => seats.Values.Any(seat => seat % 2 == 0) && seats.Values.Any(seat => seat % 2 == 1);

        void OnGUI()
        {
            if (local == null || PanelVisible || viewedPlayer == null) return;
            var player = viewedPlayer;
            GUI.Label(new Rect(20, 16, 650, 32),
                $"Equipo {(player.Team == 0 ? "A" : "B")} · {(player.Monster ? "Monstruo" : "Humano")}   |   {match.ScoreA} – {match.ScoreB}   |   {Mathf.CeilToInt(match.Deadline.RemainingTime(runner) ?? 0)} s");
            GUI.Label(new Rect(20, Screen.height - 62, 750, 42),
                $"Salud {player.Health:0}   Armadura {player.Armor:0}   Munición {player.Loaded}/{player.Reserve}   Bajas {player.Kills}   Muertes {player.Deaths}");
            GUI.Label(new Rect(Screen.width / 2f - 6, Screen.height / 2f - 10, 24, 24), "+");
            if (player.Health <= 0) GUI.Label(new Rect(Screen.width / 2f - 75, Screen.height / 2f + 20, 250, 30), "Esperando reaparición");
        }

        void BuildRoomMenu()
        {
            roomCanvas = GameUi.Canvas(transform, "VersusMenu", 110);
            roomCanvas.GetComponent<UnityEngine.UI.CanvasScaler>().screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            roomCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            var root = roomCanvas.transform;
            GameUi.Image(GameUi.Fill(root, "Base"), GameUi.Ink);
            var menu = GetComponent<SessionMenu>();
            ScreenBackground.Create(root, menu != null ? menu.MenuBackground : null);
            GameUi.Image(GameUi.Rect(root, "Rule", Vector2.up, Vector2.up, new Vector2(64, -70), new Vector2(1470, 7)), GameUi.Accent);
            GameUi.Text(root, "Brand", "MEATGRINDER", Vector2.up, Vector2.up, new Vector2(64, -83),
                new Vector2(1460, 150), 100, GameUi.Paper, role: UiFontRole.Display);
            roomTitle = RoomText(root, "Heading", "MULTIJUGADOR", 68, -218, 1250, 44, 23, UiFontRole.System);

            var panel = GameUi.Rect(root, "RoomPanel", Vector2.up, Vector2.up, new Vector2(48, -295), new Vector2(1488, 520));
            GameUi.Panel(panel).color = GameUi.Ink;
            roomSetup = GameUi.Fill(panel, "CreateOrJoin");
            roomSummary = GameUi.Fill(panel, "RoomSummary");
            RoomText(roomSetup, "RoomCodeLabel", "CÓDIGO DE SALA", 32, -20, 550, 30, 19, UiFontRole.System);
            var entry = GameUi.Rect(roomSetup, "RoomCode", Vector2.up, Vector2.up, new Vector2(32, -54), new Vector2(610, 56));
            var plate = GameUi.Panel(entry, false);
            plate.raycastTarget = true;
            roomCode = entry.gameObject.AddComponent<UnityEngine.UI.InputField>();
            roomCode.targetGraphic = plate;
            roomCode.characterLimit = 24;
            roomCode.textComponent = RoomText(entry, "Value", "", 18, 0, 570, 56, 24, UiFontRole.System);
            roomCode.textComponent.supportRichText = false;
            var placeholder = RoomText(entry, "Placeholder", "4–24 LETRAS, NÚMEROS O GUIONES", 18, 0, 570, 56, 19, UiFontRole.System);
            placeholder.color = GameUi.Muted;
            roomCode.placeholder = placeholder;
            roomCode.onValueChanged.AddListener(value => room = value);

            RoomText(roomSetup, "ModeLabel", "MODALIDAD", 32, -132, 610, 32, 19, UiFontRole.System);
            var names = new[] { "Versus Campaign", "Versus Continuous", "Private Match" };
            modeMarkers = new UnityEngine.UI.Image[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                var option = (VersusMode)i;
                modeMarkers[i] = ChoiceMarker(RoomButton(roomSetup, option.ToString(), names[i], 32, -170 - i * 64, 610,
                    () => { mode = option; if (mode != VersusMode.Private) friendlyFire = false; }));
            }
            RoomText(roomSetup, "TeamSizeLabel", "TAMAÑO DE EQUIPOS", 700, -20, 650, 30, 19, UiFontRole.System);
            sizeMarkers = new[]
            {
                ChoiceMarker(RoomButton(roomSetup, "TwoPerTeam", "2 CONTRA 2", 700, -54, 270, () => teamSize = 2)),
                ChoiceMarker(RoomButton(roomSetup, "FourPerTeam", "4 CONTRA 4", 982, -54, 270, () => teamSize = 4))
            };
            mapOptions = GameUi.Fill(roomSetup, "MapOptions");
            RoomText(mapOptions, "MapLabel", "MAPA INICIAL", 700, -132, 650, 32, 19, UiFontRole.System);
            mapMarkers = new UnityEngine.UI.Image[local.Maps.Length];
            for (int i = 0; i < mapMarkers.Length; i++)
            {
                int index = i;
                mapMarkers[i] = ChoiceMarker(RoomButton(mapOptions, local.Maps[i].MapId, local.Maps[i].MapId,
                    700 + i * 135, -170, 124, () => firstMap = index));
            }
            mapName = RoomText(mapOptions, "MapName", "", 700, -232, 650, 34, 22, UiFontRole.Industrial);
            fireOption = RoomButton(roomSetup, "FriendlyFire", "FUEGO AMIGO", 700, -280, 380, () => friendlyFire = !friendlyFire);
            fireMarker = ChoiceMarker(fireOption);
            RoomButton(roomSetup, "CreateRoom", "CREAR SALA", 32, -380, 310, () => _ = Connect(true));
            RoomButton(roomSetup, "JoinRoom", "UNIRSE POR CÓDIGO", 356, -380, 310, () => _ = Connect(false));
            roomDetails = RoomText(roomSummary, "RoomDetails", "", 32, -24, 1390, 210, 31, UiFontRole.Industrial);
            primaryAction = RoomButton(roomSummary, "RoomAction", "", 32, -380, 310,
                () => { if (paused && match.Phase == VersusPhase.Playing) paused = false; else StartMatch(); });
            roomStatus = RoomText(panel, "RoomStatus", "", 32, -450, 1390, 48, 22, UiFontRole.System);
            leaveAction = RoomButton(panel, "LeaveRoom", "VOLVER", 1115, -380, 325, () => _ = Leave());
            RefreshRoomMenu();
        }

        UnityEngine.UI.Button RoomButton(Transform parent, string name, string label, int x, int y, int width,
            UnityEngine.Events.UnityAction action) =>
            GameUi.Button(parent, name, label, new Vector2(x, y), new Vector2(width, 56), action, () => local.ReduceMotion);

        static UnityEngine.UI.Text RoomText(Transform parent, string name, string label, int x, int y, int width, int height,
            int size, UiFontRole font) =>
            GameUi.Text(parent, name, label, Vector2.up, Vector2.up, new Vector2(x, y), new Vector2(width, height), size,
                GameUi.Paper, role: font);

        static UnityEngine.UI.Image ChoiceMarker(UnityEngine.UI.Button button)
        {
            var strip = GameUi.Rect(button.transform, "CurrentChoice", Vector2.zero, Vector2.zero, new Vector2(16, 4),
                new Vector2(((RectTransform)button.transform).sizeDelta.x - 32, 3));
            return GameUi.Image(strip, GameUi.Accent);
        }

        void RefreshRoomMenu()
        {
            if (roomCanvas == null) return;
            bool visible = PanelVisible && !leaving;
            roomCanvas.gameObject.SetActive(visible);
            if (!visible) return;
            bool setup = match == null && runner == null && !busy;
            roomSetup.gameObject.SetActive(setup);
            roomSummary.gameObject.SetActive(!setup);
            roomTitle.text = setup ? "MULTIJUGADOR / CREAR O UNIRSE" : "MULTIJUGADOR";
            leaveAction.interactable = !busy;
            SetCaption(leaveAction, runner == null ? "VOLVER" : "SALIR DE SALA");
            roomStatus.text = status;
            if (setup)
            {
                for (int i = 0; i < modeMarkers.Length; i++) modeMarkers[i].enabled = i == (int)mode;
                for (int i = 0; i < mapMarkers.Length; i++) mapMarkers[i].enabled = i == firstMap;
                sizeMarkers[0].enabled = teamSize == 2;
                sizeMarkers[1].enabled = teamSize == 4;
                mapOptions.gameObject.SetActive(mode != VersusMode.Campaign);
                mapName.text = local.Maps[firstMap].DisplayName;
                fireOption.gameObject.SetActive(mode == VersusMode.Private);
                fireMarker.enabled = friendlyFire;
                SetCaption(fireOption, friendlyFire ? "FUEGO AMIGO  /  SÍ" : "FUEGO AMIGO  /  NO");
            }
            else
            {
                string action = "";
                roomDetails.text = "";
                if (match != null)
                {
                    string map = local.Maps[match.Map].DisplayName;
                    string detail = match.Phase switch
                    {
                        VersusPhase.Lobby => $"SALA DE ESPERA   /   {runner.ActivePlayers.Count()} JUGADORES",
                        VersusPhase.Loading => $"CARGANDO  {map}",
                        VersusPhase.Results => $"{WinnerLabel()}   /   {match.ScoreA} – {match.ScoreB}",
                        _ => $"PAUSA   /   {map}"
                    };
                    roomDetails.text = $"{match.Mode}   /   {room}\n\n{detail}";
                    if (paused && match.Phase == VersusPhase.Playing) action = "CONTINUAR";
                    else if (runner.IsServer && seats.Count >= 2 && !busy)
                        action = match.Phase == VersusPhase.Lobby ? "INICIAR PARTIDA" :
                            match.Phase == VersusPhase.Results ? "REVANCHA" : "";
                }
                primaryAction.gameObject.SetActive(action.Length > 0);
                SetCaption(primaryAction, action);
            }
            var events = EventSystem.current;
            if (events != null && (events.currentSelectedGameObject == null || !events.currentSelectedGameObject.activeInHierarchy))
                events.SetSelectedGameObject(setup ? roomCode.gameObject :
                    primaryAction.gameObject.activeInHierarchy ? primaryAction.gameObject : leaveAction.gameObject);
        }

        static void SetCaption(UnityEngine.UI.Button button, string value) =>
            button.GetComponentInChildren<UnityEngine.UI.Text>(true).text = value;

        string WinnerLabel() => match.ForfeitWinner == 1 ? "GANA A POR ABANDONO" :
            match.ForfeitWinner == 2 ? "GANA B POR ABANDONO" :
            match.ScoreA == match.ScoreB ? "EMPATE" : match.ScoreA > match.ScoreB ? "GANA A" : "GANA B";

        public void OnPlayerJoined(NetworkRunner current, PlayerRef player)
        {
            if (!current.IsServer || seats.ContainsKey(player)) return;
            int seat = Enumerable.Range(0, teamSize * 2).First(value => !seats.ContainsValue(value));
            seats.Add(player, seat);
        }

        public void OnPlayerLeft(NetworkRunner current, PlayerRef player)
        {
            if (!current.IsServer) return;
            Despawn(player);
            seats.Remove(player);
            if (!busy && match != null && match.Phase == VersusPhase.Playing && !HasBothTeams())
                EndMatch(seats.Values.Any(seat => seat % 2 == 0) ? 1 : 2);
        }

        public void OnSceneLoadDone(NetworkRunner current)
        {
            if (current.IsServer) return;
            foreach (var map in local.Maps)
            {
                var scene = SceneManager.GetSceneByName(map.SceneName);
                if (scene.IsValid() && scene.isLoaded) PrepareArena(scene);
            }
        }

        public void OnSceneLoadStart(NetworkRunner current)
        {
            if (view != null) return;
            var boot = SceneManager.GetSceneByName("Boot");
            if (boot.IsValid() && boot.isLoaded)
            {
                menuCamera = boot.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<Camera>()).FirstOrDefault();
                if (menuCamera != null)
                {
                    menuCamera.enabled = false;
                    menuListener = menuCamera.GetComponent<AudioListener>();
                    if (menuListener != null) menuListener.enabled = false;
                }
            }
            var camera = new GameObject("VersusView", typeof(Camera), typeof(AudioListener));
            camera.transform.SetParent(transform, false);
            view = camera.GetComponent<Camera>();
            view.fieldOfView = local.FieldOfView;
            view.nearClipPlane = .05f;
        }

        public void OnShutdown(NetworkRunner current, ShutdownReason reason)
        {
            if (view != null) Destroy(view.gameObject);
            if (menuCamera != null) menuCamera.enabled = true;
            if (menuListener != null) menuListener.enabled = true;
            runner = null;
            match = viewedPlayer = null;
            seats.Clear();
            paused = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (!leaving && string.IsNullOrEmpty(status)) status = "Sala cerrada: " + reason;
            if (!leaving && arena.IsValid() && arena.isLoaded) SceneManager.UnloadSceneAsync(arena);
        }

        public void OnConnectRequest(NetworkRunner current, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            if (match != null && match.Phase != VersusPhase.Lobby) request.Refuse();
            else request.Accept();
        }
        public void OnConnectedToServer(NetworkRunner current) { }
        public void OnDisconnectedFromServer(NetworkRunner current, NetDisconnectReason reason) { }
        public void OnConnectFailed(NetworkRunner current, NetAddress address, NetConnectFailedReason reason) { }
        public void OnInputMissing(NetworkRunner current, PlayerRef player, NetworkInput input) { }
        public void OnSessionListUpdated(NetworkRunner current, List<SessionInfo> sessions) { }
        public void OnCustomAuthenticationResponse(NetworkRunner current, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner current, HostMigrationToken token) { }
        public void OnObjectEnterAOI(NetworkRunner current, NetworkObject obj, PlayerRef player) { }
        public void OnObjectExitAOI(NetworkRunner current, NetworkObject obj, PlayerRef player) { }
        public void OnReliableDataReceived(NetworkRunner current, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner current, PlayerRef player, ReliableKey key, float progress) { }
        public void OnUserSimulationMessage(NetworkRunner current, SimulationMessagePtr message) { }
    }
}
