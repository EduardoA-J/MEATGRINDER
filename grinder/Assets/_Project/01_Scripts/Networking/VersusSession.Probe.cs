#if DEVELOPMENT_BUILD || UNITY_EDITOR
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
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Networking
{
    public sealed partial class VersusSession
    {
        bool probeClient;
        bool probeMove;
        bool probeHost;
        bool probeRelay;
        string probeRegion;
        readonly Dictionary<PlayerRef, int> probeAcknowledgements = new();
        const int ProbeAckChannel = 0x4d475145;
        static string[] ProbeArgs => Environment.GetCommandLineArgs();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static async void RunProductProbe()
        {
            var args = ProbeArgs;
            int index = Array.IndexOf(args, "--meatgrinder-versus-probe");
            if (index < 0 || index + 2 >= args.Length) return;
            var role = args[index + 1];
            bool host = role == "host";
            VersusSession session = null;
            int exit = 0;
            try
            {
                await WaitProbe(() => SceneManager.GetActiveScene().name == "Boot" &&
                    FindAnyObjectByType<LocalGameSession>()?.Phase == LocalSessionPhase.Menu, "Boot");
                var local = FindAnyObjectByType<LocalGameSession>();
                Open(local);
                session = local.GetComponent<VersusSession>();
                session.probeClient = !host;
                session.probeHost = host;
                session.probeRelay = args.Contains("--meatgrinder-relay");
                int regionArg = Array.IndexOf(args, "--meatgrinder-region");
                if (regionArg >= 0 && regionArg + 1 < args.Length) session.probeRegion = args[regionArg + 1];
                session.room = args[index + 2];
                int rosterArg = Array.IndexOf(args, "--meatgrinder-roster");
                int roster = rosterArg >= 0 && rosterArg + 1 < args.Length && int.TryParse(args[rosterArg + 1], out var total) ? total : 2;
                session.mode = args.Contains("--meatgrinder-campaign") ? VersusMode.Campaign :
                    roster > 2 ? VersusMode.Private : VersusMode.Continuous;
                if (roster > 2) session.teamSize = roster / 2;
                Application.runInBackground = true;
                if (args.Contains("--meatgrinder-impaired"))
                {
                    var conditions = NetworkProjectConfig.Global.NetworkConditions;
                    conditions.Enabled = true;
                    conditions.DelayMin = conditions.DelayMax = .12f;
                    conditions.LossChanceMin = conditions.LossChanceMax = .04f;
                    conditions.AdditionalLoss = 0;
                    NetworkProjectConfig.Global.NetworkConditions = conditions;
                    Debug.Log("[VersusProbe] impairment requested: 120 ms, 4% loss");
                }
                await session.Connect(host);
                if (session.runner == null) throw new InvalidOperationException(session.status);
                Debug.Log("[VersusProbe] Connected " + role);
                await WaitProbe(() => session.match != null && session.runner.ActivePlayers.Count() == roster, "lobby con " + roster);
                Debug.Log("[VersusProbe] Lobby players: " + roster);
                if (roster > 2) await session.MonsterRosterProbe(host, roster);
                else if (host) await session.HostProbe();
                else await session.ClientProbe();
                Debug.Log("[VersusProbe] PASS " + role);
            }
            catch (Exception error)
            {
                exit = 1;
                Debug.LogError("[VersusProbe] FAIL " + role + ": " + error.Message);
            }
            finally
            {
                if (session != null && session.runner != null)
                {
                    try { await session.runner.Shutdown(); }
                    catch (Exception error) { Debug.LogError("[VersusProbe] Shutdown: " + error.Message); exit = 1; }
                }
                Application.Quit(exit);
            }
        }

        FusionAppSettings GetProbeSettings(FusionAppSettings settings)
        {
            if (string.IsNullOrEmpty(probeRegion)) return null;
            var copy = settings.GetCopy();
            copy.FixedRegion = probeRegion;
            return copy;
        }

        async Task MonsterRosterProbe(bool host, int roster)
        {
            var players = runner.ActivePlayers.OrderBy(player => player.PlayerId).ToArray();
            int rank = Array.IndexOf(players, runner.LocalPlayer);
            var choice = (rank / 2) % 2 == 0 ? MonsterKind.Light : MonsterKind.Heavy;
            match.RPC_ChooseMonster(choice);
            match.RPC_ChooseMonster((MonsterKind)99);
            await WaitProbe(() => ChoiceFor(runner.LocalPlayer) == choice, "selección replicada");
            if (host)
            {
                await WaitProbe(() => match.MonsterChoices.Count == roster, "selecciones del lobby");
                StartMatch();
            }
            await WaitProbe(() => match != null && match.Phase == VersusPhase.Playing && viewedPlayer != null, "inicio de partida de monstruos");
            await WaitProbe(() => GetProbeAvatars().Length == roster, "réplica del roster completo");
            var avatars = GetProbeAvatars();
            if (avatars.Count(avatar => avatar.Monster) != roster / 2 ||
                !avatars.Any(avatar => avatar.Archetype == MonsterKind.Light) || !avatars.Any(avatar => avatar.Archetype == MonsterKind.Heavy))
                throw new InvalidOperationException("Faltan equipos equilibrados o alguna de las dos clases");
            Debug.Log("[VersusProbe] Validated balanced roster and both archetypes: " + roster);
            var switched = avatars.Where(avatar => avatar.Monster).OrderBy(avatar => avatar.Object.InputAuthority.PlayerId).First();
            var newKind = switched.Archetype == MonsterKind.Light ? MonsterKind.Heavy : MonsterKind.Light;
            await AcknowledgeProbe(host, 0, roster);
            if (switched.HasInputAuthority) match.RPC_ChooseMonster(newKind);
            if (host)
            {
                var monsters = avatars.Where(avatar => avatar.Monster).ToArray();
                foreach (var monster in monsters)
                {
                    var definition = MonsterDefinition.Get(monster.Archetype);
                    var controller = monster.Body.GetComponent<CharacterController>();
                    if (monster.Vitality.Current != definition.Health.MaxHealth ||
                        !Mathf.Approximately(controller.height, definition.Movement.Height) ||
                        !Mathf.Approximately(controller.radius, definition.Movement.Radius) ||
                        monster.Body.GetComponent<PlayerMelee>().DamageAmount != definition.Melee.Damage ||
                        definition.VisualPrefab == null ||
                        pickups.First(item => item.Definition.Type == PickupType.Weapon).CollectAuthoritatively(monster.Vitality))
                        throw new InvalidOperationException("Perfil, hitbox, melee, modelo o restricción de arma incorrectos");
                }
                var target = switched;
                var attacker = viewedPlayer.Vitality;
                await WaitProbe(() => ChoiceFor(target.Object.InputAuthority) == newKind, "nueva elección en juego");
                await WaitProbe(() => !target.Vitality.IsSpawnProtected, "protección de aparición");
                if (!attacker.GetComponent<DamageResolverHost>().Resolver.Apply(new DamageInfo(attacker.StableId,
                        attacker.TeamId, target.Vitality, 1000, DamageType.Melee, target.Position, 9101, true, true, "roster-probe")))
                    throw new InvalidOperationException("No se confirmó la baja rival");
            }
            await WaitProbe(() => switched.Archetype == newKind && switched.Health == MonsterDefinition.Get(newKind).Health.MaxHealth,
                "cambio de clase al reaparecer");
            await AcknowledgeProbe(host, 1, roster);
            Debug.Log("[VersusProbe] Monster selection changed on respawn");
            if (host && match.Mode == VersusMode.Private) match.Deadline = TickTimer.CreateFromSeconds(runner, 2f);
            if (match.Mode == VersusMode.Campaign)
            {
                await CampaignRosterProbe(host, roster);
                return;
            }
            await WaitProbe(() => match != null && match.Phase == VersusPhase.Results, "resultado Private Match");
            if (match.ScoreA != 1 || match.ScoreB != 0 || match.ForfeitWinner != 0)
                throw new InvalidOperationException("Private Match debe decidirse por bajas rivales");
            await AcknowledgeProbe(host, 2, roster);
            if (!host) await Task.Delay(1500);
        }

        async Task CampaignRosterProbe(bool host, int roster)
        {
            int maps = local.Maps.Length;
            for (int round = 0; round < maps * 2; round++)
            {
                int half = round / maps;
                int map = round % maps;
                await WaitProbe(() => match != null && match.Phase == VersusPhase.Playing && match.Half == half && match.Map == map &&
                    viewedPlayer != null && arena.name == local.Maps[map].SceneName &&
                    GetProbeAvatars().Length == roster, "Campaign mapa " + map + " mitad " + half);
                if (GetProbeAvatars().Any(avatar => (bool)avatar.Monster != ((avatar.Team ^ half) == 1)))
                    throw new InvalidOperationException("Los roles de Campaign no se intercambiaron correctamente");
                await AcknowledgeProbe(host, round + 2, roster);
                if (!host) continue;
                if (half == 0 || map == 0)
                {
                    var human = GetProbeAvatars().First(avatar => !avatar.Monster);
                    ExitReached(human.Vitality);
                }
                else match.Deadline = TickTimer.CreateFromSeconds(runner, 1);
            }
            await WaitProbe(() => match != null && match.Phase == VersusPhase.Results, "resultado Campaign");
            if (match.ScoreA != maps || match.ScoreB != 1 || match.ForfeitWinner != 0)
                throw new InvalidOperationException("Campaign debe contar salidas humanas, sin sumar las bajas");
            Debug.Log("[VersusProbe] Campaign: four maps, role swap and route scoring confirmed");
            await AcknowledgeProbe(host, maps * 2 + 2, roster);
            if (!host) await Task.Delay(1500);
        }

        async Task AcknowledgeProbe(bool host, int step, int roster)
        {
            if (!host)
            {
                runner.SendReliableDataToServer(ReliableKey.FromInts(ProbeAckChannel, step, 0, 0), new byte[] { 1 });
                return;
            }
            await WaitProbe(() => probeAcknowledgements.Count(entry => entry.Value >= step) == roster - 1,
                "confirmaciones de los clientes para el paso " + step);
        }

        void ReceiveProbeAck(NetworkRunner current, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
        {
            if (!probeHost || !current.IsServer || !seats.ContainsKey(player) || data.Length != 1) return;
            key.GetInts(out int channel, out int step, out _, out _);
            if (channel == ProbeAckChannel && step is >= 0 and <= 32 &&
                (!probeAcknowledgements.TryGetValue(player, out int previous) || step > previous))
                probeAcknowledgements[player] = step;
        }

        VersusState[] GetProbeAvatars() => FindObjectsByType<VersusState>()
            .Where(avatar => avatar.Object != null && avatar.Object.IsValid && !avatar.IsMatch && !avatar.IsProjectile).ToArray();

        async Task HostProbe()
        {
            StartMatch();
            await WaitProbe(() => viewedPlayer != null && match.Phase == VersusPhase.Playing, "spawn del host");
            var remoteRef = seats.Keys.Single(player => player != runner.LocalPlayer);
            var connection = runner.GetPlayerConnectionType(remoteRef);
            if (probeRelay && connection != ConnectionType.Relayed)
                throw new InvalidOperationException("Se esperaba conexión por relay");
            Debug.Log("[VersusProbe] Transport: " + connection);
            if (!runner.TryGetPlayerObject(remoteRef, out var remoteObject))
                throw new InvalidOperationException("No hay avatar remoto");
            var remote = remoteObject.GetComponent<VersusState>();
            Vector3 initial = remote.Position;
            await WaitProbe(() => Vector3.ProjectOnPlane(remote.Position - initial, Vector3.up).sqrMagnitude > .1f, "intención de movimiento del cliente");
            Debug.Log("[VersusProbe] Remote input moved authoritative avatar");

            var pickup = pickups.First(item => item.Definition.Type == PickupType.Armor);
            int collected = 0;
            pickup.PickedUp += (_, _) => collected++;
            bool first = pickup.CollectAuthoritatively(viewedPlayer.Vitality);
            bool duplicate = pickup.CollectAuthoritatively(remote.Vitality);
            if (!first || duplicate || collected != 1 || pickup.IsAvailable)
                throw new InvalidOperationException("Dos solicitudes de un pickup deben concederlo una vez");
            Debug.Log("[VersusProbe] Pickup contention granted once");

            await WaitProbe(() => !remote.Vitality.IsSpawnProtected, "protección de aparición");
            var attacker = viewedPlayer.Vitality;
            var resolver = attacker.GetComponent<DamageResolverHost>().Resolver;
            var damage = new DamageInfo(attacker.StableId, attacker.TeamId, remote.Vitality, 1000,
                DamageType.Melee, remote.Position, 8101, true, true, "versus-probe");
            bool applied = resolver.Apply(damage);
            bool repeated = resolver.Apply(damage);
            if (!applied || repeated || remote.Vitality.IsAlive || match.ScoreA + match.ScoreB != 1)
                throw new InvalidOperationException($"Daño autoritativo: aplicado={applied}, repetido={repeated}, vida={remote.Vitality.Current}, marcador={match.ScoreA}-{match.ScoreB}");
            await WaitProbe(() => remote.Vitality.IsAlive, "reaparición remota");
            Debug.Log("[VersusProbe] Damage, single score and respawn confirmed");

            for (int map = 1; map < local.Maps.Length; map++)
            {
                match.Deadline = TickTimer.CreateFromSeconds(runner, 1.5f);
                int expected = map;
                await WaitProbe(() => match != null && match.Phase == VersusPhase.Playing && match.Map == expected, "transición de mapa " + expected);
            }
            match.Deadline = TickTimer.CreateFromSeconds(runner, 1.5f);
            await WaitProbe(() => match.Phase == VersusPhase.Results, "resultados");
            if (match.ScoreA + match.ScoreB != 1) throw new InvalidOperationException("La puntuación no se conservó");
            await Task.Delay(1800);
            StartMatch();
            await WaitProbe(() => match.Phase == VersusPhase.Playing, "revancha");
            if (match.Map != 0 || match.ScoreA != 0 || match.ScoreB != 0)
                throw new InvalidOperationException("La revancha debe reiniciar mapa y puntuación");
            await WaitProbe(() => match.Phase == VersusPhase.Results && match.ForfeitWinner != 0, "abandono remoto");
            Debug.Log("[VersusProbe] Results, rematch and forfeit confirmed");
        }

        async Task ClientProbe()
        {
            await WaitProbe(() => viewedPlayer != null && match.Phase == VersusPhase.Playing, "spawn remoto");
            probeMove = true;
            await Task.Delay(1000);
            probeMove = false;
            await WaitProbe(() => viewedPlayer != null && viewedPlayer.Health <= 0, "muerte replicada");
            await WaitProbe(() => viewedPlayer != null && viewedPlayer.Health > 0, "respawn replicado");
            for (int map = 1; map < local.Maps.Length; map++)
            {
                int expected = map;
                await WaitProbe(() => match.Phase == VersusPhase.Playing && match.Map == expected
                    && arena.IsValid() && arena.name == local.Maps[expected].SceneName && viewedPlayer != null, "mapa replicado " + expected);
            }
            await WaitProbe(() => match.Phase == VersusPhase.Results, "resultados replicados");
            if (match.ScoreA + match.ScoreB != 1) throw new InvalidOperationException("Marcador remoto incorrecto");
            await WaitProbe(() => match.Phase == VersusPhase.Playing && match.Map == 0 && viewedPlayer != null, "revancha replicada");
            await Task.Delay(800);
        }

        static async Task WaitProbe(Func<bool> ready, string step)
        {
            float deadline = Time.realtimeSinceStartup + 45;
            while (!ready())
            {
                if (Time.realtimeSinceStartup >= deadline) throw new TimeoutException(step);
                await Task.Delay(40);
            }
        }
    }
}
#endif
