using System.Collections.Generic;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Gestiona los efectos de power-ups activos en un jugador
    public sealed class PowerUpManager : MonoBehaviour
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
            
            // Si ya existe, aplicar regla de acumulación
            // D-11 pendiente - por ahora reinicia duración
            if (activeEffects.ContainsKey(type))
            {
                activeEffects[type].Reset();
            }
            else
            {
                activeEffects[type] = effect;
                effect.Activate();
            }

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
                return GetEffectValue(PowerUpType.Invulnerability);
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
    }
}
