using System.Collections.Generic;
using Meatgrinder.Combat;
using Meatgrinder.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Players
{
    // Local shot path. Host migration: keep the input latch here, send origin, direction, weapon id and a client tick,
    // and run TryBeginShot, HitscanQuery and ExplosionQuery only where ICombatAuthority is true.
    [DefaultExecutionOrder(100)]
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private PlayerLook look;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private DamageResolverHost damage;
        [SerializeField] private LocalCombatAuthority authority;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private LocalProjectileSpawner projectiles;
        [SerializeField] private Transform projectileMuzzle;
        [SerializeField] private HealthComponent health;
        [SerializeField] private float muzzleClearance = 0.6f; // PENDIENTE

        readonly List<HitscanHit> hits = new List<HitscanHit>(16);
        int seed = 1;

        void Awake()
        {
            if (projectiles == null)
            {
                projectiles = GetComponent<LocalProjectileSpawner>();
                if (projectiles == null)
                {
                    projectiles = gameObject.AddComponent<LocalProjectileSpawner>();
                }
            }

            if (GetComponent<PlayerMelee>() == null)
            {
                gameObject.AddComponent<PlayerMelee>();
            }

            if (GetComponent<PlayerLife>() == null)
            {
                gameObject.AddComponent<PlayerLife>();
            }

            if (GetComponent<Meatgrinder.Pickups.PowerUpManager>() == null)
            {
                gameObject.AddComponent<Meatgrinder.Pickups.PowerUpManager>();
            }

            if (GetComponent<PlayerImpulseReceiver>() == null)
            {
                gameObject.AddComponent<PlayerImpulseReceiver>();
            }

            if (GetComponent<PlayerVoidFall>() == null && SceneManager.GetActiveScene().name.StartsWith("M0"))
            {
                gameObject.AddComponent<PlayerVoidFall>();
            }
        }

        void FixedUpdate()
        {
            if (weapons == null)
            {
                return;
            }

            weapons.Tick(Time.fixedDeltaTime);
            if (input == null)
            {
                return;
            }

            int slot = input.ConsumeWeaponSlot();
            if (slot > 0)
            {
                weapons.SelectWeapon(slot == 1 ? "shotgun" : "rocket_launcher");
            }

            if (input.ConsumeReload())
            {
                weapons.TryBeginReload();
            }

            if (input.ConsumeAttack() == false || look == null || look.View == null)
            {
                return;
            }

            Transform view = look.View.transform;
            FireFrom(view.position, view.forward, seed++);
        }

        public void FireFrom(Vector3 origin, Vector3 direction, int shotSeed)
        {
            if (weapons == null || damage == null || (health != null && health.IsAlive == false))
            {
                return;
            }

            if (authority != null && !authority.IsAuthoritative) return;
            if (GetComponent<PlayerMelee>()?.IsSwinging == true) return;
            if (weapons.ActiveDefinition == null)
            {
                GetComponent<PlayerMelee>()?.TrySwing();
                return;
            }
            health?.BeginSpawnProtection(0f);
            damage.Resolver.AttackerPowerUps = GetComponent<IDamageModifiers>();

            ShotTicket ticket = weapons.TryBeginShot();
            if (ticket.Accepted == false)
            {
                return;
            }

            Vector3 aim = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
            Vector3 muzzle = origin;
            bool authoritative = authority == null || authority.IsAuthoritative;
            int attackerId = health != null ? health.StableId : 0;
            int team = health != null ? health.TeamId : 0;
            if (feedback != null)
            {
                feedback.Kick(ticket.Weapon.RecoilPitch);
            }

            if (ticket.Weapon.Delivery == WeaponDelivery.Projectile)
            {
                ResolveProjectileOrigin(ticket.Weapon, ref muzzle, ref aim);
                LaunchRocket(ticket, muzzle, aim, attackerId, team, authoritative);
                return;
            }

            FireHitscan(ticket, muzzle, aim, shotSeed, attackerId, team, authoritative);
        }

        internal void BindForTests(
            WeaponController weaponController,
            DamageResolverHost resolverHost,
            PlayerImpactFeedback impact,
            LocalProjectileSpawner spawner,
            HealthComponent body,
            LocalCombatAuthority gate)
        {
            weapons = weaponController;
            damage = resolverHost;
            feedback = impact;
            projectiles = spawner;
            health = body;
            authority = gate;
        }

        void FireHitscan(
            ShotTicket ticket,
            Vector3 muzzle,
            Vector3 aim,
            int shotSeed,
            int attackerId,
            int team,
            bool authoritative)
        {
            HitscanQuery.Collect(ticket.Weapon, muzzle, aim, shotSeed, transform, hits);
            for (int i = 0; i < hits.Count; i++)
            {
                HitscanHit hit = hits[i];
                if (hit.Target == null)
                {
                    if (feedback != null)
                    {
                        feedback.Present(hit.Point, false);
                    }

                    continue;
                }

                damage.Resolver.TargetPowerUps = hit.Target is HealthComponent body
                    ? body.GetComponent<IDamageModifiers>()
                    : null;
                bool applied = damage.Resolver.Apply(new DamageInfo(
                    attackerId,
                    team,
                    hit.Target,
                    hit.Damage,
                    DamageType.Hitscan,
                    hit.Point,
                    ticket.ConfirmationBase + (ulong)hit.Pellet,
                    authoritative,
                    true,
                    ticket.Weapon.WeaponId));
                if (feedback != null)
                {
                    feedback.Present(hit.Point, applied);
                }
            }
        }

        void ResolveProjectileOrigin(WeaponDefinition weapon, ref Vector3 origin, ref Vector3 direction)
        {
            if (projectileMuzzle == null) return;
            float distance = Mathf.Max(1f, weapon.MaxRange);
            Vector3 target = origin + direction * distance;
            foreach (var hit in Physics.RaycastAll(origin, direction, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.transform.IsChildOf(transform) || hit.distance >= distance) continue;
                distance = hit.distance; target = hit.point;
            }
            Vector3 offset = projectileMuzzle.position - origin;
            float safeDistance = offset.magnitude;
            if (safeDistance < .001f) return;
            Vector3 travel = offset / safeDistance;
            // Keep a muzzle poking into a wall from spawning a rocket on its far side.
            foreach (var hit in Physics.SphereCastAll(origin, weapon.ProjectileRadius, travel, safeDistance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.transform.IsChildOf(transform)) continue;
                safeDistance = Mathf.Min(safeDistance, Mathf.Max(0, hit.distance - .01f));
            }
            origin += travel * safeDistance;
            Vector3 aim = target - origin;
            if (aim.sqrMagnitude > .0001f) direction = aim.normalized;
        }

        void LaunchRocket(
            ShotTicket ticket,
            Vector3 muzzle,
            Vector3 aim,
            int attackerId,
            int team,
            bool authoritative)
        {
            if (projectiles == null)
            {
                return;
            }

            ProjectileLaunch shot = new ProjectileLaunch
            {
                Origin = muzzle,
                Direction = aim,
                Weapon = ticket.Weapon,
                Resolver = damage.Resolver,
                Presenter = feedback,
                Owner = transform,
                AttackerId = attackerId,
                AttackerTeam = team,
                ConfirmationBase = ticket.ConfirmationBase,
                HasAuthority = authoritative,
                AllowSelfDamage = true
            };
            projectiles.Spawn(shot);
        }
    }
}
