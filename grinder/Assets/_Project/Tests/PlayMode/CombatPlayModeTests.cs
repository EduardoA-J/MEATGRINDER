using System.Collections;
using System.Collections.Generic;
using Meatgrinder.Combat;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class CombatPlayModeTests
    {
        readonly List<Object> trash = new List<Object>();

        [TearDown]
        public void Cleanup()
        {
            for (int i = 0; i < trash.Count; i++)
            {
                if (trash[i] != null)
                {
                    Object.Destroy(trash[i]);
                }
            }

            trash.Clear();
        }

        [UnityTest]
        public IEnumerator ShotgunDamagesTheOpenTargetAndNotTheHiddenOne()
        {
            WeaponDefinition weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            trash.Add(weapon);
            weapon.SetSpread(0f, 1);
            weapon.SetFalloff(20f, 30f, 0.25f, 40f);
            weapon.SetMaxRange(30f);
            weapon.SetAmmo(2, 0, 2, 1);
            weapon.SetFireInterval(0f);
            weapon.SetRecoil(0f);
            HealthComponent open = CreateHealth(new Vector3(700f, 300f, 4f), 50f, 71, 2);
            Box(new Vector3(700f, 300f, 8f), new Vector3(3f, 3f, 0.5f));
            HealthComponent hidden = CreateHealth(new Vector3(700f, 300f, 11f), 50f, 72, 2);
            GameObject shooter = new GameObject("play-shooter");
            trash.Add(shooter);
            shooter.transform.position = new Vector3(700f, 300f, 0f);
            WeaponController weapons = shooter.AddComponent<WeaponController>();
            weapons.LoadForTests(new[] { weapon });
            PlayerImpactFeedback feedback = shooter.AddComponent<PlayerImpactFeedback>();
            PlayerCombat combat = shooter.AddComponent<PlayerCombat>();
            combat.BindForTests(
                weapons,
                shooter.AddComponent<DamageResolverHost>(),
                feedback,
                null,
                null,
                null);
            yield return new WaitForFixedUpdate();
            combat.FireFrom(shooter.transform.position, Vector3.forward, 4);
            Assert.Less(open.Current, 50f);
            Assert.AreEqual(50f, hidden.Current, 0.001f);
            Assert.Greater(feedback.TargetConfirms, 0);
            open.gameObject.SetActive(false);
            combat.FireFrom(shooter.transform.position, Vector3.forward, 5);
            Assert.Greater(feedback.WorldMarks, 0);
        }

        [UnityTest]
        public IEnumerator RocketMovesAndSparesTheCoveredTarget()
        {
            WeaponDefinition weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            trash.Add(weapon);
            weapon.SetDelivery(WeaponDelivery.Projectile);
            weapon.SetSpeed(20f);
            weapon.SetBlast(6.5f, 70f, 0.2f);
            weapon.SetAmmo(1, 0, 1, 1);
            HealthComponent open = CreateHealth(new Vector3(735f, 320f, 2.4f), 90f, 81, 2);
            HealthComponent covered = CreateHealth(new Vector3(730f, 320f, 7f), 90f, 82, 2);
            HealthComponent shooter = CreateHealth(new Vector3(730f, 320f, -1.2f), 90f, 80, 1);
            Box(new Vector3(730f, 320f, 4.2f), new Vector3(2f, 3f, 1f));
            GameObject host = new GameObject("play-rocket");
            trash.Add(host);
            WeaponController weapons = host.AddComponent<WeaponController>();
            weapons.LoadForTests(new[] { weapon });
            LocalProjectileSpawner spawner = host.AddComponent<LocalProjectileSpawner>();
            PlayerCombat combat = host.AddComponent<PlayerCombat>();
            combat.BindForTests(
                weapons,
                host.AddComponent<DamageResolverHost>(),
                host.AddComponent<PlayerImpactFeedback>(),
                spawner,
                shooter,
                null);
            combat.FireFrom(new Vector3(730f, 320f, 0f), Vector3.forward, 1);
            for (int i = 0; i < 25; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.Less(open.Current, 90f);
            Assert.AreEqual(90f, covered.Current, 0.001f);
            Assert.AreEqual(90f, shooter.Current, 0.001f);
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

        void Box(Vector3 position, Vector3 scale)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trash.Add(body);
            body.transform.position = position;
            body.transform.localScale = scale;
        }
    }
}
