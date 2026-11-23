using System.Collections.Generic;
using UnityEngine;

namespace Meatgrinder.Combat
{
    // Local history only. Network authority must supply its own confirmed attack identity.
    public sealed class DamageResolver
    {
        const int HistoryCapacity = 8192;
        readonly HashSet<(int, int, DamageType, string, ulong)> confirmations = new();
        readonly Queue<(int, int, DamageType, string, ulong)> order = new();

        public bool FriendlyFire { get; set; }
        public IDamageModifiers AttackerPowerUps { get; set; }
        public IDamageModifiers TargetPowerUps { get; set; }

        public bool Apply(in DamageInfo info)
        {
            if (!info.HasAuthority || info.Target == null || !info.Target.IsAlive ||
                info.Amount <= 0f || float.IsNaN(info.Amount) || float.IsInfinity(info.Amount)) return false;

            bool environmental = info.Type == DamageType.Void;
            bool self = info.AttackerId == info.Target.StableId;
            if (!environmental)
            {
                if (info.ExcludeAttacker && self) return false;
                if (!self && !FriendlyFire && info.AttackerTeam != 0 && info.Target.TeamId == info.AttackerTeam) return false;
                if (info.Target is HealthComponent protectedBody && protectedBody.IsSpawnProtected) return false;
            }

            var key = (info.AttackerId, info.Target.StableId, info.Type, info.WeaponId, info.ConfirmationId);
            if (!confirmations.Add(key)) return false;
            order.Enqueue(key);
            if (order.Count > HistoryCapacity) confirmations.Remove(order.Dequeue());

            float finalDamage = info.Amount;
            if (!environmental)
            {
                if (info.Type == DamageType.Melee && AttackerPowerUps != null)
                    finalDamage *= AttackerPowerUps.GetMeleeMultiplier();
                if (AttackerPowerUps != null && !(self && info.Type == DamageType.Explosion))
                    finalDamage *= AttackerPowerUps.GetDamageMultiplier();
                if (TargetPowerUps != null)
                    finalDamage *= 1f - Mathf.Clamp01(TargetPowerUps.GetDamageReduction());
            }
            if (finalDamage <= 0f) return false;
            if (environmental && info.Target is HealthComponent body) body.ReceiveConfirmed(finalDamage, true);
            else info.Target.ReceiveConfirmed(finalDamage);
            return true;
        }
    }
}
