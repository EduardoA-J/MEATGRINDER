using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Recupera salud gradualmente durante un tiempo
    // PENDIENTE - valores configurables
    public sealed class RegenerationEffect : PowerUpEffect
    {
        HealthComponent health;
        float healthPerSecond;

        public RegenerationEffect(HealthComponent health, float duration, float healthPerSecond) 
            : base(PowerUpType.Regeneration, duration, healthPerSecond)
        {
            this.health = health;
            this.healthPerSecond = healthPerSecond;
        }

        protected override void OnActivate()
        {
            Debug.Log($"[PowerUp] Regeneration activado: {effectValue} HP/s durante {duration}s");
        }

        protected override void OnDeactivate()
        {
            Debug.Log("[PowerUp] Regeneration expirado");
        }

        protected override void OnTick(float deltaTime)
        {
            if (health == null || health.IsAlive == false || health.Current >= health.Max)
            {
                return;
            }

            health.AddHealth(healthPerSecond * deltaTime);
        }
    }
}
