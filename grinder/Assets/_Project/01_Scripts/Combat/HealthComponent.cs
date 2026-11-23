using System;
using UnityEngine;

namespace Meatgrinder.Combat
{
    public sealed class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthProfile profile;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxArmor = 100f; // D-07: máximo 100
        [SerializeField] private float armorAbsorption = 0.5f; // D-07: absorbe 50 %
        [SerializeField] private int teamId;
        [SerializeField] private int stableId;

        static int nextGeneratedId = 1;

        bool idAssigned;
        bool isDead;
        float spawnProtectionRemaining;

        public bool RecoverySuppressed { get; set; }
        public float Current { get; private set; }
        public float Armor { get; private set; }
        public float Max => profile != null ? profile.MaxHealth : (maxHealth > 0f ? maxHealth : 1f);
        public float MaxArmor => profile != null ? profile.MaxArmor : Mathf.Max(0f, maxArmor);
        public float ArmorAbsorption => profile != null ? profile.ArmorAbsorption : Mathf.Clamp01(armorAbsorption);
        public int TeamId => teamId;
        public bool IsAlive => isDead == false && Current > 0f;
        public bool IsSpawnProtected => spawnProtectionRemaining > 0f;

        public int StableId
        {
            get
            {
                EnsureId();
                return stableId;
            }
        }

        public event Action<float> Damaged;
        public event Action<float> HealthChanged;
        public event Action<float> ArmorChanged;
        public event Action Died;

        void Awake()
        {
            ResetHealth();
        }

        public void ReceiveConfirmed(float amount)
        {
            ReceiveConfirmed(amount, false);
        }

        internal void ReceiveConfirmed(float amount, bool bypassArmor)
        {
            if (isDead || amount <= 0f)
            {
                return;
            }

            // Aplicar absorción de armadura si hay armadura disponible
            float damageToHealth = amount;
            if (!bypassArmor && Armor > 0f && ArmorAbsorption > 0f)
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
            bool diedNow = Current <= 0f;
            isDead = diedNow;
            HealthChanged?.Invoke(Current);
            Damaged?.Invoke(amount); // El evento reporta el daño total recibido, no solo el que llegó a salud
            if (!diedNow)
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
            spawnProtectionRemaining = 0f;
            RecoverySuppressed = false;
            EnsureId();
            HealthChanged?.Invoke(Current);
            ArmorChanged?.Invoke(Armor);
        }

        public void AddArmor(float amount)
        {
            if (amount <= 0f || isDead || RecoverySuppressed)
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
            AddHealth(amount, false);
        }

        public void AddHealth(float amount, bool allowAboveMax)
        {
            if (amount <= 0f || isDead || RecoverySuppressed)
            {
                return;
            }

            Current = allowAboveMax ? Current + amount : Mathf.Max(Current, Mathf.Min(Max, Current + amount));
            HealthChanged?.Invoke(Current);
        }

        public void AddOverhealth(float amount, float cap)
        {
            if (isDead || RecoverySuppressed || amount <= 0f) return;
            Current = Mathf.Max(Current, Mathf.Min(Mathf.Max(Max, cap), Current + amount));
            HealthChanged?.Invoke(Current);
        }

        public void DecayToward(float floor, float amount)
        {
            if (isDead || RecoverySuppressed || amount <= 0f || Current <= floor)
            {
                return;
            }

            Current = Mathf.Max(floor, Current - amount);
            HealthChanged?.Invoke(Current);
        }

        public void BeginSpawnProtection(float seconds)
        {
            spawnProtectionRemaining = Mathf.Max(0f, seconds);
        }

        public void TickSpawnProtection(float deltaTime)
        {
            if (spawnProtectionRemaining <= 0f || deltaTime <= 0f)
            {
                return;
            }

            spawnProtectionRemaining = Mathf.Max(0f, spawnProtectionRemaining - deltaTime);
        }

        public void RespawnAtFullHealth()
        {
            ResetHealth();
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
