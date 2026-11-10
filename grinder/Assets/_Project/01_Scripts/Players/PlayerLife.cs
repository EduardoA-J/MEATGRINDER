using Meatgrinder.Combat;
using Meatgrinder.Movement;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Players
{
    // Local life cycle. Durations stay on serialized fields marked PENDIENTE (D-08).
    // Host migration later runs this timer where ICombatAuthority is true.
    public sealed class PlayerLife : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float respawnDelay = 3f; // PENDIENTE D-08
        [SerializeField] private float spawnProtection = 1f; // PENDIENTE D-08

        bool waiting;
        float respawnRemaining;
        bool subscribed;

        public bool IsWaitingToRespawn => waiting;

        void Awake()
        {
            Resolve();
        }

        void OnEnable()
        {
            Resolve();
            Subscribe();
            if (health != null)
            {
                health.BeginSpawnProtection(spawnProtection);
            }
        }

        void OnDisable()
        {
            if (subscribed && health != null)
            {
                health.Died -= OnDied;
            }

            subscribed = false;
        }

        void FixedUpdate()
        {
            Tick(Time.fixedDeltaTime);
        }

        internal void Configure(float delay, float protection)
        {
            Resolve();
            Subscribe();
            respawnDelay = Mathf.Max(0f, delay);
            spawnProtection = Mathf.Max(0f, protection);
            if (health != null)
            {
                health.BeginSpawnProtection(spawnProtection);
            }
        }

        internal void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            if (health != null)
            {
                health.TickSpawnProtection(deltaTime);
            }

            if (waiting == false)
            {
                return;
            }

            respawnRemaining -= deltaTime;
            if (respawnRemaining > 0f)
            {
                return;
            }

            CompleteRespawn();
        }

        void Resolve()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            if (weapons == null)
            {
                weapons = GetComponent<WeaponController>();
            }

            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }
        }

        void Subscribe()
        {
            if (subscribed || health == null)
            {
                return;
            }

            health.Died += OnDied;
            subscribed = true;
        }

        void OnDied()
        {
            if (waiting)
            {
                return;
            }

            waiting = true;
            respawnRemaining = respawnDelay;
            GetComponent<ITemporaryEffects>()?.Clear();
            if (weapons != null)
            {
                weapons.Unequip();
            }

            if (movement != null)
            {
                movement.enabled = false;
            }
        }

        void CompleteRespawn()
        {
            waiting = false;
            if (spawnPoint != null)
            {
                transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            if (health != null)
            {
                health.RespawnAtFullHealth();
                health.BeginSpawnProtection(spawnProtection);
            }

            if (weapons != null)
            {
                weapons.Equip();
            }

            if (movement != null)
            {
                movement.enabled = true;
            }
        }
    }
}
