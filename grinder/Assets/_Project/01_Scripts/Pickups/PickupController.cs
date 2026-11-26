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
        public bool Replica { get; set; }
        public void ShowReplica(bool value)
        {
            if (visual != null) visual.gameObject.SetActive(value);
            if (trigger != null) trigger.enabled = false;
        }

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
            if (Replica) return;
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
            if (Replica || available == false || definition == null)
            {
                return;
            }

            // Verificar que es un jugador
            HealthComponent health = other.GetComponentInParent<HealthComponent>();
            if (health == null || health.IsAlive == false)
            {
                return;
            }

            CollectAuthoritatively(health);
        }

        bool CanPickup(HealthComponent health)
        {
            if (!health.AllowWeapons && (definition.Type == PickupType.Weapon || definition.Type == PickupType.Ammo))
                return false;
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
                    return health.GetComponent<PowerUpManager>() != null;
                case PickupType.Weapon:
                    return definition.Weapon != null && health.GetComponent<WeaponController>() != null;
                case PickupType.Ammo:
                    return health.GetComponent<WeaponController>()?.HasWeapon(definition.Weapon) == true;

                default:
                    return false;
            }
        }

        internal bool Collect(HealthComponent health) => CollectAuthoritatively(health);

        public bool CollectAuthoritatively(HealthComponent health)
        {
            if (Replica || available == false || definition == null || health == null || health.IsAlive == false)
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

                case PickupType.Ammo:
                    health.GetComponent<WeaponController>()?.AddAmmo(definition.Weapon, definition.AmmoAmount);
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
