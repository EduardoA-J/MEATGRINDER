using System.Collections.Generic;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Gestiona los efectos de power-ups activos en un jugador
    public sealed class PowerUpManager : MonoBehaviour, Meatgrinder.Combat.IDamageModifiers, Meatgrinder.Combat.ITemporaryEffects, Meatgrinder.Movement.ISpeedScale
    {
        readonly Dictionary<PowerUpType, PowerUpEffect> activeEffects = new Dictionary<PowerUpType, PowerUpEffect>();

        public event System.Action<PowerUpType, float> PowerUpActivated;
        public event System.Action<PowerUpType> PowerUpExpired;

        public bool HasEffect(PowerUpType type)
        {
            return activeEffects.ContainsKey(type) && activeEffects[type].IsActive;
        }

        public float GetTimeRemaining(PowerUpType type)
        {
            return activeEffects.ContainsKey(type) ? activeEffects[type].TimeRemaining : 0f;
        }

        public float GetEffectValue(PowerUpType type)
        {
            return activeEffects.ContainsKey(type) ? activeEffects[type].EffectValue : 0f;
        }

        void FixedUpdate()
        {
            List<PowerUpType> expired = new List<PowerUpType>();
            foreach (var kvp in activeEffects)
            {
                kvp.Value.Tick(Time.fixedDeltaTime);
                if (kvp.Value.IsActive == false)
                {
                    expired.Add(kvp.Key);
                }
            }

            foreach (var type in expired)
            {
                activeEffects.Remove(type);
                PowerUpExpired?.Invoke(type);
            }
        }

        public void ActivatePowerUp(PowerUpEffect effect)
        {
            if (effect == null)
            {
                return;
            }

            PowerUpType type = effect.Type;
            
            // D-11 pendiente. Regla provisional: el mismo tipo reinicia con la definición nueva.
            // No suma duraciones.
            if (activeEffects.ContainsKey(type))
            {
                activeEffects[type].Deactivate();
            }

            activeEffects[type] = effect;
            effect.Activate();

            PowerUpActivated?.Invoke(type, effect.Duration);
        }

        public float GetDamageMultiplier()
        {
            if (HasEffect(PowerUpType.QuadDamage))
            {
                return GetEffectValue(PowerUpType.QuadDamage);
            }

            return 1f;
        }

        public float GetSpeedMultiplier()
        {
            if (HasEffect(PowerUpType.Haste))
            {
                return GetEffectValue(PowerUpType.Haste);
            }

            return 1f;
        }

        public float GetDamageReduction()
        {
            if (HasEffect(PowerUpType.Invulnerability))
            {
                return Mathf.Clamp01(GetEffectValue(PowerUpType.Invulnerability));
            }

            return 0f;
        }

        public float GetMeleeMultiplier()
        {
            if (HasEffect(PowerUpType.Berserk))
            {
                return GetEffectValue(PowerUpType.Berserk);
            }

            return 1f;
        }

        public void Clear()
        {
            foreach (PowerUpEffect effect in activeEffects.Values)
            {
                effect.Deactivate();
            }

            activeEffects.Clear();
        }

        public void Grant(PickupDefinition definition, Meatgrinder.Combat.HealthComponent health)
        {
            if (definition == null || definition.Type != PickupType.PowerUp)
            {
                return;
            }

            PowerUpEffect effect = CreateEffect(definition, health);
            if (effect != null)
            {
                ActivatePowerUp(effect);
            }
        }

        static PowerUpEffect CreateEffect(PickupDefinition definition, Meatgrinder.Combat.HealthComponent health)
        {
            float duration = definition.Duration;
            float value = definition.EffectValue;
            switch (definition.PowerUpType)
            {
                case PowerUpType.QuadDamage:
                    return new QuadDamageEffect(duration, value);
                case PowerUpType.Invulnerability:
                    return new InvulnerabilityEffect(duration, value);
                case PowerUpType.Berserk:
                    return new BerserkEffect(duration, value);
                case PowerUpType.Haste:
                    return new HasteEffect(duration, value);
                case PowerUpType.Regeneration:
                    return new RegenerationEffect(health, duration, value);
                case PowerUpType.Megahealth:
                    return new MegahealthEffect(health, duration, value, duration > 0f ? value / duration : 0f);
                default:
                    return null;
            }
        }
    }
}
