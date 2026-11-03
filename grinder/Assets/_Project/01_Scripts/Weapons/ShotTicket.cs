namespace Meatgrinder.Weapons
{
    public readonly struct ShotTicket
    {
        ShotTicket(bool accepted, ShotRejectReason reason, WeaponDefinition weapon, ulong confirmationBase)
        {
            Accepted = accepted;
            Reason = reason;
            Weapon = weapon;
            ConfirmationBase = confirmationBase;
        }

        public bool Accepted { get; }
        public ShotRejectReason Reason { get; }
        public WeaponDefinition Weapon { get; }
        public ulong ConfirmationBase { get; }

        public static ShotTicket Ok(WeaponDefinition weapon, ulong confirmationBase)
        {
            return new ShotTicket(true, ShotRejectReason.None, weapon, confirmationBase);
        }

        public static ShotTicket Rejected(ShotRejectReason reason)
        {
            return new ShotTicket(false, reason, null, 0);
        }
    }
}
