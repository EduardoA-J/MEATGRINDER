using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Aumenta temporalmente la velocidad de movimiento
    // Se integra con MovementSimulator mediante modificador temporal
    // PENDIENTE - valores configurables
    public sealed class HasteEffect : PowerUpEffect
    {
        public HasteEffect(float duration, float speedMultiplier) 
            : base(PowerUpType.Haste, duration, speedMultiplier)
        {
        }

        protected override void OnActivate()
        {
            // El multiplicador se aplica en el sistema de movimiento
            // mediante PowerUpManager.GetSpeedMultiplier()
            Debug.Log($"[PowerUp] Haste activado: {effectValue}x velocidad durante {duration}s");
        }

        protected override void OnDeactivate()
        {
            // Los valores se restauran automáticamente al consultar el multiplicador
            Debug.Log("[PowerUp] Haste expirado");
        }
    }
}
