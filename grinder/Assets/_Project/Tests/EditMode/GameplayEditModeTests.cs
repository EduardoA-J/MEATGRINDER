using System.Collections.Generic;
using Meatgrinder.Combat;
using Meatgrinder.Movement;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEngine;

namespace Meatgrinder.Tests
{
    public sealed class GameplayEditModeTests
    {
        readonly List<Object> trash = new List<Object>();

        [TearDown]
        public void Cleanup()
        {
            for (int i = 0; i < trash.Count; i++)
            {
                if (trash[i] != null)
                {
                    Object.DestroyImmediate(trash[i]);
                }
            }

            trash.Clear();
        }

        [Test]
        public void GrantedRocketKeepsAmmoSeparateFromTheShotgun()
        {
            WeaponDefinition shotgun = Weapon("shotgun", WeaponDelivery.Hitscan, 6, 24);
            WeaponDefinition rocket = Weapon("rocket_launcher", WeaponDelivery.Projectile, 4, 12);
            WeaponController weapons = Keep(new GameObject("weapons")).AddComponent<WeaponController>();
            weapons.LoadForTests(new[] { shotgun });
            int shotgunLoaded = weapons.Loaded;
            int shotgunReserve = weapons.Reserve;

            int index = weapons.TryGrant(rocket, 5);
            Assert.AreEqual(1, index);
            Assert.AreEqual(2, weapons.SlotCount);
            Assert.IsTrue(weapons.Select(index));
            Assert.AreEqual(4, weapons.Loaded);
            Assert.AreEqual(5, weapons.Reserve);
            Assert.IsTrue(weapons.Select(0));
            Assert.AreEqual(shotgunLoaded, weapons.Loaded);
            Assert.AreEqual(shotgunReserve, weapons.Reserve);
        }

        [Test]
        public void QuadDamageDoublesTheConfirmedHit()
        {
            HealthComponent health = Health();
            PowerUpManager powers = health.gameObject.AddComponent<PowerUpManager>();
            PickupDefinition quad = Keep(ScriptableObject.CreateInstance<PickupDefinition>());
            quad.Configure(PickupType.PowerUp, 0f, PowerUpType.QuadDamage, 30f, 2f, null, 0);
            powers.Grant(quad, health);
            DamageResolver resolver = new DamageResolver();
            resolver.AttackerPowerUps = powers;

            Assert.IsTrue(resolver.Apply(Hit(health, 10f, 1, DamageType.Hitscan)));
            Assert.AreEqual(80f, health.Current, 0.001f);
        }

        [Test]
        public void BerserkScalesMeleeAndLeavesHitscanAlone()
        {
            HealthComponent attacker = Health();
            HealthComponent meleeTarget = Health();
            HealthComponent shotTarget = Health();
            PowerUpManager powers = attacker.gameObject.AddComponent<PowerUpManager>();
            PickupDefinition berserk = Keep(ScriptableObject.CreateInstance<PickupDefinition>());
            berserk.Configure(PickupType.PowerUp, 0f, PowerUpType.Berserk, 30f, 2f, null, 0);
            powers.Grant(berserk, attacker);
            attacker.gameObject.AddComponent<DamageResolverHost>();
            PlayerMelee melee = attacker.gameObject.AddComponent<PlayerMelee>();
            melee.Configure(20f, 2f, 0f);

            Assert.IsTrue(melee.ApplyStrike(meleeTarget));
            Assert.AreEqual(60f, meleeTarget.Current, 0.001f);

            DamageResolver resolver = new DamageResolver();
            resolver.AttackerPowerUps = powers;
            Assert.IsTrue(resolver.Apply(Hit(shotTarget, 10f, 4, DamageType.Hitscan)));
            Assert.AreEqual(90f, shotTarget.Current, 0.001f);
        }

        [Test]
        public void SpawnProtectionExpires()
        {
            HealthComponent health = Health();
            health.BeginSpawnProtection(1f);
            DamageResolver resolver = new DamageResolver();
            Assert.IsFalse(resolver.Apply(Hit(health, 25f, 1, DamageType.Hitscan)));
            Assert.AreEqual(100f, health.Current, 0.001f);
            health.TickSpawnProtection(1f);
            Assert.IsTrue(resolver.Apply(Hit(health, 25f, 2, DamageType.Hitscan)));
            Assert.AreEqual(75f, health.Current, 0.001f);
        }

