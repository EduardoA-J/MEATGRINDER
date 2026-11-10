using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Permite superar temporalmente el límite de salud con degradación
    // PENDIENTE - valores configurables
    public sealed class MegahealthEffect : PowerUpEffect
    {
        HealthComponent health;
        float degradationPerSecond;
        float normalMax;

        public MegahealthEffect(HealthComponent health, float duration, float amount, float degradationRate) 
            : base(PowerUpType.Megahealth, duration, amount)
        {
            this.health = health;
            this.degradationPerSecond = degradationRate;
        }

        protected override void OnActivate()
        {
            if (health == null)
            {
                return;
            }

            normalMax = health.Max;
            health.AddHealth(effectValue, true);
            Debug.Log($"[PowerUp] Megahealth activado: +{effectValue} HP durante {duration}s");
        }

        protected override void OnDeactivate()
        {
            Debug.Log("[PowerUp] Megahealth expirado");
        }

        protected override void OnTick(float deltaTime)
        {
            if (health == null || health.Current <= normalMax)
            {
                Deactivate();
                return;
            }

            if (degradationPerSecond > 0f)
            {
                health.DecayToward(normalMax, degradationPerSecond * deltaTime);
            }
        }
    }
}
