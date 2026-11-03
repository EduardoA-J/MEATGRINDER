using Fusion;
using UnityEngine;

namespace Meatgrinder.Networking
{
    public sealed class NetworkPlayerSpawner : MonoBehaviour
    {
        [SerializeField] private NetworkPrefabRef playerPrefab;

        public bool HasPrefab => playerPrefab.IsValid;

        public void AssignPrefab(NetworkPrefabRef prefab)
        {
            playerPrefab = prefab;
        }

        public void SpawnFor(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer == false)
            {
                return;
            }

            if (playerPrefab.IsValid == false)
            {
                Debug.LogError("[NetworkTest] Player prefab is not assigned.");
                return;
            }

            Vector3 position = new Vector3(player.AsIndex * 2.5f, 1f, 0f);
            NetworkObject avatar = runner.Spawn(playerPrefab, position, Quaternion.identity, player, BeforeSpawned);
            if (avatar == null)
            {
                Debug.LogError("[NetworkTest] Host failed to spawn player " + player.PlayerId);
                return;
            }

            runner.SetPlayerObject(player, avatar);
            Debug.Log("[NetworkTest] Host spawned player " + player.PlayerId + " at " + position);
        }

        public void DespawnFor(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer == false)
            {
                return;
            }

            if (runner.TryGetPlayerObject(player, out NetworkObject avatar) == false || avatar == null)
            {
                Debug.LogWarning("[NetworkTest] Player " + player.PlayerId + " left without a player object.");
                return;
            }

            runner.Despawn(avatar);
            Debug.Log("[NetworkTest] Host despawned player " + player.PlayerId);
        }

        private static void BeforeSpawned(NetworkRunner runner, NetworkObject networkObject)
        {
            int index = networkObject.InputAuthority.AsIndex;
            networkObject.transform.SetPositionAndRotation(new Vector3(index * 2.5f, 1f, 0f), Quaternion.identity);
        }
    }
}
