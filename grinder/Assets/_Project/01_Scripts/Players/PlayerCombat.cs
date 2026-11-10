using System.Collections.Generic;
using Meatgrinder.Combat;
using Meatgrinder.Weapons;
using UnityEngine;

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
                weapons.Select(slot - 1);
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

            damage.Resolver.AttackerPowerUps = GetComponent<IDamageModifiers>();

            ShotTicket ticket = weapons.TryBeginShot();
            if (ticket.Accepted == false)
            {
                return;
            }

            Vector3 aim = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
            Vector3 muzzle = origin + (aim * muzzleClearance);
            bool authoritative = authority == null || authority.IsAuthoritative;
            int attackerId = health != null ? health.StableId : 0;
            int team = health != null ? health.TeamId : 0;
            if (feedback != null)
            {
                feedback.Kick(ticket.Weapon.RecoilPitch);
            }

            if (ticket.Weapon.Delivery == WeaponDelivery.Projectile)
            {
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
                HasAuthority = authoritative
            };
            projectiles.Spawn(shot);
        }
    }
}
