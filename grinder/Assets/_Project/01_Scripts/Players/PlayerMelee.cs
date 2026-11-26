using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Players
{
    // One swing, one confirmation per target. Balance values remain configurable in MeleeTuning.
    public sealed class PlayerMelee : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerLook look;
        [SerializeField] private DamageResolverHost damage;
        [SerializeField] private HealthComponent health;
        [SerializeField] private LocalCombatAuthority authority;
        [SerializeField] private float damageAmount = 20f;
        [SerializeField] private float range = 2.2f;
        [SerializeField] private float radius = 0.45f;
        [SerializeField] private float cooldown = 0.55f;

        [SerializeField] private MeleeTuning tuning;
        public event System.Action Swung;
        float Reach => tuning != null ? tuning.Range : range;
        float Radius => tuning != null ? tuning.Radius : radius;
        float Interval => tuning != null ? tuning.Cooldown : cooldown;
        float cooldownRemaining;
        ulong nextConfirmation = 1;
        bool swingPending;
        bool impactApplied;
        public bool IsSwinging => cooldownRemaining > 0f;
        public float SwingProgress => IsSwinging ? 1f - cooldownRemaining / Mathf.Max(.01f, Interval) : 0f;
        public event System.Action<bool> Impact;

        public float DamageAmount => tuning != null ? tuning.Damage : damageAmount;
        public float CooldownRemaining => cooldownRemaining;

        public void SetTuning(MeleeTuning value)
        {
            CancelSwing();
            tuning = value;
        }

        void Awake()
        {
            if (input == null)
            {
                input = GetComponent<PlayerInputReader>();
            }

            if (look == null)
            {
                look = GetComponent<PlayerLook>();
            }

            if (damage == null)
            {
                damage = GetComponent<DamageResolverHost>();
            }

            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            if (authority == null)
            {
                authority = GetComponent<LocalCombatAuthority>();
            }
        }

        void FixedUpdate()
        {
            TickSwing(Time.fixedDeltaTime);

            if (input == null || input.ConsumeMelee() == false)
            {
                return;
            }

            TrySwing();
        }

        public bool TrySwing()
        {
            if (!BeginSwing()) return false;
            swingPending = true;
            impactApplied = false;
            return true;
        }

        internal void TickSwing(float dt)
        {
            if (health != null && !health.IsAlive) { CancelSwing(); return; }
            float before = Interval - cooldownRemaining;
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - dt);
            float after = Interval - cooldownRemaining;
            float windup = tuning != null ? tuning.Windup : .12f;
            float window = tuning != null ? tuning.ImpactWindow : .1f;
            if (swingPending && !impactApplied && after >= windup && before <= windup + window)
            {
                impactApplied = Strike();
                if (impactApplied) Impact?.Invoke(true);
            }
            if (swingPending && after >= windup + window)
            {
                if (!impactApplied) Impact?.Invoke(false);
                swingPending = false;
            }
        }

        public void CancelSwing() { swingPending = false; cooldownRemaining = 0f; }

        bool Strike()
        {
            Transform view = look != null && look.View != null ? look.View.transform : transform;
            Vector3 origin = view.position;
            Vector3 aim = view.forward.normalized;
            Physics.SyncTransforms();
            Collider[] found = Physics.OverlapCapsule(origin, origin + aim * Mathf.Max(0f, Reach - Radius),
                Radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            HealthComponent closest = null;
            Vector3 hitPoint = default;
            float nearest = float.MaxValue;
            foreach (Collider collider in found)
            {
                var candidate = collider.GetComponentInParent<HealthComponent>();
                if (candidate == null || candidate == health || !candidate.IsAlive) continue;
                Vector3 point = collider.ClosestPoint(origin);
                Vector3 delta = point - origin;
                float distance = delta.magnitude;
                if (distance > Reach || distance >= nearest || Vector3.Dot(aim, delta) < 0f) continue;
                bool blocked = false;
                foreach (var hit in Physics.RaycastAll(origin, delta.normalized, distance,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.transform.IsChildOf(transform) || hit.collider.GetComponentInParent<HealthComponent>() == candidate) continue;
                    blocked = true;
                    break;
                }
                if (blocked) continue;
                closest = candidate;
                nearest = distance;
                hitPoint = point;
            }
            bool applied = closest != null && ResolveStrike(closest, hitPoint);
            if (applied) GetComponent<PlayerImpactFeedback>()?.Present(hitPoint, true);
            return applied;
        }

        bool BeginSwing()
        {
            if (damage == null) damage = GetComponent<DamageResolverHost>();
            if (health == null) health = GetComponent<HealthComponent>();
            if (authority == null) authority = GetComponent<LocalCombatAuthority>();
            if (damage == null || cooldownRemaining > 0f || (health != null && !health.IsAlive) ||
                (authority != null && !authority.IsAuthoritative)) return false;
            cooldownRemaining = Interval;
            health?.BeginSpawnProtection(0f);
            Swung?.Invoke();
            return true;
        }

        internal bool ApplyStrike(HealthComponent target)
        {
            return target != null && BeginSwing() && ResolveStrike(target, target.transform.position);
        }

        bool ResolveStrike(HealthComponent target, Vector3 point)
        {
            damage.Resolver.AttackerPowerUps = GetComponent<IDamageModifiers>();
            damage.Resolver.TargetPowerUps = target.GetComponent<IDamageModifiers>();
            return damage.Resolver.Apply(new DamageInfo(health != null ? health.StableId : 0,
                health != null ? health.TeamId : 0, target, DamageAmount, DamageType.Melee,
                point, nextConfirmation++, authority == null || authority.IsAuthoritative, true, "melee"));
        }

        internal void Configure(float amount, float reach, float strikeCooldown)
        {
            tuning = null;
            damageAmount = amount;
            range = reach;
            cooldown = strikeCooldown;
            cooldownRemaining = 0f;
        }
    }
}