        [Test]
        public void RespawnRestoresHealthAfterThePendingDelay()
        {
            GameObject body = Keep(new GameObject("life"));
            HealthComponent health = body.AddComponent<HealthComponent>();
            health.UseProfile(null);
            PlayerLife life = body.AddComponent<PlayerLife>();
            life.Configure(0.25f, 0f);
            DamageResolver resolver = new DamageResolver();
            Assert.IsTrue(resolver.Apply(Hit(health, 500f, 1, DamageType.Explosion)));
            Assert.IsFalse(health.IsAlive);
            Assert.IsTrue(life.IsWaitingToRespawn);
            life.Tick(0.25f);
            Assert.IsFalse(life.IsWaitingToRespawn);
            Assert.IsTrue(health.IsAlive);
            Assert.AreEqual(health.Max, health.Current, 0.001f);
        }

        [Test]
        public void PickupHealsAndGrantsTheRocket()
        {
            GameObject body = Keep(new GameObject("picker"));
            HealthComponent health = body.AddComponent<HealthComponent>();
            health.UseProfile(null);
            WeaponController weapons = body.AddComponent<WeaponController>();
            WeaponDefinition shotgun = Weapon("shotgun", WeaponDelivery.Hitscan, 6, 12);
            WeaponDefinition rocket = Weapon("rocket_launcher", WeaponDelivery.Projectile, 4, 8);
            weapons.LoadForTests(new[] { shotgun });
            new DamageResolver().Apply(Hit(health, 60f, 1, DamageType.Hitscan));

            PickupController healthPickup = Keep(new GameObject("health-pickup")).AddComponent<PickupController>();
            PickupDefinition healthDef = Keep(ScriptableObject.CreateInstance<PickupDefinition>());
            healthDef.Configure(PickupType.Health, 25f, PowerUpType.QuadDamage, 0f, 0f, null, 0);
            healthPickup.SetDefinition(healthDef);
            Assert.IsTrue(healthPickup.Collect(health));
            Assert.AreEqual(65f, health.Current, 0.001f);
            Assert.IsFalse(healthPickup.IsAvailable);

            PickupController weaponPickup = Keep(new GameObject("weapon-pickup")).AddComponent<PickupController>();
            PickupDefinition weaponDef = Keep(ScriptableObject.CreateInstance<PickupDefinition>());
            weaponDef.Configure(PickupType.Weapon, 0f, PowerUpType.QuadDamage, 0f, 0f, rocket, 3);
            weaponPickup.SetDefinition(weaponDef);
            Assert.IsTrue(weaponPickup.Collect(health));
            Assert.IsTrue(weapons.Select(1));
            Assert.AreEqual(4, weapons.Loaded);
            Assert.AreEqual(3, weapons.Reserve);
        }

        [Test]
        public void HasteRaisesGroundSpeed()
        {
            MovementTuning tuning = Keep(ScriptableObject.CreateInstance<MovementTuning>());
            MovementState plain = MovementState.Standing();
            MovementState fast = MovementState.Standing();
            plain.IsGrounded = true;
            fast.IsGrounded = true;
            MovementIntent intent = new MovementIntent { Move = new Vector2(0f, 1f), Jump = false };
            MovementSimulator.Tick(ref plain, intent, tuning, Vector3.forward, Vector3.right, 0.2f, true, 1f);
            MovementSimulator.Tick(ref fast, intent, tuning, Vector3.forward, Vector3.right, 0.2f, true, 2f);
            Assert.Greater(MovementSimulator.HorizontalSpeed(fast.Velocity), MovementSimulator.HorizontalSpeed(plain.Velocity));
        }

        HealthComponent Health()
        {
            HealthComponent health = Keep(new GameObject("body")).AddComponent<HealthComponent>();
            health.UseProfile(null);
            return health;
        }

        WeaponDefinition Weapon(string id, WeaponDelivery delivery, int loaded, int reserve)
        {
            WeaponDefinition weapon = Keep(ScriptableObject.CreateInstance<WeaponDefinition>());
            weapon.SetIdentity(id, id);
            weapon.SetDelivery(delivery);
            weapon.SetAmmo(loaded, reserve, loaded, 1);
            weapon.SetDrawDuration(0f);
            return weapon;
        }

        DamageInfo Hit(HealthComponent health, float amount, ulong confirmation, DamageType type)
        {
            return new DamageInfo(9000, 1, health, amount, type, health.transform.position, confirmation, true, true, "test");
        }

        T Keep<T>(T obj) where T : Object
        {
            trash.Add(obj);
            return obj;
        }
    }
}
