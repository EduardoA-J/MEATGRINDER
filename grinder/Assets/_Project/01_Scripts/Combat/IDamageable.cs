namespace Meatgrinder.Combat
{
    public interface IDamageable
    {
        int StableId { get; }
        int TeamId { get; }
        bool IsAlive { get; }

        void ReceiveConfirmed(float amount);
    }
}
