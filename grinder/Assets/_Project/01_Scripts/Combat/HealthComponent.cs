using System;
using UnityEngine;

namespace Meatgrinder.Combat
{
    public sealed class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthProfile profile;
        [SerializeField] private float maxHealth = 100f; // PENDIENTE
        [SerializeField] private float maxArmor = 100f; // PENDIENTE - D-07 propuesta: 100
        [SerializeField] private float armorAbsorption = 0.5f; // PENDIENTE - D-07 propuesta: 50%
        [SerializeField] private int teamId;
        [SerializeField] private int stableId;

        static int nextGeneratedId = 1;

        bool idAssigned;
        bool isDead;

        public float Current { get; private set; }
        public float Armor { get; private set; }
        public float Max => profile != null ? profile.MaxHealth : (maxHealth > 0f ? maxHealth : 1f);
        public float MaxArmor => maxArmor > 0f ? maxArmor : 0f;
        public float ArmorAbsorption => Mathf.Clamp01(armorAbsorption);
        public int TeamId => teamId;
        public bool IsAlive => isDead == false && Current > 0f;

        public int StableId
        {
            get
            {
                EnsureId();
                return stableId;
            }
        }

        public event Action<float> Damaged;
        public event Action<float> ArmorChanged;
        public event Action Died;

        void Awake()
        {
            ResetHealth();
        }

        public void ReceiveConfirmed(float amount)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            // Aplicar absorción de armadura si hay armadura disponible
            float damageToHealth = amount;
            if (Armor > 0f && ArmorAbsorption > 0f)
            {
                float absorbed = amount * ArmorAbsorption;
                float armorDamage = Mathf.Min(absorbed, Armor);
                Armor = Mathf.Max(0f, Armor - armorDamage);
                ArmorChanged?.Invoke(Armor);
                
                // El daño restante va a salud
                float absorptionRatio = armorDamage / absorbed;
                damageToHealth = amount * (1f - (ArmorAbsorption * absorptionRatio));
            }

            Current = Mathf.Max(0f, Current - damageToHealth);
            Damaged?.Invoke(amount); // El evento reporta el daño total recibido, no solo el que llegó a salud
            if (Current > 0f)
            {
                return;
            }

            isDead = true;
            Died?.Invoke();
        }

        internal void ResetHealth()
        {
            isDead = false;
            Current = Max;
            Armor = 0f; // La armadura no se resetea automáticamente con la salud
            EnsureId();
        }

        public void AddArmor(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            float previous = Armor;
            Armor = Mathf.Min(MaxArmor, Armor + amount);
            if (Mathf.Abs(Armor - previous) > 0.01f)
            {
                ArmorChanged?.Invoke(Armor);
            }
        }

        public void AddHealth(float amount)
        {
            if (amount <= 0f || isDead)
            {
                return;
            }

            Current = Mathf.Min(Max, Current + amount);
        }

        internal void SetTeam(int value)
        {
            teamId = value;
        }

        internal void SetStableId(int value)
        {
            stableId = value;
            idAssigned = true;
        }

        internal void UseProfile(HealthProfile value)
        {
            profile = value;
            ResetHealth();
        }

        void EnsureId()
        {
            if (idAssigned)
            {
                return;
            }

            if (stableId == 0)
            {
                stableId = nextGeneratedId++;
            }

            idAssigned = true;
        }
    }
}
