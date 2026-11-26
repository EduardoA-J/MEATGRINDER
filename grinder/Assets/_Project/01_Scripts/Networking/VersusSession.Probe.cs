#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
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
                session.room = args[index + 2];
                session.mode = VersusMode.Continuous;
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
                await WaitProbe(() => session.match != null && session.runner.ActivePlayers.Count() == 2, "lobby de dos jugadores");
                Debug.Log("[VersusProbe] Two players in lobby");
                if (host) await session.HostProbe();
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
