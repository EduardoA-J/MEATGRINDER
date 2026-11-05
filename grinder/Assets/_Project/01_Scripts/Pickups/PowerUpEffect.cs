using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Clase base para efectos de power-ups temporales
    public abstract class PowerUpEffect
    {
        protected PowerUpType type;
        protected float duration;
        protected float effectValue;
        protected float timeRemaining;
        protected bool active;

        public PowerUpType Type => type;
        public float Duration => duration;
        public float TimeRemaining => timeRemaining;
        public bool IsActive => active;
        public float EffectValue => effectValue;

        public PowerUpEffect(PowerUpType type, float duration, float effectValue)
        {
            this.type = type;
            this.duration = duration;
            this.effectValue = effectValue;
            this.timeRemaining = duration;
            this.active = false;
        }

        public virtual void Activate()
        {
            active = true;
            timeRemaining = duration;
            OnActivate();
        }

        public virtual void Deactivate()
        {
            active = false;
            OnDeactivate();
        }

        public virtual void Tick(float deltaTime)
        {
            if (active == false)
            {
                return;
            }

            timeRemaining -= deltaTime;
            if (timeRemaining <= 0f)
            {
                Deactivate();
                return;
            }

            OnTick(deltaTime);
        }

        public virtual void Extend(float additionalDuration)
        {
            timeRemaining += additionalDuration;
        }

        public virtual void Reset()
        {
            timeRemaining = duration;
        }

        protected abstract void OnActivate();
        protected abstract void OnDeactivate();
        protected virtual void OnTick(float deltaTime) { }
    }
}
