using Meatgrinder.Combat;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    // Componente de escena que representa un pickup.
    // La validación autoritativa vendrá después con Fusion.
    public sealed class PickupController : MonoBehaviour
    {
        [SerializeField] private PickupDefinition definition;
        [SerializeField] private Transform visual;
        [SerializeField] private Collider trigger;
        [SerializeField] private float rotationSpeed = 45f;
        
        float respawnTimer;
        bool available = true;

        public event System.Action<PickupDefinition, GameObject> PickedUp;

        public PickupDefinition Definition => definition;
        public bool IsAvailable => available;

        void Awake()
        {
            if (trigger == null)
            {
                trigger = GetComponent<Collider>();
            }

            if (trigger != null)
            {
                trigger.isTrigger = true;
            }
        }

        void Update()
        {
            if (available == false)
            {
                respawnTimer -= Time.deltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            else if (visual != null)
            {
                visual.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (available == false || definition == null)
            {
                return;
            }

            // Verificar que es un jugador
            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health == null || health.IsAlive == false)
            {
                return;
            }

            Collect(health);
        }

        bool CanPickup(HealthComponent health)
        {
            if (definition == null)
            {
                return false;
            }

            switch (definition.Type)
            {
                case PickupType.Health:
                    return definition.CanPickupWhenFull || health.Current < health.Max;

                case PickupType.Armor:
                    return definition.CanPickupWhenFull || health.Armor < health.MaxArmor;

                case PickupType.PowerUp:
                case PickupType.Weapon:
                    return true;

                default:
                    return false;
            }
        }

        internal bool Collect(HealthComponent health)
        {
            if (available == false || definition == null || health == null || health.IsAlive == false)
            {
                return false;
            }

            if (CanPickup(health) == false)
            {
                return false;
            }

            ApplyPickup(health);
            PickedUp?.Invoke(definition, health.gameObject);
            Hide();
            return true;
        }

        void ApplyPickup(HealthComponent health)
        {
            if (definition == null)
            {
                return;
            }

            switch (definition.Type)
            {
                case PickupType.Health:
                    health.AddHealth(definition.Amount);
                    break;

                case PickupType.Armor:
                    health.AddArmor(definition.Amount);
                    break;

                case PickupType.PowerUp:
                    PowerUpManager powers = health.GetComponent<PowerUpManager>();
                    if (powers != null)
                    {
                        powers.Grant(definition, health);
                    }

                    break;

                case PickupType.Weapon:
                    WeaponController weapons = health.GetComponent<WeaponController>();
                    if (weapons != null && definition.Weapon != null)
                    {
                        weapons.TryGrant(definition.Weapon, definition.AmmoAmount);
                    }

                    break;
            }
        }

        void Hide()
        {
            available = false;
            respawnTimer = definition != null ? definition.RespawnTime : 0f;
            if (visual != null)
            {
                visual.gameObject.SetActive(false);
            }

            if (trigger != null)
            {
                trigger.enabled = false;
            }
        }

        void Respawn()
        {
            available = true;
            if (visual != null)
            {
                visual.gameObject.SetActive(true);
            }

            if (trigger != null)
            {
                trigger.enabled = true;
            }
        }

        internal void ForceRespawn()
        {
            Respawn();
        }

        internal void SetDefinition(PickupDefinition value)
        {
            definition = value;
        }
    }
}
