using UnityEngine;

namespace Meatgrinder.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject sessionPrefab;
        void Awake()
        {
            if (FindFirstObjectByType<LocalGameSession>() == null && sessionPrefab != null)
                Instantiate(sessionPrefab).name = "LocalGameSession";
        }
    }
}
