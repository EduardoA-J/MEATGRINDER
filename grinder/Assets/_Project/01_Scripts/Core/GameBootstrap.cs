using UnityEngine;

namespace Meatgrinder.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject sessionPrefab;
        [SerializeField] private bool loadMenuOnStart;

        void Awake()
        {
            var session = FindFirstObjectByType<LocalGameSession>();
            if (session == null && sessionPrefab != null)
            {
                var instance = Instantiate(sessionPrefab);
                instance.name = "LocalGameSession";
                session = instance.GetComponent<LocalGameSession>();
            }
            if (loadMenuOnStart && session != null) session.ReturnToMenu();
        }
    }
}
