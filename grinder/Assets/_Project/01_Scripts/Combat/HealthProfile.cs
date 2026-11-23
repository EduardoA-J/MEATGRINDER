using UnityEngine;

namespace Meatgrinder.Combat
{
    [CreateAssetMenu(fileName = "HealthProfile", menuName = "MEATGRINDER/Health Profile")]
    public sealed class HealthProfile : ScriptableObject
    {
        [SerializeField] private float maxHealth = 100f; // PENDIENTE
        [SerializeField, Min(0f)] private float maxArmor = 100f;
        [SerializeField, Range(0f, 1f)] private float armorAbsorption = 0.5f;

        public float MaxHealth => maxHealth > 0f ? maxHealth : 1f;
        public float MaxArmor => Mathf.Max(0f, maxArmor);
        public float ArmorAbsorption => Mathf.Clamp01(armorAbsorption);

        internal void SetMaxHealth(float value)
        {
            maxHealth = value;
        }
    }
}
