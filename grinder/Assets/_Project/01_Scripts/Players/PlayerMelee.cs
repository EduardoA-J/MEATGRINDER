using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Players
{
    // One swing, one confirmation per target. Numbers are PENDIENTE: the GDD does not fix them.
    public sealed class PlayerMelee : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerLook look;
        [SerializeField] private DamageResolverHost damage;
        [SerializeField] private HealthComponent health;
        [SerializeField] private LocalCombatAuthority authority;
        [SerializeField] private float damageAmount = 20f; // PENDIENTE
        [SerializeField] private float range = 2.2f; // PENDIENTE
        [SerializeField] private float radius = 0.45f; // PENDIENTE
        [SerializeField] private float cooldown = 0.55f; // PENDIENTE

        float cooldownRemaining;
        ulong nextConfirmation = 1;

        public float DamageAmount => damageAmount;
        public float CooldownRemaining => cooldownRemaining;

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
            if (cooldownRemaining > 0f)
            {
                cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.fixedDeltaTime);
            }

            if (input == null || input.ConsumeMelee() == false)
            {
                return;
            }

            TrySwing();
        }

        public bool TrySwing()
        {
            if (health != null && health.IsAlive == false)
            {
                return false;
            }

            Transform view = look != null && look.View != null ? look.View.transform : transform;
            Vector3 origin = view.position;
            Vector3 aim = view.forward.sqrMagnitude > 0.0001f ? view.forward.normalized : Vector3.forward;
            Collider[] found = Physics.OverlapSphere(origin + (aim * (range * 0.5f)), Mathf.Max(radius, range * 0.5f));
            HealthComponent closest = null;
            float closestDistance = float.MaxValue;
            for (int i = 0; i < found.Length; i++)
            {
                HealthComponent candidate = found[i].GetComponentInParent<HealthComponent>();
                if (candidate == null || candidate == health || candidate.IsAlive == false)
                {
                    continue;
                }

                float distance = Vector3.Distance(origin, candidate.transform.position);
                if (distance > range || distance >= closestDistance)
                {
                    continue;
                }

                closest = candidate;
                closestDistance = distance;
            }

            return closest != null && ApplyStrike(closest);
        }

        internal bool ApplyStrike(HealthComponent target)
        {
            if (damage == null)
            {
                damage = GetComponent<DamageResolverHost>();
            }

            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            if (target == null || damage == null || cooldownRemaining > 0f)
            {
                return false;
            }

            if (health != null && health.IsAlive == false)
            {
                return false;
            }

            bool authoritative = authority == null || authority.IsAuthoritative;
            damage.Resolver.AttackerPowerUps = GetComponent<IDamageModifiers>();
            damage.Resolver.TargetPowerUps = target.GetComponent<IDamageModifiers>();
            ulong confirmation = nextConfirmation++;
            bool applied = damage.Resolver.Apply(new DamageInfo(
                health != null ? health.StableId : 0,
                health != null ? health.TeamId : 0,
                target,
                damageAmount,
                DamageType.Melee,
                target.transform.position,
                confirmation,
                authoritative,
                true,
                "melee"));
            cooldownRemaining = cooldown;
            return applied;
        }

        internal void Configure(float amount, float reach, float strikeCooldown)
        {
            damageAmount = amount;
            range = reach;
            cooldown = strikeCooldown;
            cooldownRemaining = 0f;
        }
    }
}
