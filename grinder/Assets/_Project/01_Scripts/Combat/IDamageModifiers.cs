namespace Meatgrinder.Combat
{
    public interface IDamageModifiers
    {
        float GetDamageMultiplier();

        float GetDamageReduction();

        float GetMeleeMultiplier();
    }

    public interface ITemporaryEffects
    {
        void Clear();
    }
}
