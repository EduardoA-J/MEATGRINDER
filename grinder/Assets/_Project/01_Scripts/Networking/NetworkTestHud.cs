using UnityEngine;

namespace Meatgrinder.Networking
{
    public sealed class NetworkTestHud : MonoBehaviour
    {
        private NetworkSessionController session;
        private string sessionField = NetworkSessionController.DefaultSessionName;

        private void Awake()
        {
            session = GetComponent<NetworkSessionController>();
            if (session != null)
            {
                sessionField = session.SessionName;
            }
        }

        private void OnGUI()
        {
            if (session == null)
            {
                return;
            }

            const float width = 420f;
            GUILayout.BeginArea(new Rect(16f, 16f, width, Screen.height - 32f), GUI.skin.window);
            GUILayout.Label("MEATGRINDER — prueba de sesión");
            GUILayout.Label("Estado: " + session.Status);
            GUILayout.Label("Rol: " + session.RoleLabel);
            GUILayout.Label("Jugadores en sesión: " + session.PlayerCount);
            GUILayout.Label("Avatares visibles: " + NetworkTestAvatar.VisibleCount);

            GUILayout.Space(8f);
            GUILayout.Label("Nombre de sesión");
            sessionField = GUILayout.TextField(sessionField ?? string.Empty, 64);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Host"))
            {
                session.SetSessionName(sessionField);
                session.StartAsHost();
            }

            if (GUILayout.Button("Client"))
            {
                session.SetSessionName(sessionField);
                session.StartAsClient();
            }

            if (GUILayout.Button("Salir"))
            {
                session.LeaveSession();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(8f);
            GUILayout.Label("Diagnóstico");
            for (int i = 0; i < session.LogLines.Count; i++)
            {
                GUILayout.Label(session.LogLines[i]);
            }

            GUILayout.EndArea();
        }
    }
}
