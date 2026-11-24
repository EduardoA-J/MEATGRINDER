using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Weapons;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Meatgrinder.Editor.ProductAssets;

namespace Meatgrinder.Editor
{
    public static class ProductSceneInstaller
    {
        const string StartupScenePath = Root + "00_Scenes/Startup.unity";
        const string BootScenePath = Root + "00_Scenes/Boot.unity";
        public static readonly string[] MapNames = { "M01_TheSlaughterhouse", "M02_CastleOfTheDamned", "M03_RuinsOfTheForgotten", "M04_TheLongestVoid" };
        static readonly string[] Titles = { "THE SLAUGHTERHOUSE", "CASTLE OF THE DAMNED", "RUINS OF THE FORGOTTEN", "THE LONGEST VOID" };
        static readonly string[] Descriptions = { "Metal, cámaras de procesado y pasarelas. Domina las rutas de la fábrica.", "Murallas, patios y criptas. Alterna exploración y combate entre torres.", "Templos rotos y terrazas. Busca recursos entre columnas y desniveles.", "Islas suspendidas. Controla los puentes y sobrevive al borde del vacío." };
        static Material steel, dark, copper, glow;
        static PresentationTheme theme;
        static WeaponDefinition shotgun, rocket;
        static GameObject shotgunVisual, rocketVisual, meleeVisual;
        static LifeTuning life;
        static MeleeTuning melee;
        static GameObject sessionPrefab;

