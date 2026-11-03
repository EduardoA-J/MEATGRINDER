using System.Collections.Generic;

namespace Meatgrinder.Combat
{
    // One confirmation changes health once. Without a Fusion host this object is local.
    // Later the host calls Apply inside FixedUpdateNetwork. Clients only read the result.
    public sealed class DamageResolver
    {
        readonly HashSet<ulong> confirmations = new HashSet<ulong>();

        public bool FriendlyFire { get; set; }

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

            info.Target.ReceiveConfirmed(info.Amount);
            return true;
        }
    }
}
