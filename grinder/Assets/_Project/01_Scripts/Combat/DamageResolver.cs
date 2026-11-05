using System.Collections.Generic;

namespace Meatgrinder.Combat
{
    // One confirmation changes health once. Without a Fusion host this object is local.
    // Later the host calls Apply inside FixedUpdateNetwork. Clients only read the result.
    public sealed class DamageResolver
    {
        readonly HashSet<ulong> confirmations = new HashSet<ulong>();

        public bool FriendlyFire { get; set; }
        public Meatgrinder.Pickups.PowerUpManager AttackerPowerUps { get; set; }
        public Meatgrinder.Pickups.PowerUpManager TargetPowerUps { get; set; }

        public bool Apply(in DamageInfo info)
        {
            if (info.HasAuthority == false || info.Target == null || info.Target.IsAlive == false || info.Amount <= 0f)
            {
                return false;
            }

            if (confirmations.Add(info.ConfirmationId) == false)
            {
                return false;
            }

            if (info.ExcludeAttacker && info.AttackerId == info.Target.StableId)
            {
                return false;
            }

            if (FriendlyFire == false
                && info.AttackerTeam != 0
                && info.Target.TeamId == info.AttackerTeam)
            {
                return false;
            }

            // Aplicar modificadores de power-ups ANTES de aplicar el daño
            float finalDamage = info.Amount;

            // Quad Damage del atacante (multiplica daño de salida)
            if (AttackerPowerUps != null)
            {
                float damageMultiplier = AttackerPowerUps.GetDamageMultiplier();
                if (damageMultiplier != 1f)
                {
                    finalDamage *= damageMultiplier;
                }
            }

            // Invulnerability del objetivo (reduce daño de entrada)
            if (TargetPowerUps != null)
            {
                float damageReduction = TargetPowerUps.GetDamageReduction();
                if (damageReduction > 0f)
                {
                    finalDamage *= (1f - damageReduction);
                }
            }

            info.Target.ReceiveConfirmed(finalDamage);
            return true;
        }
    }
}
