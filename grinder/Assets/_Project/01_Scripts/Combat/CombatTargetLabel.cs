using UnityEngine;

namespace Meatgrinder.Combat
{
    // Health binding kept for the scene. The readable label lives on TargetBillboard.
    public sealed class CombatTargetLabel : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;

        public HealthComponent Health => health;

        void Awake()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }
        }
    }
}
