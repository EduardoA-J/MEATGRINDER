using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    public sealed class MegahealthEffect : PowerUpEffect
    {
        readonly HealthComponent health;
        readonly float degradationPerSecond;
        readonly float cap;
        readonly float delay;
        float delayRemaining;

        public MegahealthEffect(HealthComponent health, float duration, float amount, float degradationRate,
            float cap = 200f, float delay = 5f) : base(PowerUpType.Megahealth, duration, amount)
        {
            this.health = health;
            this.degradationPerSecond = Mathf.Max(0.01f, degradationRate);
            this.cap = cap;
            this.delay = delay;
        }

        protected override void OnActivate()
        {
            delayRemaining = delay;
            if (health == null) return;
            health.AddOverhealth(effectValue, cap);
            duration = timeRemaining = delay + Mathf.Max(0f, health.Current - health.Max) / degradationPerSecond;
        }

        protected override void OnDeactivate() { }

        protected override void OnTick(float deltaTime)
        {
            if (health == null || !health.IsAlive || health.Current <= health.Max)
            {
                Deactivate();
                return;
            }
            float waiting = Mathf.Min(deltaTime, delayRemaining);
            delayRemaining -= waiting;
            health.DecayToward(health.Max, degradationPerSecond * (deltaTime - waiting));
        }
    }
}
