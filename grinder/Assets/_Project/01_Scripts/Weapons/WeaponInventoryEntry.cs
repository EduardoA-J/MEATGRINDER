namespace Meatgrinder.Weapons
{
    public readonly struct WeaponInventoryEntry
    {
        public WeaponInventoryEntry(WeaponDefinition weapon, int loaded, int reserve)
        {
            Weapon = weapon;
            Loaded = loaded;
            Reserve = reserve;
        }
        public WeaponDefinition Weapon { get; }
        public int Loaded { get; }
        public int Reserve { get; }
    }
}
