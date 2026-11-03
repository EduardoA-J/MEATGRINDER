using System;
using UnityEngine;

namespace Meatgrinder.Combat
{
    public sealed class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private HealthProfile profile;
        [SerializeField] private float maxHealth = 100f; // PENDIENTE
        [SerializeField] private int teamId;
        [SerializeField] private int stableId;

        static int nextGeneratedId = 1;

        bool idAssigned;
        bool isDead;

        public float Current { get; private set; }
        public float Max => profile != null ? profile.MaxHealth : (maxHealth > 0f ? maxHealth : 1f);
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

            Current = Mathf.Max(0f, Current - amount);
            Damaged?.Invoke(amount);
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
            EnsureId();
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
