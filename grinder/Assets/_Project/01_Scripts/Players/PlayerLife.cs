using Meatgrinder.Combat;
using Meatgrinder.Movement;
using Meatgrinder.Pickups;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class PlayerLife : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private LocalSpawnPoints map;
        [SerializeField] private LifeTuning tuning;
        float respawnDelay = 4f;
        float spawnProtection = 1f;
        bool waiting;
        float respawnRemaining;
        bool subscribed;
        Vector3 deathPosition;
        Transform previousSpawn;
        public bool IsWaitingToRespawn => waiting;
        public float RespawnRemaining => Mathf.Max(0f, respawnRemaining);
        public string DeathCause { get; private set; }
        public event System.Action Respawned;
        float Delay => tuning != null ? tuning.RespawnDelay : respawnDelay;
        float Protection => tuning != null ? tuning.SpawnProtection : spawnProtection;

        void Awake() => Resolve();
        void Start()
        {
            Resolve();
            EnsureSpawnMap();
            // Product spawns begin melee-only. Standalone combat harnesses do not have a map spawn
            // and must retain their test loadout.
            if (spawnPoint != null || map != null || gameObject.name == "LocalPlayer") weapons?.ClearInventory();
            health?.BeginSpawnProtection(Protection);
            previousSpawn = spawnPoint;
            if (spawnPoint != null)
            {
                var body = GetComponent<CharacterController>();
                bool wasEnabled = body != null && body.enabled;
                if (body != null) body.enabled = false;
                transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                GetComponent<PlayerLook>()?.ResetLook(spawnPoint.rotation);
                movement?.ResetMotion();
                if (body != null) body.enabled = wasEnabled;
            }
        }
        void OnEnable() { Resolve(); Subscribe(); }
        void OnDisable()
        {
            if (subscribed && health != null) health.Died -= OnDied;
            subscribed = false;
        }
        void FixedUpdate() => Tick(Time.fixedDeltaTime);

        internal void Configure(float delay, float protection)
        {
            Resolve();
            Subscribe();
            tuning = null;
            respawnDelay = Mathf.Max(0f, delay);
            spawnProtection = Mathf.Max(0f, protection);
            health?.BeginSpawnProtection(Protection);
        }
        internal void Tick(float deltaTime)
        {
            if (deltaTime <= 0f) return;
            var authority = GetComponent<LocalCombatAuthority>();
            if (authority != null && !authority.IsAuthoritative) return;
            health?.TickSpawnProtection(deltaTime);
            if (!waiting) return;
            respawnRemaining -= deltaTime;
            if (respawnRemaining <= 0f) CompleteRespawn();
        }
        void Resolve()
        {
            if (health == null) health = GetComponent<HealthComponent>();
            if (weapons == null) weapons = GetComponent<WeaponController>();
            if (movement == null) movement = GetComponent<PlayerMovement>();
            if (map == null) map = FindFirstObjectByType<LocalSpawnPoints>();
        }

        void EnsureSpawnMap()
        {
            if (map != null || (spawnPoint == null && gameObject.name != "LocalPlayer")) return;
            GameObject root = new GameObject("RuntimeSpawnPoints");
            map = root.AddComponent<LocalSpawnPoints>();
            var points = new System.Collections.Generic.List<Transform>();
            foreach (Transform marker in FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (marker.name.StartsWith("Spawn_Human_")) points.Add(marker);
            map.SetPoints(points.ToArray());
        }
        void Subscribe()
        {
            if (subscribed || health == null) return;
            health.Died += OnDied;
            subscribed = true;
        }
        void OnDied()
        {
            if (waiting) return;
            waiting = true;
            deathPosition = transform.position;
            DeathCause = GetComponent<PlayerVoidFall>()?.IsFalling == true ? "CAÍDA AL VACÍO" : "DAÑO DE COMBATE";
            respawnRemaining = Delay;
            GetComponent<ITemporaryEffects>()?.Clear();
            if (weapons != null)
            {
                DeathLoot loot = DeathLoot.Drop(deathPosition, movement != null ? movement.Velocity : Vector3.zero,
                    weapons.CaptureInventory(), tuning != null ? tuning.LootLifetime : 90f, tuning != null ? tuning.LootPrefab : null);
                if (loot != null)
                    foreach (Collider a in loot.GetComponents<Collider>())
                    {
                        if (a.isTrigger) continue;
                        foreach (Collider b in GetComponentsInChildren<Collider>()) Physics.IgnoreCollision(a, b);
                    }
                weapons.ClearInventory();
                weapons.Unequip();
            }
            if (movement != null) { movement.ResetMotion(); movement.enabled = false; }
            GetComponent<PlayerMelee>()?.CancelSwing();
            if (TryGetComponent<PlayerInputReader>(out var input)) input.enabled = false;
            if (TryGetComponent<PlayerLook>(out var look)) look.enabled = false;
        }
        void CompleteRespawn()
        {
            Transform destination = spawnPoint;
            if (map != null)
            {
                var controller = GetComponent<CharacterController>();
                destination = map.Choose(deathPosition, previousSpawn, transform,
                    controller != null ? controller.height : 1.8f, controller != null ? controller.radius : 0.4f);
                // Wait for a safe, different point instead of placing the body inside another player.
                if (destination == null) return;
            }
            waiting = false;
            var body = GetComponent<CharacterController>();
            bool wasEnabled = body != null && body.enabled;
            if (body != null) body.enabled = false;
            if (destination != null)
            {
                transform.SetPositionAndRotation(destination.position, destination.rotation);
                GetComponent<PlayerLook>()?.ResetLook(destination.rotation);
                previousSpawn = destination;
            }
            movement?.ResetMotion();
            if (body != null) body.enabled = wasEnabled;
            GetComponent<PlayerVoidFall>()?.ResetFall();
            if (health != null) { health.RespawnAtFullHealth(); health.BeginSpawnProtection(Protection); }
            weapons?.Equip();
            if (movement != null) movement.enabled = true;
            if (TryGetComponent<PlayerInputReader>(out var input)) input.enabled = true;
            if (TryGetComponent<PlayerLook>(out var look)) look.enabled = true;
            Respawned?.Invoke();
        }
    }
}
