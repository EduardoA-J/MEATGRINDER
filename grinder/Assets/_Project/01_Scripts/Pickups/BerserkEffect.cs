using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Potencia temporalmente ataques melee
    // PENDIENTE - valores configurables, requiere ataques melee implementados
    public sealed class BerserkEffect : PowerUpEffect
    {
        public BerserkEffect(float duration, float meleeMultiplier) 
            : base(PowerUpType.Berserk, duration, meleeMultiplier)
        {
        }

        protected override void OnActivate()
        {
            Debug.Log($"[PowerUp] Berserk activado: {effectValue}x daño melee durante {duration}s");
        }

        protected override void OnDeactivate()
        {
            Debug.Log("[PowerUp] Berserk expirado");
        }
    }
}
