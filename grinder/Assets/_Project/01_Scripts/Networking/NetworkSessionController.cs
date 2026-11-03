using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Networking
{
    public sealed class NetworkSessionController : MonoBehaviour, INetworkRunnerCallbacks
    {
        public const string DefaultSessionName = "MeatgrinderProbe";

        private const string RoleArgument = "-meatgrinderRole";
        private const string SessionArgument = "-meatgrinderSession";
        private const string AutoConnectArgument = "-meatgrinderAutoConnect";
        private const string ProbeArgument = "-meatgrinderProbe";

        private readonly List<string> logLines = new List<string>();
        private NetworkRunner runner;
        private NetworkPlayerSpawner spawner;
        private bool isStarting;
        private bool autoConnect;
        private bool probe;
        private bool sawTwoAvatars;
        private bool sawRemoteLeave;
        private float probeElapsed;
        private string requestedRole = string.Empty;

        public string Status { get; private set; } = "Desconectado";
        public string RoleLabel { get; private set; } = "Ninguno";
        public string SessionName { get; private set; } = DefaultSessionName;
        public int PlayerCount { get; private set; }
        public IReadOnlyList<string> LogLines => logLines;

        private void Awake()
        {
            spawner = GetComponent<NetworkPlayerSpawner>();
            NetworkTestAvatar.ResetCount();
            ReadCommandLine();
            Report("Listo. Sesión por defecto: " + SessionName);
        }

        private void Start()
        {
            if (autoConnect == false)
            {
                return;
            }

            if (requestedRole == "client")
            {
                StartAsClient();
            }
            else
            {
                StartAsHost();
            }
        }

        private void Update()
        {
            if (probe == false || runner == null || runner.IsRunning == false)
            {
                return;
            }

            probeElapsed += Time.unscaledDeltaTime;
            if (NetworkTestAvatar.VisibleCount >= 2)
            {
                sawTwoAvatars = true;
            }

            if (requestedRole == "client" && sawTwoAvatars)
            {
                WriteProbe("CLIENT_SAW_TWO");
                LeaveSession();
                return;
            }

            if (requestedRole != "client" && sawTwoAvatars && sawRemoteLeave && NetworkTestAvatar.VisibleCount <= 1)
            {
                WriteProbe("HOST_SAW_DESPAWN");
                QuitProbe(0);
            }

            if (probeElapsed > 120f)
            {
                WriteProbe("TIMEOUT visible=" + NetworkTestAvatar.VisibleCount + " players=" + PlayerCount);
                QuitProbe(2);
            }
        }

        public void SetSessionName(string sessionName)
        {
            SessionName = string.IsNullOrWhiteSpace(sessionName) ? DefaultSessionName : sessionName.Trim();
        }

        public void StartAsHost()
        {
            requestedRole = "host";
            _ = StartSession(GameMode.Host);
        }

        public void StartAsClient()
        {
            requestedRole = "client";
            _ = StartSession(GameMode.Client);
        }

        public void LeaveSession()
        {
            if (runner == null || runner.IsRunning == false)
            {
                Status = "Desconectado";
                RoleLabel = "Ninguno";
                return;
            }

            Report("Cerrando sesión.");
            runner.Shutdown();
        }

        private async Task StartSession(GameMode mode)
        {
            if (isStarting)
            {
                return;
            }

            if (runner != null && runner.IsRunning)
            {
                Report("Ya hay una sesión activa.");
                return;
            }

            if (spawner == null || spawner.HasPrefab == false)
            {
                Report("Falta el prefab de jugador.");
                return;
            }

            if (HasAppId() == false)
            {
                Report("Falta el App ID local de Fusion.");
                return;
            }

            int buildIndex = gameObject.scene.buildIndex;
            if (buildIndex < 0)
            {
                Report("La escena no está en Build Settings.");
                return;
            }

            isStarting = true;
            Status = "Conectando como " + mode;
            RoleLabel = mode.ToString();
            Report(Status + ". Sesión " + SessionName);

            try
            {
                GameObject runnerObject = new GameObject("NetworkRunner");
                DontDestroyOnLoad(runnerObject);
                runner = runnerObject.AddComponent<NetworkRunner>();
                runner.ProvideInput = false;
                runnerObject.AddComponent<NetworkSceneManagerDefault>();
                runnerObject.AddComponent<NetworkObjectProviderDefault>();
                runner.AddCallbacks(this);

                NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
                sceneInfo.AddSceneRef(SceneRef.FromIndex(buildIndex), LoadSceneMode.Additive);

                StartGameResult result = await runner.StartGame(new StartGameArgs
                {
                    GameMode = mode,
                    SessionName = SessionName,
                    Scene = sceneInfo,
                    SceneManager = runner.GetComponent<NetworkSceneManagerDefault>(),
                    ObjectProvider = runner.GetComponent<NetworkObjectProviderDefault>(),
                    PlayerCount = 4,
                    IsOpen = true,
                    IsVisible = true
                });

                if (result.Ok == false)
                {
                    Status = "Error de conexión";
                    Report("StartGame falló: " + result.ShutdownReason + " " + result.ErrorMessage);
                    WriteProbe("START_FAILED " + result.ShutdownReason);
                    return;
                }

                Status = "Conectado";
                RoleLabel = runner.GameMode.ToString();
                Report("StartGame correcto. Rol " + RoleLabel + ". Sesión " + runner.SessionInfo.Name);
                WriteProbe("START_OK " + RoleLabel);
            }
            catch (Exception exception)
            {
                Status = "Error de conexión";
                Report("Excepción al iniciar: " + exception.Message);
                WriteProbe("START_EXCEPTION " + exception.GetType().Name);
                Debug.LogException(exception);
            }
            finally
            {
                isStarting = false;
            }
        }

        private static bool HasAppId()
        {
            return PhotonAppSettings.TryGetGlobal(out PhotonAppSettings settings)
                && settings.AppSettings != null
                && string.IsNullOrEmpty(settings.AppSettings.AppIdFusion) == false;
        }

        private void ReadCommandLine()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i] == RoleArgument && i + 1 < arguments.Length)
                {
                    requestedRole = arguments[i + 1];
                }
                else if (arguments[i] == SessionArgument && i + 1 < arguments.Length)
                {
                    SetSessionName(arguments[i + 1]);
                }
                else if (arguments[i] == AutoConnectArgument)
                {
                    autoConnect = true;
                }
                else if (arguments[i] == ProbeArgument)
                {
                    probe = true;
                }
            }
        }

        private void Report(string message)
        {
            Debug.Log("[NetworkTest] " + message);
            logLines.Add(message);
            if (logLines.Count > 12)
            {
                logLines.RemoveAt(0);
            }

            WriteProbe(message);
        }

        private void WriteProbe(string message)
        {
            string directory = Environment.GetEnvironmentVariable("MEATGRINDER_LOG_DIR");
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Logs"));
            }

            try
            {
                Directory.CreateDirectory(directory);
                string role = string.IsNullOrEmpty(requestedRole) ? "manual" : requestedRole;
                string path = Path.Combine(directory, "network-test-" + role + ".txt");
                File.AppendAllText(path, DateTime.UtcNow.ToString("o") + " " + message + Environment.NewLine);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[NetworkTest] No se pudo escribir el diagnóstico: " + exception.Message);
            }
        }

        private void QuitProbe(int exitCode)
        {
            if (probe == false)
            {
                return;
            }

            probe = false;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.Exit(exitCode);
#else
            Application.Quit(exitCode);
#endif
        }

        public void OnPlayerJoined(NetworkRunner callbackRunner, PlayerRef player)
        {
            PlayerCount = CountPlayers(callbackRunner);
            Report("Jugador unido. PlayerId=" + player.PlayerId + " total=" + PlayerCount);
            if (spawner != null)
            {
                spawner.SpawnFor(callbackRunner, player);
            }
        }

        public void OnPlayerLeft(NetworkRunner callbackRunner, PlayerRef player)
        {
            if (spawner != null)
            {
                spawner.DespawnFor(callbackRunner, player);
            }

            PlayerCount = CountPlayers(callbackRunner);
            sawRemoteLeave = true;
            Report("Jugador salió. PlayerId=" + player.PlayerId + " total=" + PlayerCount);
        }

        public void OnConnectedToServer(NetworkRunner callbackRunner)
        {
            Report("Conectado al servidor. LocalPlayer=" + callbackRunner.LocalPlayer.PlayerId);
        }

        public void OnDisconnectedFromServer(NetworkRunner callbackRunner, NetDisconnectReason reason)
        {
            Status = "Desconectado";
            Report("Desconectado del servidor: " + reason);
        }

        public void OnConnectFailed(NetworkRunner callbackRunner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Status = "Error de conexión";
            Report("Conexión fallida: " + reason);
        }

        public void OnConnectRequest(NetworkRunner callbackRunner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            request.Accept();
            Report("Solicitud de conexión aceptada.");
        }

        public void OnShutdown(NetworkRunner callbackRunner, ShutdownReason shutdownReason)
        {
            Status = "Desconectado";
            RoleLabel = "Ninguno";
            runner = null;
            Report("Runner cerrado: " + shutdownReason);
            if (probe && requestedRole == "client" && sawTwoAvatars)
            {
                QuitProbe(0);
            }
        }

        public void OnInput(NetworkRunner callbackRunner, NetworkInput input)
        {
        }

        public void OnInputMissing(NetworkRunner callbackRunner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnSessionListUpdated(NetworkRunner callbackRunner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner callbackRunner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner callbackRunner, HostMigrationToken hostMigrationToken)
        {
            Report("Host migration no está habilitada en esta prueba.");
        }

        public void OnSceneLoadStart(NetworkRunner callbackRunner)
        {
            Report("Carga de escena iniciada.");
        }

        public void OnSceneLoadDone(NetworkRunner callbackRunner)
        {
            Report("Carga de escena terminada.");
        }

        public void OnObjectEnterAOI(NetworkRunner callbackRunner, NetworkObject networkObject, PlayerRef player)
        {
        }

        public void OnObjectExitAOI(NetworkRunner callbackRunner, NetworkObject networkObject, PlayerRef player)
        {
        }

        public void OnReliableDataReceived(NetworkRunner callbackRunner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner callbackRunner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner callbackRunner, SimulationMessagePtr message)
        {
        }

        private static int CountPlayers(NetworkRunner callbackRunner)
        {
            int count = 0;
            foreach (PlayerRef player in callbackRunner.ActivePlayers)
            {
                if (player.IsRealPlayer)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