        [MenuItem("Meatgrinder/Product/Install local flow and presentation")]
        public static void Install()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before authoring assets.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save the open scene before installing product content.");
            var setup = EditorSceneManager.GetSceneManagerSetup();
            try
            {
                CreateAssets();
                CreatePlayer();
                CreateSession();
                for (int i = 0; i < MapNames.Length; i++) InstallMap(i);
                CreateBoot();
                CreateStartup();
                AssetDatabase.SaveAssets();
                Directory.CreateDirectory(ProductVerificationRunner.Output);
                File.WriteAllText(Path.Combine(ProductVerificationRunner.Output, "installation.txt"), "Installed Startup, Boot, four map definitions, local session, player tuning, visual prefabs, pickups, map presentation. Existing map geometry preserved.\n" + DateTime.UtcNow.ToString("O"));
            }
            finally { if (setup.Length > 0 && !Application.isBatchMode) EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }
        static void CreateAssets()
        {
            steel = Material("BrushedSteel", new Color(.45f, .5f, .52f), 0, .7f);
            dark = Material("BlackSteel", new Color(.16f, .18f, .2f), 0, .65f);
            copper = Material("Oxide", new Color(.56f, .18f, .07f), 0, .45f);
            glow = Material("ReactorAmber", new Color(1, .4f, .06f), -1, .2f, new Color(1, .22f, .015f));
            theme = Data<PresentationTheme>(Root + "02_ScriptableObjects/Presentation/ProductTheme.asset");
            Array(theme, "powerIcons", Enumerable.Range(0, 6).Select(i => (UnityEngine.Object)Icon((PowerUpType)i)).ToArray());
            Array(theme, "feedbackSounds", Enumerable.Range(0, 10).Select(i => (UnityEngine.Object)Sound(i)).ToArray());
            life = Data<LifeTuning>(Root + "02_ScriptableObjects/Players/LocalLife.asset");
            melee = Data<MeleeTuning>(Root + "02_ScriptableObjects/Players/LocalMelee.asset");
            Set(life, "maximumVoidDuration", 5f);
            string lootPath = Root + "03_Prefabs/Pickups/DeathLoot.prefab";
            if (Asset<GameObject>(lootPath) == null)
            {
                var bag = new GameObject("DeathLoot");
                var solid = bag.AddComponent<BoxCollider>(); solid.size = new Vector3(.65f, .4f, .45f);
                var trigger = bag.AddComponent<SphereCollider>(); trigger.radius = 1; trigger.isTrigger = true;
                bag.AddComponent<Rigidbody>(); bag.AddComponent<DeathLoot>();
                Part(bag.transform, "Case", PrimitiveType.Cube, Vector3.zero, solid.size, dark);
                for (int i = -1; i <= 1; i += 2) Part(bag.transform, "Strap", PrimitiveType.Cube, new Vector3(i * .2f, 0, 0), new Vector3(.06f, .42f, .47f), copper);
                SavePrefab(bag, lootPath);
            }
            Set(life, "lootPrefab", Asset<GameObject>(lootPath));
            shotgun = Asset<WeaponDefinition>(Root + "02_ScriptableObjects/Weapons/Shotgun.asset");
            rocket = Asset<WeaponDefinition>(Root + "02_ScriptableObjects/Weapons/RocketLauncher.asset");
            Set(shotgun, "shotSound", Sound(0)); Set(rocket, "shotSound", Sound(1));
            shotgunVisual = WeaponModel("ShotgunProduct", 0); rocketVisual = WeaponModel("RocketProduct", 1); meleeVisual = WeaponModel("CleaverProduct", 2);
            CreateRocket();
            Resource("Shotgun_Weapon", "Escopeta", PickupType.Weapon, shotgun, 24);
            Resource("Rocket_Weapon", "Lanzacohetes", PickupType.Weapon, rocket, 12);
            Resource("Shell_Ammo", "Cartuchos", PickupType.Ammo, shotgun, 6);
            Resource("Rocket_AmmoOnly", "Cohetes", PickupType.Ammo, rocket, 4);
            foreach (string guid in AssetDatabase.FindAssets("t:PickupDefinition", new[] { Root + "02_ScriptableObjects/Pickups" }))
            {
                var definition = Asset<PickupDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                var prefab = PickupPrefab(definition);
                if (definition.Type == PickupType.PowerUp)
                {
                    string path = AssetDatabase.GetAssetPath(prefab);
                    var contents = PrefabUtility.LoadPrefabContents(path);
                    foreach (var sprite in contents.GetComponentsInChildren<SpriteRenderer>()) sprite.sprite = Icon(definition.PowerUpType);
                    PrefabUtility.SaveAsPrefabAsset(contents, path); PrefabUtility.UnloadPrefabContents(contents);
                }
                Set(definition, "visualPrefab", prefab.transform.Find("Visual").gameObject);
                Set(definition, "pickupSound", Sound(6));
            }
            for (int i = 0; i < 4; i++)
            {
                var map = Data<MapDefinition>(Root + "02_ScriptableObjects/Maps/M0" + (i + 1) + ".asset");
                Set(map, "mapId", "M0" + (i + 1)); Set(map, "displayName", Titles[i]); Set(map, "sceneName", MapNames[i]); Set(map, "description", Descriptions[i]);
            }
        }
        static void Resource(string name, string display, PickupType type, WeaponDefinition weapon, int ammo)
        {
            var definition = Data<PickupDefinition>(Root + "02_ScriptableObjects/Pickups/" + name + ".asset");
            Set(definition, "pickupId", name.ToLowerInvariant()); Set(definition, "displayName", display);
            Set(definition, "type", (int)type); Set(definition, "weapon", weapon); Set(definition, "ammoAmount", ammo); Set(definition, "respawnTime", 20f);
        }
        static GameObject WeaponModel(string name, int kind)
        {
            string path = Root + "03_Prefabs/Weapons/" + name + ".prefab";
            var existing = Asset<GameObject>(path); if (existing != null) return existing;
            var root = new GameObject(name); var socket = new GameObject("ModelSocket"); socket.transform.SetParent(root.transform, false);
            Transform parent = socket.transform;
            if (kind == 0)
            {
                Part(parent, "Receiver", PrimitiveType.Cube, Vector3.zero, new Vector3(.13f, .14f, .36f), dark);
                Tube(parent, "Barrel", new Vector3(0, .035f, .34f), .05f, .56f, steel);
                Tube(parent, "MagazineTube", new Vector3(0, -.045f, .25f), .035f, .42f, dark);
                Part(parent, "Stock", PrimitiveType.Cube, new Vector3(0, -.035f, -.23f), new Vector3(.11f, .16f, .22f), copper);
                var pump = new GameObject("Pump"); pump.transform.SetParent(parent, false);
                for (int i = 0; i < 7; i++) Part(pump.transform, "GripRib" + i, PrimitiveType.Cube, new Vector3(0, -.05f, .14f + i * .018f), new Vector3(.14f, .09f, .012f), copper);
                Part(parent, "Sight", PrimitiveType.Cube, new Vector3(0, .09f, .52f), new Vector3(.018f, .045f, .025f), glow);
            }
            else if (kind == 1)
            {
                Tube(parent, "LaunchTube", new Vector3(0, .02f, .15f), .13f, .7f, dark);
                Tube(parent, "MuzzleCollar", new Vector3(0, .02f, .49f), .16f, .1f, steel);
                Tube(parent, "MuzzleBore", new Vector3(0, .02f, .546f), .12f, .008f, dark);
                for (int i = 0; i < 5; i++) Tube(parent, "CoolingRing" + i, new Vector3(0, .02f, .05f + i * .06f), .14f, .022f, copper);
                Part(parent, "Grip", PrimitiveType.Cube, new Vector3(.02f, -.17f, -.08f), new Vector3(.09f, .24f, .1f), dark);
                Part(parent, "SightRail", PrimitiveType.Cube, new Vector3(0, .18f, .12f), new Vector3(.06f, .05f, .3f), steel);
                Part(parent, "PowerIndicator", PrimitiveType.Cube, new Vector3(.1f, .08f, -.15f), new Vector3(.05f, .04f, .14f), glow);
            }
            else
            {
                Part(parent, "Handle", PrimitiveType.Cube, new Vector3(0, -.05f, 0), new Vector3(.06f, .26f, .07f), copper);
                Part(parent, "Guard", PrimitiveType.Cube, new Vector3(0, .08f, 0), new Vector3(.19f, .03f, .1f), dark);
                Part(parent, "Blade", PrimitiveType.Cube, new Vector3(.07f, .27f, 0), new Vector3(.27f, .32f, .025f), steel);
                Part(parent, "CuttingEdge", PrimitiveType.Cube, new Vector3(.07f, .44f, 0), new Vector3(.27f, .025f, .012f), Material("BladeEdge", new Color(.8f, .84f, .85f), -1, .8f));
            }
            Part(parent, "RightGauntlet", PrimitiveType.Capsule, new Vector3(.01f, -.18f, -.08f), new Vector3(.12f, .12f, .16f), dark);
            if (kind < 2) Part(parent, "SupportGauntlet", PrimitiveType.Capsule, new Vector3(-.04f, -.13f, .2f), new Vector3(.13f, .1f, .16f), dark);
            var muzzle = new GameObject("MuzzleSocket"); muzzle.transform.SetParent(root.transform, false); muzzle.transform.localPosition = new Vector3(0, .035f, .6f);
            var flash = Part(muzzle.transform, "Flash", PrimitiveType.Sphere, Vector3.zero, new Vector3(.15f, .15f, .25f), glow); flash.GetComponent<Renderer>().enabled = false;
            return SavePrefab(root, path);
        }
        static void Tube(Transform parent, string name, Vector3 position, float radius, float length, Material material)
        {
            var part = Part(parent, name, PrimitiveType.Cylinder, position, new Vector3(radius * 2, length * .5f, radius * 2), material);
            part.transform.localRotation = Quaternion.Euler(90, 0, 0);
        }
        static void CreateRocket()
        {
            string path = Root + "03_Prefabs/Weapons/RocketProjectile.prefab";
            var prefab = Asset<GameObject>(path);
            if (prefab == null)
            {
                var root = new GameObject("RocketProjectile"); root.AddComponent<RocketProjectile>();
                Tube(root.transform, "Shell", Vector3.zero, .07f, .35f, copper);
                Part(root.transform, "Nose", PrimitiveType.Sphere, new Vector3(0, 0, .18f), new Vector3(.14f, .14f, .2f), steel);
                var trail = root.AddComponent<TrailRenderer>(); trail.time = .22f; trail.startWidth = .12f; trail.endWidth = 0; trail.sharedMaterial = glow;
                prefab = SavePrefab(root, path);
            }
            Set(rocket, "projectilePrefab", prefab);
        }
        static GameObject PickupPrefab(PickupDefinition definition)
        {
            string path = Root + "03_Prefabs/Pickups/" + definition.name + ".prefab";
            var existing = Asset<GameObject>(path); if (existing != null) return existing;
            var root = new GameObject(definition.name); var collider = root.AddComponent<SphereCollider>(); collider.isTrigger = true; collider.radius = .85f;
            var pickup = root.AddComponent<PickupController>(); Set(pickup, "definition", definition); Set(pickup, "trigger", collider);
            var visual = new GameObject("Visual"); visual.transform.SetParent(root.transform, false); visual.transform.localPosition = Vector3.up * .5f;
            Color color = definition.Type == PickupType.PowerUp ? PresentationTheme.PowerColor(definition.PowerUpType) : definition.Type == PickupType.Health ? new Color(1, .15f, .12f) : definition.Type == PickupType.Armor ? new Color(.2f, .55f, 1) : new Color(1, .6f, .1f);
            Material emissive = Material("Pickup_" + definition.name, color, -1, .3f, color * .4f);
            if (definition.Type == PickupType.Weapon)
            {
                var model = (GameObject)PrefabUtility.InstantiatePrefab(definition.Weapon == shotgun ? shotgunVisual : rocketVisual);
                model.transform.SetParent(visual.transform, false); model.transform.localScale = Vector3.one * 1.6f; model.transform.localRotation = Quaternion.Euler(0, 70, 0);
            }
            else if (definition.Type == PickupType.PowerUp)
            {
                var core = Part(visual.transform, "Core", PrimitiveType.Cube, Vector3.zero, Vector3.one * .42f, emissive); core.transform.localRotation = Quaternion.Euler(35, 0, 45);
                for (int i = 0; i < 4; i++)
                {
                    var face = new GameObject("Emblem" + i); face.transform.SetParent(visual.transform, false);
                    face.transform.localRotation = Quaternion.Euler(0, i * 90, 0); face.transform.localPosition = face.transform.forward * .37f;
                    var sprite = face.AddComponent<SpriteRenderer>(); sprite.sprite = Icon(definition.PowerUpType); sprite.color = color; face.transform.localScale = Vector3.one * .5f;
                }
            }
            else
            {
                Part(visual.transform, "SupplyCase", PrimitiveType.Cube, Vector3.zero, new Vector3(.55f, .32f, .35f), dark);
                Part(visual.transform, "Stripe", PrimitiveType.Cube, new Vector3(0, .18f, 0), new Vector3(.5f, .03f, .1f), emissive);
                if (definition.Type == PickupType.Health) Part(visual.transform, "Cross", PrimitiveType.Cube, new Vector3(0, .2f, 0), new Vector3(.1f, .03f, .3f), emissive);
            }
            var halo = Part(root.transform, "Pedestal", PrimitiveType.Cylinder, new Vector3(0, -.16f, 0), new Vector3(.9f, .035f, .9f), emissive);
            Set(pickup, "visual", visual.transform);
            var motion = root.AddComponent<PickupPresentation>(); Set(motion, "visual", visual.transform); Set(motion, "halo", halo.GetComponent<Renderer>()); Set(motion, "sound", Sound(6));
            return SavePrefab(root, path);
        }
        static T Get<T>(GameObject go) where T : Component => go.GetComponent<T>() ?? go.AddComponent<T>();
        static void CreatePlayer()
        {
            string path = Root + "03_Prefabs/Players/LocalPlayer.prefab";
            var player = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Set(Get<PlayerLife>(player), "tuning", life); Set(Get<PlayerVoidFall>(player), "tuning", life); Set(Get<PlayerMelee>(player), "tuning", melee);
                Get<PowerUpManager>(player); Get<PlayerImpulseReceiver>(player); Get<LocalProjectileSpawner>(player);
                Array(player.GetComponent<WeaponController>(), "loadout", System.Array.Empty<UnityEngine.Object>());
                Set(Get<CombatHud>(player), "theme", theme); Set(Get<CombatAudio>(player), "theme", theme); Get<CameraFeedback>(player);
                var fx = Get<CombatEffects>(player); Set(fx, "sparkMaterial", glow); Set(fx, "explosionSound", Sound(8));
                var camera = player.GetComponentInChildren<Camera>();
                if (camera.transform.Find("WeaponVisualRig") == null)
                {
                    foreach (var old in player.GetComponentsInChildren<ShotgunView>(true)) old.gameObject.SetActive(false);
                    var root = new GameObject("WeaponVisualRig"); root.transform.SetParent(camera.transform, false);
                    var rig = root.AddComponent<WeaponVisualRig>();
                    var sg = (GameObject)PrefabUtility.InstantiatePrefab(shotgunVisual); sg.transform.SetParent(root.transform, false);
                    var rv = (GameObject)PrefabUtility.InstantiatePrefab(rocketVisual); rv.transform.SetParent(root.transform, false);
                    var mv = (GameObject)PrefabUtility.InstantiatePrefab(meleeVisual); mv.transform.SetParent(root.transform, false);
                    var shotgunView = root.AddComponent<ShotgunView>(); var rocketView = root.AddComponent<RocketView>();
                    shotgunView.Bind(sg.transform, sg.transform.Find("MuzzleSocket/Flash").GetComponent<Renderer>(), player.GetComponent<PlayerImpactFeedback>(), player.GetComponent<WeaponController>());
                    rocketView.Bind(rv.transform, rv.transform.Find("MuzzleSocket/Flash").GetComponent<Renderer>(), player.GetComponent<PlayerImpactFeedback>(), player.GetComponent<WeaponController>());
                    Set(rig, "shotgun", sg.transform); Set(rig, "rocket", rv.transform); Set(rig, "melee", mv.transform); Set(rig, "muzzleSocket", sg.transform.Find("MuzzleSocket"));
                }
                foreach (var readout in player.GetComponents<CombatReadout>()) readout.enabled = false;
                foreach (var readout in player.GetComponents<MovementProbeReadout>()) readout.enabled = false;
                var visualRig = player.GetComponentInChildren<WeaponVisualRig>(true);
                Set(player.GetComponent<PlayerCombat>(), "projectileMuzzle", visualRig.transform.Find("RocketProduct/MuzzleSocket"));
                foreach (Transform model in visualRig.transform)
                {
                    model.localScale = Vector3.one * (model.name.Contains("Rocket") ? .65f : .72f);
                    foreach (var renderer in model.GetComponentsInChildren<Renderer>(true)) renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                PrefabUtility.SaveAsPrefabAsset(player, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(player); }
        }
        static void CreateSession()
        {
            var root = new GameObject("LocalGameSession"); var session = root.AddComponent<LocalGameSession>();
            Array(session, "maps", Enumerable.Range(1, 4).Select(i => (UnityEngine.Object)Asset<MapDefinition>(Root + "02_ScriptableObjects/Maps/M0" + i + ".asset")).ToArray());
            Set(root.AddComponent<SessionMenu>(), "navigationSound", Sound(9));
            var loading = root.AddComponent<SessionLoadingScreen>();
            Set(loading, "steelTexture", Asset<Texture2D>(Art + "BlackSteel.png"));
            Set(loading, "oxideTexture", Asset<Texture2D>(Art + "Oxide.png"));
            sessionPrefab = SavePrefab(root, Root + "03_Prefabs/UI/LocalGameSession.prefab");
        }
        static void InstallMap(int index)
        {
            string path = Root + "00_Scenes/Maps/M0" + (index + 1) + "/" + MapNames[index] + ".unity";
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            var protectedParts = new[] { "PB_Ramp", "PB_RampLanding" }.Select(GameObject.Find).Where(x => x != null).ToArray();
            var snapshots = protectedParts.Select(x => EditorJsonUtility.ToJson(x.transform) + EditorJsonUtility.ToJson(x.GetComponent<Collider>())).ToArray();
            var root = GameObject.Find("ProductContent") ?? new GameObject("ProductContent");
            var map = Get<LocalSpawnPoints>(root);
            var points = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).Where(t => t.name.StartsWith("Spawn_Human_")).OrderBy(t => t.name).ToArray();
            Array(map, "points", points); Set(map, "voidHeight", index == 3 ? -4f : -8f);
            var player = GameObject.Find("LocalPlayer");
            var lives = player.GetComponents<PlayerLife>();
            if (lives.Length > 1)
            {
                foreach (var duplicate in lives.Skip(1))
                {
                    var spawn = new SerializedObject(duplicate).FindProperty("spawnPoint").objectReferenceValue;
                    if (spawn != null) Set(lives[0], "spawnPoint", spawn);
                    UnityEngine.Object.DestroyImmediate(duplicate);
                }
            }
            if (new SerializedObject(Get<PlayerLife>(player)).FindProperty("spawnPoint").objectReferenceValue == null && points.Length > 0)
                Set(Get<PlayerLife>(player), "spawnPoint", points[0]);
            Set(Get<PlayerLife>(player), "map", map); Set(Get<PlayerLife>(player), "tuning", life);
            Set(Get<PlayerVoidFall>(player), "map", map); Set(Get<PlayerVoidFall>(player), "tuning", life);
            Set(Get<PlayerMelee>(player), "tuning", melee);
            Set(Get<GameBootstrap>(root), "sessionPrefab", sessionPrefab);
            foreach (var pickup in UnityEngine.Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None)) UpgradePickup(pickup);
            for (int i = 0; i < points.Length; i++)
            {
                PlacePickup(root.transform, "Supply_Shotgun_" + i, "Shotgun_Weapon", points[i].position + new Vector3(2, .4f, 2));
                PlacePickup(root.transform, "Supply_Shells_" + i, "Shell_Ammo", points[i].position + new Vector3(-2, .4f, 2));
            }
            if (points.Length > 0)
            {
                PlacePickup(root.transform, "Supply_Rocket", "Rocket_Weapon", points[0].position + new Vector3(5, .4f, 4));
                PlacePickup(root.transform, "Supply_Rockets", "Rocket_AmmoOnly", points[0].position + new Vector3(7, .4f, 4));
            }
            ProductMapPresentation.Dress(index, root.transform, points, dark, steel, copper, glow);
            var camera = player.GetComponentInChildren<Camera>(true);
            if (camera != null) { camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = RenderSettings.fogColor; }
            ProductRoutes.Install(index, root.transform, dark, copper, glow);
            for (int i = 0; i < protectedParts.Length; i++)
                if (snapshots[i] != EditorJsonUtility.ToJson(protectedParts[i].transform) + EditorJsonUtility.ToJson(protectedParts[i].GetComponent<Collider>())) throw new InvalidOperationException("Protected ramp geometry changed.");
            EditorSceneManager.SaveScene(scene);
        }
        static void UpgradePickup(PickupController pickup)
        {
            if (pickup.Definition == null)
            {
                string key = pickup.name.Contains("Health") && !pickup.name.Contains("Mega") ? "Health_Small" :
                    pickup.name.Contains("Armor") ? "Armor_Small" : pickup.name.Contains("Quad") ? "Quad" :
                    pickup.name.Contains("Berserk") ? "Berserk" : pickup.name.Contains("Rockets") ? "Rocket_Weapon" :
                    pickup.name.Contains("Invulnerability") ? "Invulnerability" : pickup.name.Contains("Haste") ? "Haste" :
                    pickup.name.Contains("Regeneration") ? "Regeneration" : pickup.name.Contains("Megahealth") ? "Megahealth" : null;
                if (key == null) throw new InvalidOperationException("Unrecognized pickup role: " + pickup.name);
                Set(pickup, "definition", Asset<PickupDefinition>(Root + "02_ScriptableObjects/Pickups/" + key + ".asset"));
                Debug.Log("[Product] Repaired missing definition: " + pickup.name + " -> " + key);
            }
            var existingVisual = pickup.transform.Find("ProductVisual");
            if (existingVisual != null)
            {
                if (pickup.Definition.Type == PickupType.PowerUp)
                    foreach (var sprite in existingVisual.GetComponentsInChildren<SpriteRenderer>()) sprite.sprite = Icon(pickup.Definition.PowerUpType);
                return;
            }
            var data = new SerializedObject(pickup); var old = data.FindProperty("visual").objectReferenceValue as Transform;
            if (old != null && old != pickup.transform) old.gameObject.SetActive(false);
            var source = Asset<GameObject>(Root + "03_Prefabs/Pickups/" + pickup.Definition.name + ".prefab");
            var visual = UnityEngine.Object.Instantiate(source.transform.Find("Visual").gameObject, pickup.transform); visual.name = "ProductVisual";
            Set(pickup, "visual", visual.transform);
            var presentation = Get<PickupPresentation>(pickup.gameObject); Set(presentation, "visual", visual.transform); Set(presentation, "sound", Sound(6));
        }
        static void PlacePickup(Transform parent, string name, string definition, Vector3 position)
        {
            if (parent.Find(name) != null) return;
            var prefab = Asset<GameObject>(Root + "03_Prefabs/Pickups/" + definition + ".prefab");
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab); instance.name = name; instance.transform.SetParent(parent, false); instance.transform.position = position;
        }
        static void CreateBoot()
        {
            var scene = File.Exists(BootScenePath) ? EditorSceneManager.OpenScene(BootScenePath) : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = GameObject.Find("Boot") ?? new GameObject("Boot"); Set(Get<GameBootstrap>(root), "sessionPrefab", sessionPrefab);
            if (GameObject.Find("MenuCamera") == null)
            {
                var camera = new GameObject("MenuCamera", typeof(Camera), typeof(AudioListener)); camera.transform.position = new Vector3(6, 3, -10); camera.transform.LookAt(new Vector3(0, 2, 3));
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor; camera.GetComponent<Camera>().backgroundColor = new Color(.025f, .03f, .04f);
                Part(root.transform, "Floor", PrimitiveType.Cube, new Vector3(0, -.2f, 0), new Vector3(40, .4f, 40), dark);
                for (int i = 0; i < 7; i++) { Part(root.transform, "IndustrialColumn" + i, PrimitiveType.Cube, new Vector3(-10 + i * 4, 3, 8), new Vector3(.6f, 6, .8f), copper); Part(root.transform, "LightStrip" + i, PrimitiveType.Cube, new Vector3(-10 + i * 4, 3, 7.5f), new Vector3(.1f, 4, .08f), glow); }
                var lamp = new GameObject("MenuLight", typeof(Light)); lamp.transform.rotation = Quaternion.Euler(40, -30, 0); lamp.GetComponent<Light>().type = LightType.Directional; lamp.GetComponent<Light>().intensity = 1.2f;
            }
            EditorSceneManager.SaveScene(scene, BootScenePath);
        }
        static void CreateStartup()
        {
            var scene = File.Exists(StartupScenePath) ? EditorSceneManager.OpenScene(StartupScenePath) : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = GameObject.Find("Startup") ?? new GameObject("Startup");
            var camera = Get<Camera>(root);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.025f, .03f, .04f);
            camera.cullingMask = 0;
            Get<AudioListener>(root);
            var bootstrap = Get<GameBootstrap>(root);
            Set(bootstrap, "sessionPrefab", sessionPrefab);
            Set(bootstrap, "loadMenuOnStart", true);
            EditorSceneManager.SaveScene(scene, StartupScenePath);

            var scenes = EditorBuildSettings.scenes.Where(s => s.path != StartupScenePath && s.path != BootScenePath).ToList();
            scenes.Insert(0, new EditorBuildSettingsScene(StartupScenePath, true));
            scenes.Add(new EditorBuildSettingsScene(BootScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
        public static void Capture() => ProductMapPresentation.CaptureMaps();
        public static void CaptureAndBuild()
        {
            ProductVerificationRunner.InspectModels();
            Capture();
            BuildWindows();
        }
        public static void BuildWindows()
        {
            string folder = "Builds/LocalProduct"; Directory.CreateDirectory(folder);
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled && (s.path == StartupScenePath || s.path == BootScenePath || s.path.Contains("/Maps/M0")))
                    .OrderBy(s => s.path != StartupScenePath).Select(s => s.path).ToArray(),
                locationPathName = folder + "/MEATGRINDER.exe", target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
            });
            Directory.CreateDirectory(ProductVerificationRunner.Output);
            File.WriteAllText(Path.Combine(ProductVerificationRunner.Output, "windows-build.txt"), report.summary.result + " errors=" + report.summary.totalErrors + " size=" + report.summary.totalSize);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) throw new InvalidOperationException("Windows build failed.");
        }
    }
}
