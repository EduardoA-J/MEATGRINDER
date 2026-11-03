using UnityEngine;

namespace Meatgrinder.Combat
{
    [CreateAssetMenu(fileName = "HealthProfile", menuName = "MEATGRINDER/Health Profile")]
    public sealed class HealthProfile : ScriptableObject
    {
        [SerializeField] private float maxHealth = 100f; // PENDIENTE

        public float MaxHealth => maxHealth > 0f ? maxHealth : 1f;

        internal void SetMaxHealth(float value)
        {
            maxHealth = value;
        }
    }
}
