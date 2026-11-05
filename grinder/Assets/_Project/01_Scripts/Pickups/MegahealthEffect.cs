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
            health.AddHealth(effectValue); // Añade salud adicional
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

            // Degradar salud por encima del límite normal
            float excess = health.Current - normalMax;
            if (excess > 0f && degradationPerSecond > 0f)
            {
                // Reducir directamente sin pasar por ReceiveConfirmed
                float reduction = Mathf.Min(excess, degradationPerSecond * deltaTime);
                // Nota: necesitaríamos un método interno en HealthComponent para esto
                // Por ahora solo reportamos
            }
        }
    }
}
