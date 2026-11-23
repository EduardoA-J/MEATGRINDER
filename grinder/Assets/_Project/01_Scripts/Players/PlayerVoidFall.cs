using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class PlayerVoidFall : MonoBehaviour
    {
        [SerializeField] private LifeTuning tuning;
        [SerializeField] private LocalSpawnPoints map;
        HealthComponent health;
        DamageResolverHost resolver;
        LocalCombatAuthority authority;
        float elapsed;
        float healthAtEntry;
        ulong confirmation;
        public bool IsFalling { get; private set; }
        public float Progress => IsFalling ? Mathf.Clamp01(elapsed / (tuning != null ? tuning.MaximumVoidDuration : 5f)) : 0f;

        void Awake()
        {
            health = GetComponent<HealthComponent>();
            resolver = GetComponent<DamageResolverHost>();
            authority = GetComponent<LocalCombatAuthority>();
            if (map == null)
            {
                map = FindFirstObjectByType<LocalSpawnPoints>();
                if (map == null && gameObject.name == "LocalPlayer")
                {
                    GameObject root = new GameObject("RuntimeSpawnPoints");
                    map = root.AddComponent<LocalSpawnPoints>();
                    map.SetPoints(FindSpawnPoints());
                }
            }
        }

        static Transform[] FindSpawnPoints()
        {
            var list = new System.Collections.Generic.List<Transform>();
            foreach (var marker in Object.FindObjectsByType<Transform>(FindObjectsSortMode.None))
                if (marker.name.StartsWith("Spawn_Human_")) list.Add(marker);
            return list.ToArray();
        }

        void FixedUpdate() => Tick(Time.fixedDeltaTime);

        public void ResetFall()
        {
            IsFalling = false;
            elapsed = 0f;
            if (health != null) health.RecoverySuppressed = false;
        }

        internal void Tick(float deltaTime)
        {
            if (map == null || tuning == null || health == null || resolver == null || !health.IsAlive ||
                (authority != null && !authority.IsAuthoritative)) return;
            if (!IsFalling)
            {
                if (transform.position.y >= map.VoidHeight) return;
                IsFalling = true;
                healthAtEntry = health.Current;
                health.RecoverySuppressed = true;
            }
            float before = Mathf.Max(0f, elapsed - tuning.VoidGrace);
            elapsed += deltaTime;
            float after = Mathf.Max(0f, elapsed - tuning.VoidGrace);
            float damage = healthAtEntry * tuning.VoidHealthFractionPerSecond * (after - before);
            if (elapsed >= tuning.MaximumVoidDuration) damage = health.Current;
            if (damage <= 0f) return;
            resolver.Resolver.Apply(new DamageInfo(0, 0, health, damage, DamageType.Void,
                transform.position, ++confirmation, true, false, "void"));
        }
    }
}
