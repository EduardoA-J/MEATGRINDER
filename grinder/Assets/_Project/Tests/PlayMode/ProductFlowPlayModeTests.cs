using System.Collections;
using System.IO;
using System.Linq;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Movement;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class ProductFlowPlayModeTests
    {
        static readonly string[] Maps = { "M01_TheSlaughterhouse", "M02_CastleOfTheDamned", "M03_RuinsOfTheForgotten", "M04_TheLongestVoid" };
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Time.timeScale = 1;
            var session = Object.FindFirstObjectByType<LocalGameSession>();
            if (session != null) Object.Destroy(session.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator BootMenuMapPauseResultsAndReturnAreConnected()
        {
            yield return Load("Boot");
            var session = Object.FindFirstObjectByType<LocalGameSession>();
            Assert.NotNull(session);
            Assert.AreEqual(LocalSessionPhase.Menu, session.Phase);
            Assert.AreEqual(1, Object.FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None).Length);
            yield return new WaitForSecondsRealtime(.25f);
            Capture("01-menu");
            Click("Play"); yield return null;
            Click("M01");
            for (int i = 0; i < 600 && session.Phase != LocalSessionPhase.Playing; i++) yield return null;
            Assert.AreEqual(LocalSessionPhase.Playing, session.Phase);
            Assert.NotNull(session.Player);
            yield return new WaitForFixedUpdate();
            var weapons = session.Player.GetComponent<WeaponController>();
            Assert.AreEqual(0, weapons.SlotCount, "Product first spawn must start melee-only.");
            Capture("02-melee-hud");
            var shotgun = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None).First(p => p.Definition.PickupId == "shotgun_weapon");
            Assert.True(shotgun.Collect(session.Player.GetComponent<HealthComponent>()));
            weapons.Tick(1); yield return null;
            Assert.AreEqual("shotgun", weapons.ActiveDefinition.WeaponId);
            Capture("03-shotgun-hud");
            var rocket = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None).First(p => p.Definition.PickupId == "rocket_weapon");
            Assert.True(rocket.Collect(session.Player.GetComponent<HealthComponent>()));
            weapons.SelectWeapon("rocket_launcher"); weapons.Tick(1); yield return null;
            Capture("04-rocket-hud");
            foreach (var p in Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None))
                if (p.Definition.Type == PickupType.PowerUp) Assert.True(p.Collect(session.Player.GetComponent<HealthComponent>()));
            yield return new WaitForSeconds(.15f);
            Capture("05-powers-hud");
            session.Pause(); yield return null;
            Assert.AreEqual(0, Time.timeScale);
            Assert.False(session.Player.GetComponent<PlayerInputReader>().enabled);
            Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
            Click("Resume"); yield return null;
            Assert.AreEqual(1, Time.timeScale);
            Assert.True(session.Player.GetComponent<PlayerInputReader>().enabled);
            session.Finish(); yield return null;
            Assert.AreEqual(LocalSessionPhase.Results, session.Phase);
            Assert.GreaterOrEqual(session.Pickups, 8);
            yield return new WaitForSecondsRealtime(.25f);
            Capture("06-results");
            Click("MainMenu");
            for (int i = 0; i < 600 && session.Phase != LocalSessionPhase.Menu; i++) yield return null;
            Assert.AreEqual("Boot", SceneManager.GetActiveScene().name);
            Assert.AreEqual(1, Object.FindObjectsByType<LocalGameSession>(FindObjectsSortMode.None).Length);
            Assert.AreEqual(1, Time.timeScale);
            Click("Create"); Click("M02"); yield return null;
            Assert.AreEqual("Boot", SceneManager.GetActiveScene().name, "Creating a match must wait for confirmation.");
            Click("SetupOptions"); yield return null;
            Click("Back"); yield return null;
            yield return new WaitForSecondsRealtime(.25f);
            Capture("09-create-match");
            Click("CreateConfirmed");
            float deadline = Time.realtimeSinceStartup + 15;
            while (session.Phase == LocalSessionPhase.Loading && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(LocalSessionPhase.Playing, session.Phase);
            Assert.AreEqual(Maps[1], SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator EveryMapHasConnectedTuningWeaponPickupsAndSixPowers()
        {
            foreach (string name in Maps)
            {
                yield return Load(name);
                var player = GameObject.Find("LocalPlayer");
                Assert.AreEqual(1, player.GetComponents<PlayerLife>().Length, name + " duplicate lifecycle component");
                yield return new WaitForSeconds(.15f);
                Capture(name.Substring(0, 3) + "-spawn");
                Assert.NotNull(player.GetComponent<PlayerVoidFall>());
                Assert.NotNull(player.GetComponent<WeaponVisualRig>() ?? player.GetComponentInChildren<WeaponVisualRig>(true));
                var spawns = Object.FindFirstObjectByType<LocalSpawnPoints>();
                Assert.GreaterOrEqual(spawns.Points.Length, 2);
                var pickups = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None);
                Assert.True(pickups.All(p => p.Definition != null));
                Assert.AreEqual(6, pickups.Where(p => p.Definition.Type == PickupType.PowerUp).Select(p => p.Definition.PowerUpType).Distinct().Count());
                Assert.True(pickups.Any(p => p.Definition.Type == PickupType.Weapon && p.Definition.Weapon.WeaponId == "shotgun"));
                Assert.True(pickups.Any(p => p.Definition.Type == PickupType.Weapon && p.Definition.Weapon.WeaponId == "rocket_launcher"));
                var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
                player.transform.position = new Vector3(1000, spawns.VoidHeight - .1f, 1000); controller.enabled = true;
                var fall = player.GetComponent<PlayerVoidFall>();
                yield return new WaitForFixedUpdate();
                Assert.True(fall.IsFalling, name + " must detect void with the real scene's tuning.");
            }
        }

        [UnityTest]
        public IEnumerator VoidKillsWithinFiveSecondsDespitePowersAndRespawnsMeleeOnly()
        {
            yield return Load(Maps[3]);
            var player = GameObject.Find("LocalPlayer"); var health = player.GetComponent<HealthComponent>();
            var power = player.GetComponent<PowerUpManager>(); var weapons = player.GetComponent<WeaponController>();
            power.ActivatePowerUp(new InvulnerabilityEffect(30, 1)); power.ActivatePowerUp(new RegenerationEffect(health, 30, 100));
            health.AddArmor(100); health.AddOverhealth(100, 200);
            var pickup = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None).First(p => p.Definition.PickupId == "shotgun_weapon");
            pickup.Collect(health);
            var body = player.GetComponent<CharacterController>(); body.enabled = false;
            player.transform.position = new Vector3(1000, -4.1f, 1000); body.enabled = true;
            Vector3 deathArea = player.transform.position;
            int ticks = 0;
            while (health.IsAlive && ticks++ < 252)
            {
                yield return new WaitForFixedUpdate();
                if (ticks == 125) Capture("07-void-darkness");
            }
            Assert.False(health.IsAlive); Assert.LessOrEqual(ticks, 252);
            Assert.AreEqual(100, health.Armor, "Void bypasses armor.");
            Assert.False(power.HasEffect(PowerUpType.Invulnerability));
            Assert.False(player.GetComponent<PlayerInputReader>().enabled);
            yield return null;
            Capture("08-death");
            for (int i = 0; i < 205; i++) yield return new WaitForFixedUpdate();
            Assert.True(health.IsAlive, "A configured safe different spawn must be usable.");
            Assert.AreEqual(100, health.Current); Assert.AreEqual(0, health.Armor); Assert.AreEqual(0, weapons.SlotCount);
            Assert.False(player.GetComponent<PlayerVoidFall>().IsFalling);
            Assert.Greater(Vector3.Distance(deathArea, player.transform.position), 10);
            Assert.True(player.GetComponent<PlayerInputReader>().enabled);
        }

        [UnityTest]
        public IEnumerator KeyOpensSharedDoorsAndExitLoadsNextMap()
        {
            yield return Load(Maps[0]);
            var player = GameObject.Find("LocalPlayer");
            var key = Object.FindFirstObjectByType<RouteKey>();
            Assert.NotNull(key);
            var gates = Object.FindObjectsByType<KeyGate>(FindObjectsSortMode.None);
            Assert.AreEqual(2, gates.Length);
            Assert.True(gates.All(g => !g.IsOpen));
            Teleport(player, key.transform.position);
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Assert.True(Object.FindFirstObjectByType<MapRouteState>().Has(key.Definition));
            yield return new WaitForSeconds(1.3f);
            Assert.True(gates.All(g => g.IsOpen));
            var exit = gates.First(g => g.AdvancesMap);
            Assert.AreEqual(LocalSessionPhase.Playing, Object.FindFirstObjectByType<LocalGameSession>().Phase);
            Teleport(player, exit.transform.position + new Vector3(0, .1f, .7f));
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            float deadline = Time.realtimeSinceStartup + 15;
            while (SceneManager.GetActiveScene().name != Maps[1] && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreEqual(Maps[1], SceneManager.GetActiveScene().name);
            yield return null;
            Assert.AreEqual(0, GameObject.Find("LocalPlayer").GetComponent<WeaponController>().SlotCount);
        }

        [UnityTest]
        public IEnumerator RocketStartsAtMuzzleAndCannotSpawnThroughCover()
        {
            yield return Load(Maps[0]);
            var player = GameObject.Find("LocalPlayer"); var weapons = player.GetComponent<WeaponController>();
            var pickup = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None).First(p => p.Definition.PickupId == "rocket_weapon");
            Assert.True(pickup.Collect(player.GetComponent<HealthComponent>()));
            weapons.Tick(1); yield return null;
            var muzzle = player.GetComponentInChildren<WeaponVisualRig>(true).transform.Find("RocketProduct/MuzzleSocket");
            Assert.NotNull(muzzle);
            var view = player.GetComponent<PlayerLook>().View.transform;
            var spawner = player.GetComponent<LocalProjectileSpawner>();
            RocketProjectile spawned = null;
            spawner.Spawned += rocket => spawned = rocket;
            player.GetComponent<PlayerCombat>().FireFrom(view.position, view.forward, 43);
            Assert.NotNull(spawned);
            Assert.Less(Vector3.Distance(muzzle.position, spawned.transform.position), .001f);
            Object.Destroy(spawned.gameObject); spawned = null;
            var cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Vector3 offset = muzzle.position - view.position;
            cover.transform.position = view.position + offset * .5f;
            cover.transform.rotation = Quaternion.LookRotation(offset);
            cover.transform.localScale = new Vector3(2, 2, .05f);
            Physics.SyncTransforms(); weapons.Tick(2);
            player.GetComponent<PlayerCombat>().FireFrom(view.position, view.forward, 44);
            Assert.NotNull(spawned);
            Assert.Less(Vector3.Distance(view.position, spawned.transform.position), offset.magnitude * .5f);
            Object.Destroy(spawned.gameObject); Object.Destroy(cover);
        }

        [UnityTest]
        public IEnumerator DeathLootReturnsExactAmmoToItsRespawnedOwner()
        {
            yield return Load(Maps[0]);
            var player = GameObject.Find("LocalPlayer"); var health = player.GetComponent<HealthComponent>(); var weapons = player.GetComponent<WeaponController>();
            var pickup = Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None).First(p => p.Definition.PickupId == "shotgun_weapon");
            pickup.Collect(health); weapons.Tick(1); weapons.TryBeginShot();
            int loaded = weapons.Loaded, reserve = weapons.Reserve;
            health.BeginSpawnProtection(0);
            player.GetComponent<DamageResolverHost>().Resolver.Apply(new DamageInfo(99999, 0, health, 9999, DamageType.Hitscan, player.transform.position, 992, true, true, "test"));
            yield return new WaitForFixedUpdate();
            var loot = Object.FindFirstObjectByType<DeathLoot>(); Assert.NotNull(loot);
            for (int i = 0; i < 205; i++) yield return new WaitForFixedUpdate();
            Assert.True(health.IsAlive); Assert.AreEqual(0, weapons.SlotCount);
            Teleport(player, loot.transform.position + Vector3.up * .1f);
            yield return new WaitForFixedUpdate(); yield return new WaitForFixedUpdate();
            Assert.AreEqual(1, weapons.SlotCount); Assert.AreEqual(loaded, weapons.Loaded); Assert.AreEqual(reserve, weapons.Reserve);
        }

        static void Teleport(GameObject player, Vector3 position)
        {
            var body = player.GetComponent<CharacterController>(); body.enabled = false; player.transform.position = position; body.enabled = true;
            player.GetComponent<PlayerMovement>().ResetMotion(); Physics.SyncTransforms();
        }

        [UnityTest]
        public IEnumerator MeleeHasWindupOneImpactAndRespectsCover()
        {
            yield return Load(Maps[0]);
            var player = GameObject.Find("LocalPlayer"); var melee = player.GetComponent<PlayerMelee>();
            var view = player.GetComponent<PlayerLook>().View.transform;
            var target = GameObject.CreatePrimitive(PrimitiveType.Cube); target.name = "MeleeTestTarget";
            target.transform.position = view.position + view.forward * 1.5f; target.transform.localScale = Vector3.one * .7f;
            var health = target.AddComponent<HealthComponent>();
            Physics.SyncTransforms(); Assert.True(melee.TrySwing()); Assert.AreEqual(100, health.Current);
            yield return new WaitForSeconds(.3f);
            Assert.AreEqual(80, health.Current, .01f);
            yield return new WaitForSeconds(.35f); Assert.AreEqual(80, health.Current, .01f);
            var cover = GameObject.CreatePrimitive(PrimitiveType.Cube); cover.transform.position = view.position + view.forward * .65f; cover.transform.localScale = new Vector3(2, 2, .15f);
            Physics.SyncTransforms(); Assert.True(melee.TrySwing()); yield return new WaitForSeconds(.3f);
            Assert.AreEqual(80, health.Current, .01f);
            Object.Destroy(target); Object.Destroy(cover);
        }

        static IEnumerator Load(string name)
        {
            Time.timeScale = 1;
            var operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone) yield return null;
            yield return null;
        }
        static void Click(string name)
        {
            var button = Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None).FirstOrDefault(b => b.name == name && b.gameObject.activeInHierarchy);
            Assert.NotNull(button, "Missing active button " + name); button.onClick.Invoke();
        }
        static void Capture(string name)
        {
            if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null) return;
            var camera = Object.FindFirstObjectByType<Camera>(); if (camera == null) return;
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None).Where(c => c.renderMode == RenderMode.ScreenSpaceOverlay).ToArray();
            var texture = new RenderTexture(1366, 768, 24); var previous = camera.targetTexture;
            camera.targetTexture = texture;
            foreach (var canvas in canvases) { canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = camera.nearClipPlane + .001f; }
            Canvas.ForceUpdateCanvases(); camera.Render();
            var old = RenderTexture.active; RenderTexture.active = texture;
            var image = new Texture2D(1366, 768, TextureFormat.RGB24, false); image.ReadPixels(new Rect(0, 0, 1366, 768), 0, 0); image.Apply();
            string folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Documentation/test-results/product")); Directory.CreateDirectory(folder);
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), image.EncodeToPNG());
            RenderTexture.active = old; camera.targetTexture = previous;
            foreach (var canvas in canvases) { canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.worldCamera = null; }
            Object.Destroy(texture); Object.Destroy(image);
        }
    }
}
