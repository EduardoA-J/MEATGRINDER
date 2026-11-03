using System.Collections.Generic;
using Meatgrinder.Combat;
using Meatgrinder.Movement;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEngine;

namespace Meatgrinder.Tests
{
    public sealed class CombatEditModeTests
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
        public void SameConfirmationDoesNotApplyTwice()
        {
            HealthComponent health = CreateHealth(new Vector3(500f, 400f, 0f), 100f, 4, 2);
            DamageResolver resolver = new DamageResolver();
            DamageInfo first = Hit(1, 1, health, 30f, 9, true);
            DamageInfo second = Hit(1, 1, health, 30f, 9, true);
            Assert.IsTrue(resolver.Apply(first));
            Assert.IsFalse(resolver.Apply(second));
            Assert.AreEqual(70f, health.Current, 0.001f);
        }

        [Test]
        public void MissingAuthorityDoesNotChangeHealth()
        {
            HealthComponent health = CreateHealth(new Vector3(500f, 402f, 0f), 100f, 5, 2);
            DamageResolver resolver = new DamageResolver();
            Assert.IsFalse(resolver.Apply(Hit(1, 1, health, 40f, 3, false)));
            Assert.AreEqual(100f, health.Current, 0.001f);
        }

        [Test]
        public void DamageTypesCoverTheFourChannels()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(DamageType)).Length);
            Assert.AreNotEqual(DamageType.Hitscan, DamageType.Projectile);
            Assert.AreNotEqual(DamageType.Melee, DamageType.Explosion);
        }

        [Test]
        public void PartialDamageStopsAtZeroAndRaisesOneDeath()
        {
            HealthComponent health = CreateHealth(new Vector3(500f, 404f, 0f), 50f, 6, 2);
            int deaths = 0;
            int hits = 0;
            health.Died += () => deaths++;
            health.Damaged += amount => hits++;
            DamageResolver resolver = new DamageResolver();
            Assert.IsTrue(resolver.Apply(Hit(1, 1, health, 20f, 1, true)));
            Assert.AreEqual(30f, health.Current, 0.001f);
            Assert.IsTrue(resolver.Apply(Hit(1, 1, health, 80f, 2, true)));
            Assert.AreEqual(0f, health.Current, 0.001f);
            Assert.AreEqual(1, deaths);
            Assert.AreEqual(2, hits);
            Assert.IsFalse(resolver.Apply(Hit(1, 1, health, 10f, 3, true)));
            Assert.AreEqual(1, deaths);
        }

        [Test]
        public void FriendlyFireBlocksTheSameTeam()
        {
            HealthComponent health = CreateHealth(new Vector3(500f, 406f, 0f), 80f, 8, 1);
            DamageResolver resolver = new DamageResolver();
            Assert.IsFalse(resolver.Apply(Hit(2, 1, health, 15f, 1, true)));
            Assert.AreEqual(80f, health.Current, 0.001f);
            resolver.FriendlyFire = true;
            Assert.IsTrue(resolver.Apply(Hit(2, 1, health, 15f, 2, true)));
            Assert.AreEqual(65f, health.Current, 0.001f);
        }

        [Test]
        public void ShooterIsExcludedFromTheBlast()
        {
            HealthComponent shooter = CreateHealth(new Vector3(520f, 400f, 0f), 100f, 11, 1);
            HealthComponent near = CreateHealth(new Vector3(521.6f, 400f, 0f), 100f, 12, 2);
            HealthComponent far = CreateHealth(new Vector3(520f, 400f, -3.2f), 100f, 13, 2);
            HealthComponent covered = CreateHealth(new Vector3(520f, 400f, 3.2f), 100f, 14, 2);
            GameObject wall = Box(new Vector3(520f, 400f, 2.1f), new Vector3(6f, 3f, 0.4f));
            WeaponDefinition weapon = Weapon();
            weapon.SetBlast(4f, 80f, 0.25f);
            Physics.SyncTransforms();
            int applied = ExplosionQuery.Apply(new ProjectileLaunch
            {
                Origin = new Vector3(520f, 400f, 0f),
                Weapon = weapon,
                Resolver = new DamageResolver(),
                AttackerId = shooter.StableId,
                AttackerTeam = 1,
                ConfirmationBase = 40,
                HasAuthority = true
            });
            Assert.GreaterOrEqual(applied, 2);
            Assert.AreEqual(100f, shooter.Current, 0.001f);
            Assert.Less(near.Current, far.Current);
            Assert.Less(far.Current, 100f);
            Assert.AreEqual(100f, covered.Current, 0.001f);
            Assert.IsNotNull(wall);
        }

        [Test]
        public void CadenceReloadAndEmptyMagazineFollowTheAsset()
        {
            WeaponDefinition shotgun = Weapon();
            shotgun.SetAmmo(2, 5, 4, 1);
            shotgun.SetFireInterval(0.5f);
            shotgun.SetReload(1f);
            WeaponDefinition rocket = Weapon();
            rocket.SetIdentity("rocket", "Lanzacohetes");
            rocket.SetDelivery(WeaponDelivery.Projectile);
            rocket.SetAmmo(3, 1, 3, 1);
            WeaponController weapons = Controller(shotgun, rocket);
            Assert.IsTrue(weapons.TryBeginShot().Accepted);
            Assert.AreEqual(1, weapons.Loaded);
            Assert.IsFalse(weapons.TryBeginShot().Accepted);
            Assert.AreEqual(ShotRejectReason.CoolingDown, weapons.TryBeginShot().Reason);
            weapons.Tick(0.5f);
            Assert.IsTrue(weapons.TryBeginShot().Accepted);
            Assert.AreEqual(0, weapons.Loaded);
            weapons.Tick(0.5f);
            ShotTicket empty = weapons.TryBeginShot();
            Assert.IsFalse(empty.Accepted);
            Assert.AreEqual(ShotRejectReason.Empty, empty.Reason);
            Assert.IsTrue(weapons.IsReloading);
            Assert.IsTrue(weapons.Select(1));
            Assert.AreEqual(3, weapons.Loaded);
            weapons.Tick(1f);
            Assert.IsTrue(weapons.Select(0));
            Assert.AreEqual(4, weapons.Loaded);
            Assert.AreEqual(1, weapons.Reserve);
        }

        [Test]
        public void SwitchingBackDoesNotSpendAnotherRound()
        {
            WeaponDefinition first = Weapon();
            first.SetAmmo(4, 0, 4, 1);
            first.SetFireInterval(0f);
            WeaponDefinition second = Weapon();
            second.SetAmmo(2, 0, 2, 1);
            WeaponController weapons = Controller(first, second);
            Assert.IsTrue(weapons.TryBeginShot().Accepted);
            Assert.IsTrue(weapons.Select(1));
            Assert.AreEqual(2, weapons.Loaded);
            Assert.IsTrue(weapons.Select(0));
            Assert.AreEqual(3, weapons.Loaded);
        }

        [Test]
        public void EmptyWeaponDoesNotDamageTheTarget()
        {
            WeaponDefinition shotgun = Weapon();
            shotgun.SetAmmo(0, 0, 4, 1);
            shotgun.SetSpread(0f, 1);
            shotgun.SetFalloff(50f, 60f, 1f, 25f);
            shotgun.SetMaxRange(30f);
            HealthComponent target = CreateHealth(new Vector3(500f, 430f, 4f), 80f, 21, 2);
            PlayerCombat combat = Shooter(shotgun, new Vector3(500f, 430f, 0f));
            Physics.SyncTransforms();
            combat.FireFrom(new Vector3(500f, 430f, 0f), Vector3.forward, 1);
            Assert.AreEqual(80f, target.Current, 0.001f);
        }

        [Test]
        public void SpreadLeavesTheCenterAndFalloffDropsWithDistance()
        {
            Vector3 center = HitscanSpread.Direction(Vector3.forward, 0, 8f, 5);
            Vector3 outer = HitscanSpread.Direction(Vector3.forward, 1, 8f, 5);
            Vector3 tight = HitscanSpread.Direction(Vector3.forward, 1, 0f, 5);
            Assert.Less(Vector3.Angle(Vector3.forward, center), 0.001f);
            Assert.Greater(Vector3.Angle(center, outer), 0.1f);
            Assert.Less(Vector3.Angle(Vector3.forward, tight), 0.001f);

            WeaponDefinition weapon = Weapon();
            weapon.SetFalloff(4f, 20f, 0.25f, 10f);
            weapon.SetSpread(0f, 1);
            weapon.SetMaxRange(40f);
            CreateHealth(new Vector3(540f, 400f, 2f), 100f, 31, 2);
            CreateHealth(new Vector3(560f, 400f, 30f), 100f, 33, 2);
            List<HitscanHit> nearHits = new List<HitscanHit>();
            List<HitscanHit> farHits = new List<HitscanHit>();
            HitscanQuery.Collect(weapon, new Vector3(540f, 400f, 0f), Vector3.forward, 1, null, nearHits);
            HitscanQuery.Collect(weapon, new Vector3(560f, 400f, 0f), Vector3.forward, 1, null, farHits);
            Assert.AreEqual(1, nearHits.Count);
            Assert.AreEqual(1, farHits.Count);
            Assert.Greater(nearHits[0].Damage, farHits[0].Damage * 2f);
        }

        [Test]
        public void WallStopsTheShotgun()
        {
            WeaponDefinition weapon = Weapon();
            weapon.SetSpread(0f, 4);
            weapon.SetFalloff(30f, 40f, 1f, 10f);
            weapon.SetMaxRange(30f);
            Box(new Vector3(580f, 400f, 3f), new Vector3(3f, 3f, 0.5f));
            HealthComponent hidden = CreateHealth(new Vector3(580f, 400f, 6f), 70f, 41, 2);
            List<HitscanHit> hits = new List<HitscanHit>();
            HitscanQuery.Collect(weapon, new Vector3(580f, 400f, 0f), Vector3.forward, 2, null, hits);
            Assert.Greater(hits.Count, 0);
            for (int i = 0; i < hits.Count; i++)
            {
                Assert.IsNull(hits[i].Target);
            }

            Assert.AreEqual(70f, hidden.Current, 0.001f);
        }

        [Test]
        public void ProjectileAdvancesAtTheAssetSpeedAndStopsOnAWall()
        {
            WeaponDefinition weapon = Weapon();
            weapon.SetDelivery(WeaponDelivery.Projectile);
            weapon.SetSpeed(20f);
            weapon.SetBlast(6f, 60f, 0.2f);
            GameObject body = new GameObject("rocket-step");
            trash.Add(body);
            body.transform.position = new Vector3(600f, 500f, 0f);
            RocketProjectile openAir = body.AddComponent<RocketProjectile>();
            openAir.Launch(new ProjectileLaunch
            {
                Origin = body.transform.position,
                Direction = Vector3.up,
                Weapon = weapon,
                Resolver = new DamageResolver()
            });
            openAir.Step(0.1f);
            Assert.AreEqual(2f, body.transform.position.y - 500f, 0.02f);
            Object.DestroyImmediate(body);

            HealthComponent open = CreateHealth(new Vector3(604.5f, 460f, 2f), 100f, 51, 2);
            HealthComponent covered = CreateHealth(new Vector3(600f, 460f, 6f), 100f, 52, 2);
            Box(new Vector3(600f, 460f, 4f), new Vector3(2f, 3f, 1f));
            GameObject rocketObject = new GameObject("rocket-hit");
            trash.Add(rocketObject);
            RocketProjectile rocket = rocketObject.AddComponent<RocketProjectile>();
            rocket.Launch(new ProjectileLaunch
            {
                Origin = new Vector3(600f, 460f, 0f),
                Direction = Vector3.forward,
                Weapon = weapon,
                Resolver = new DamageResolver(),
                AttackerId = 99,
                AttackerTeam = 1,
                ConfirmationBase = 80,
                HasAuthority = true
            });
            for (int i = 0; i < 12 && rocket != null; i++)
            {
                rocket.Step(0.05f);
            }

            Assert.Less(open.Current, 100f);
            Assert.AreEqual(100f, covered.Current, 0.001f);
        }

        [Test]
        public void ZeroRecoilLeavesTheCamera()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            trash.Add(tuning);
            GameObject body = new GameObject("recoil-body");
            trash.Add(body);
            GameObject cameraObject = new GameObject("recoil-camera");
            cameraObject.transform.SetParent(body.transform, false);
            cameraObject.AddComponent<Camera>();
            PlayerLook look = body.AddComponent<PlayerLook>();
            look.SetView(cameraObject.GetComponent<Camera>());
            look.SetTuning(tuning);
            look.AddRecoil(0f);
            look.ApplyLook(Vector2.zero);
            Assert.AreEqual(0f, cameraObject.transform.localEulerAngles.x, 0.1f);
            look.AddRecoil(4f);
            look.ApplyLook(Vector2.zero);
            Assert.Less(Mathf.DeltaAngle(0f, cameraObject.transform.localEulerAngles.x), -0.5f);
            Assert.AreEqual(0f, body.transform.eulerAngles.x, 0.1f);
        }

        [Test]
        public void DefinitionExposesTheWeaponCatalog()
        {
            WeaponDefinition weapon = Weapon();
            weapon.SetIdentity("shotgun", "Escopeta");
            Assert.AreEqual("shotgun", weapon.WeaponId);
            Assert.Greater(weapon.Damage, 0f);
            Assert.Greater(weapon.FireInterval, 0f);
            Assert.Greater(weapon.MagazineSize, 0);
            Assert.Greater(weapon.MaxRange, 0f);
            Assert.Greater(weapon.BlastRadius, 0f);
            Assert.Greater(weapon.ProjectileSpeed, 0f);
            Assert.IsNull(weapon.ProjectilePrefab);
            Assert.IsNull(weapon.ShotSound);
            Assert.IsNull(weapon.ImpactVfx);
            weapon.SetRecoil(0f);
            Assert.AreEqual(0f, weapon.RecoilPitch);
        }

        HealthComponent CreateHealth(Vector3 position, float max, int id, int team)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trash.Add(body);
            body.transform.position = position;
            HealthProfile profile = ScriptableObject.CreateInstance<HealthProfile>();
            trash.Add(profile);
            profile.SetMaxHealth(max);
            HealthComponent health = body.AddComponent<HealthComponent>();
            health.UseProfile(profile);
            health.SetStableId(id);
            health.SetTeam(team);
            return health;
        }

        GameObject Box(Vector3 position, Vector3 scale)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trash.Add(body);
            body.transform.position = position;
            body.transform.localScale = scale;
            return body;
        }

        WeaponDefinition Weapon()
        {
            WeaponDefinition weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            trash.Add(weapon);
            return weapon;
        }

        WeaponController Controller(params WeaponDefinition[] weapons)
        {
            GameObject body = new GameObject("weapons");
            trash.Add(body);
            WeaponController controller = body.AddComponent<WeaponController>();
            controller.LoadForTests(weapons);
            return controller;
        }

        PlayerCombat Shooter(WeaponDefinition weapon, Vector3 position)
        {
            GameObject body = new GameObject("shooter");
            trash.Add(body);
            body.transform.position = position;
            WeaponController controller = body.AddComponent<WeaponController>();
            controller.LoadForTests(new[] { weapon });
            PlayerCombat combat = body.AddComponent<PlayerCombat>();
            combat.BindForTests(
                controller,
                body.AddComponent<DamageResolverHost>(),
                null,
                null,
                null,
                null);
            return combat;
        }

        static DamageInfo Hit(
            int attacker,
            int team,
            HealthComponent health,
            float amount,
            ulong confirmation,
            bool authority)
        {
            return new DamageInfo(
                attacker,
                team,
                health,
                amount,
                DamageType.Hitscan,
                health.transform.position,
                confirmation,
                authority,
                false,
                "test");
        }
    }
}
