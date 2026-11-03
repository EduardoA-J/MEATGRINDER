using UnityEngine;

namespace Meatgrinder.Combat
{
    public readonly struct DamageInfo
    {
        public DamageInfo(
            int attackerId,
            int attackerTeam,
            IDamageable target,
            float amount,
            DamageType type,
            Vector3 point,
            ulong confirmationId,
            bool hasAuthority,
            bool excludeAttacker,
            string weaponId)
        {
            AttackerId = attackerId;
            AttackerTeam = attackerTeam;
            Target = target;
            Amount = amount;
            Type = type;
            Point = point;
            ConfirmationId = confirmationId;
            HasAuthority = hasAuthority;
            ExcludeAttacker = excludeAttacker;
            WeaponId = weaponId ?? string.Empty;
        }

        public int AttackerId { get; }
        public int AttackerTeam { get; }
        public IDamageable Target { get; }
        public float Amount { get; }
        public DamageType Type { get; }
        public Vector3 Point { get; }
        public ulong ConfirmationId { get; }
        public bool HasAuthority { get; }
        public bool ExcludeAttacker { get; }
        public string WeaponId { get; }
    }
}
