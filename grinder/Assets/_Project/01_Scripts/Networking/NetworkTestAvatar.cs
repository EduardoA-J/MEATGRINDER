using Fusion;
using UnityEngine;

namespace Meatgrinder.Networking
{
    public sealed class NetworkTestAvatar : NetworkBehaviour
    {
        public static int VisibleCount { get; private set; }

        public static void ResetCount()
        {
            VisibleCount = 0;
        }

        public override void Spawned()
        {
            VisibleCount++;
            int playerId = Object.InputAuthority.PlayerId;
            name = "NetworkTestPlayer-" + playerId;

            Renderer avatarRenderer = GetComponent<Renderer>();
            if (avatarRenderer != null)
            {
                avatarRenderer.material.color = Color.HSVToRGB(Mathf.Repeat(playerId * 0.17f, 1f), 0.75f, 0.95f);
            }

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            TextMesh label = labelObject.AddComponent<TextMesh>();
            label.text = "P" + playerId;
            label.characterSize = 0.15f;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.color = Color.white;

            Debug.Log("[NetworkTest] Avatar spawned. PlayerId=" + playerId + " position=" + transform.position);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            VisibleCount = Mathf.Max(0, VisibleCount - 1);
            Debug.Log("[NetworkTest] Avatar despawned. VisibleCount=" + VisibleCount);
        }
    }
}
