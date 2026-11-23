using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Multiplica el daño temporalmente
    // D-10: x4 durante 30 s; la cifra permanece configurable en PickupDefinition.
    public sealed class QuadDamageEffect : PowerUpEffect
    {
        public QuadDamageEffect(float duration, float multiplier) 
            : base(PowerUpType.QuadDamage, duration, multiplier)
        {
        }

        protected override void OnActivate()
        {
            // El multiplicador se aplica en DamageResolver mediante PowerUpManager
            Debug.Log($"[PowerUp] Quad Damage activado: {effectValue}x durante {duration}s");
        }

        protected override void OnDeactivate()
        {
            Debug.Log("[PowerUp] Quad Damage expirado");
        }
    }
}
