using System.IO;
using Meatgrinder.Combat;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class CombatGreyboxBuilder
    {
        const string ScenePath = "Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity";
        const string PrefabPath = "Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab";
        const string RocketPath = "Assets/_Project/03_Prefabs/Weapons/Rocket.prefab";
        const string ShotgunPath = "Assets/_Project/02_ScriptableObjects/Weapons/Shotgun.asset";
        const string RocketWeaponPath = "Assets/_Project/02_ScriptableObjects/Weapons/RocketLauncher.asset";
        const string HumanPath = "Assets/_Project/02_ScriptableObjects/Combat/HealthHuman.asset";
        const string FragilePath = "Assets/_Project/02_ScriptableObjects/Combat/HealthFragile.asset";
        const string StandardPath = "Assets/_Project/02_ScriptableObjects/Combat/HealthStandard.asset";
        const string ToughPath = "Assets/_Project/02_ScriptableObjects/Combat/HealthTough.asset";
        const string MaterialPath = "Assets/_Project/05_ArtModels/Materials/Greybox.mat";

        [MenuItem("Meatgrinder/Combat/Install Greybox Targets")]
        public static void Install()
        {
            if (File.Exists(ScenePath) == false || File.Exists(PrefabPath) == false)
            {
                Debug.LogError("[Combat] Greybox scene or LocalPlayer prefab is missing.");
                ExitBatch(1);
                return;
            }

            Directory.CreateDirectory("Assets/_Project/02_ScriptableObjects/Weapons");
            Directory.CreateDirectory("Assets/_Project/02_ScriptableObjects/Combat");
            Directory.CreateDirectory("Assets/_Project/03_Prefabs/Weapons");

            HealthProfile human = Profile(HumanPath, 100f);
            HealthProfile fragile = Profile(FragilePath, 40f);
            HealthProfile standard = Profile(StandardPath, 80f);
            HealthProfile tough = Profile(ToughPath, 200f);
            WeaponDefinition shotgun = Shotgun();
            WeaponDefinition rocket = RocketWeapon();
            GameObject rocketPrefab = RocketPrefab();
            AssignPrefab(rocket, rocketPrefab);
            InstallPlayer(human, shotgun, rocket);
            InstallScene(fragile, standard, tough);
            AssetDatabase.SaveAssets();
            Debug.Log("[Combat] Greybox targets ready.");
            ExitBatch(0);
        }

        static HealthProfile Profile(string path, float maxHealth)
        {
            HealthProfile profile = AssetDatabase.LoadAssetAtPath<HealthProfile>(path);
            if (profile != null)
            {
                return profile;
            }

            profile = ScriptableObject.CreateInstance<HealthProfile>();
            AssetDatabase.CreateAsset(profile, path);
            SerializedObject data = new SerializedObject(profile);
            data.FindProperty("maxHealth").floatValue = maxHealth;
            data.ApplyModifiedPropertiesWithoutUndo();
            return profile;
        }

        static WeaponDefinition Shotgun()
        {
            WeaponDefinition weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(ShotgunPath);
            if (weapon != null)
            {
                return weapon;
            }

            weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            AssetDatabase.CreateAsset(weapon, ShotgunPath);
            SerializedObject data = new SerializedObject(weapon);
            data.FindProperty("weaponId").stringValue = "shotgun";
            data.FindProperty("displayName").stringValue = "Escopeta";
            data.FindProperty("delivery").enumValueIndex = (int)WeaponDelivery.Hitscan;
            data.FindProperty("damage").floatValue = 12f;
            data.FindProperty("pelletCount").intValue = 8;
            data.FindProperty("spreadDegrees").floatValue = 5f;
            data.FindProperty("maxRange").floatValue = 36f;
            data.FindProperty("falloffStart").floatValue = 6f;
            data.FindProperty("falloffEnd").floatValue = 28f;
            data.FindProperty("minimumDamageScale").floatValue = 0.3f;
            data.FindProperty("fireInterval").floatValue = 0.7f;
            data.FindProperty("magazineSize").intValue = 6;
            data.FindProperty("startingLoaded").intValue = 6;
            data.FindProperty("startingReserve").intValue = 24;
            data.FindProperty("ammoPerShot").intValue = 1;
            data.FindProperty("reloadDuration").floatValue = 1.4f;
            data.FindProperty("recoilPitch").floatValue = 1.5f;
            data.ApplyModifiedPropertiesWithoutUndo();
            return weapon;
        }

        static WeaponDefinition RocketWeapon()
        {
            WeaponDefinition weapon = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(RocketWeaponPath);
            if (weapon != null)
            {
                return weapon;
            }

            weapon = ScriptableObject.CreateInstance<WeaponDefinition>();
            AssetDatabase.CreateAsset(weapon, RocketWeaponPath);
            SerializedObject data = new SerializedObject(weapon);
            data.FindProperty("weaponId").stringValue = "rocket";
            data.FindProperty("displayName").stringValue = "Lanzacohetes";
            data.FindProperty("delivery").enumValueIndex = (int)WeaponDelivery.Projectile;
            data.FindProperty("pelletCount").intValue = 1;
            data.FindProperty("fireInterval").floatValue = 0.9f;
            data.FindProperty("magazineSize").intValue = 4;
            data.FindProperty("startingLoaded").intValue = 4;
            data.FindProperty("startingReserve").intValue = 12;
            data.FindProperty("ammoPerShot").intValue = 1;
            data.FindProperty("reloadDuration").floatValue = 1.8f;
            data.FindProperty("projectileSpeed").floatValue = 24f;
            data.FindProperty("projectileRadius").floatValue = 0.18f;
            data.FindProperty("projectileLifetime").floatValue = 4f;
            data.FindProperty("blastRadius").floatValue = 4f;
            data.FindProperty("blastDamage").floatValue = 90f;
            data.FindProperty("blastEdgeScale").floatValue = 0.2f;
            data.FindProperty("recoilPitch").floatValue = 2f;
            data.ApplyModifiedPropertiesWithoutUndo();
            return weapon;
        }

        static void AssignPrefab(WeaponDefinition weapon, GameObject prefab)
        {
            SerializedObject data = new SerializedObject(weapon);
            data.FindProperty("projectilePrefab").objectReferenceValue = prefab;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static GameObject RocketPrefab()
        {
            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(RocketPath);
            if (existing != null)
            {
                return existing;
            }

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Rocket";
            body.transform.localScale = Vector3.one * 0.36f;
            body.AddComponent<RocketProjectile>();
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(body, RocketPath);
            Object.DestroyImmediate(body);
            return prefab;
        }

        static void InstallPlayer(HealthProfile human, WeaponDefinition shotgun, WeaponDefinition rocket)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            HealthComponent health = GetOrAdd<HealthComponent>(root);
            WeaponController weapons = GetOrAdd<WeaponController>(root);
            DamageResolverHost resolver = GetOrAdd<DamageResolverHost>(root);
            LocalCombatAuthority authority = GetOrAdd<LocalCombatAuthority>(root);
            PlayerImpactFeedback feedback = GetOrAdd<PlayerImpactFeedback>(root);
            LocalProjectileSpawner spawner = GetOrAdd<LocalProjectileSpawner>(root);
            PlayerCombat combat = GetOrAdd<PlayerCombat>(root);
            CombatReadout readout = GetOrAdd<CombatReadout>(root);
            PlayerInputReader input = root.GetComponent<PlayerInputReader>();
            PlayerLook look = root.GetComponent<PlayerLook>();

            Set(health, "profile", human);
            Set(health, "teamId", 1);
            Set(feedback, "look", look);
            SerializedObject weaponObject = new SerializedObject(weapons);
            SerializedProperty loadout = weaponObject.FindProperty("loadout");
            loadout.arraySize = 2;
            loadout.GetArrayElementAtIndex(0).objectReferenceValue = shotgun;
            loadout.GetArrayElementAtIndex(1).objectReferenceValue = rocket;
            weaponObject.FindProperty("activeIndex").intValue = 0;
            weaponObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject combatObject = new SerializedObject(combat);
            combatObject.FindProperty("input").objectReferenceValue = input;
            combatObject.FindProperty("look").objectReferenceValue = look;
            combatObject.FindProperty("weapons").objectReferenceValue = weapons;
            combatObject.FindProperty("damage").objectReferenceValue = resolver;
            combatObject.FindProperty("authority").objectReferenceValue = authority;
            combatObject.FindProperty("feedback").objectReferenceValue = feedback;
            combatObject.FindProperty("projectiles").objectReferenceValue = spawner;
            combatObject.FindProperty("health").objectReferenceValue = health;
            combatObject.ApplyModifiedPropertiesWithoutUndo();
            Set(readout, "weapons", weapons);
            Set(readout, "feedback", feedback);
            Set(readout, "health", health);
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        static void InstallScene(HealthProfile fragile, HealthProfile standard, HealthProfile tough)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject previous = GameObject.Find("CombatRange");
            if (previous != null)
            {
                Object.DestroyImmediate(previous);
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            GameObject range = new GameObject("CombatRange");
            Transform parent = range.transform;
            Target(parent, "Target_Tough", new Vector3(18f, 1f, -12f), Vector3.one * 1.2f, tough, material);
            Target(parent, "Shotgun_Close", new Vector3(18f, 1f, -6f), new Vector3(1f, 2f, 1f), fragile, material);
            Target(parent, "Shotgun_Far", new Vector3(18f, 1f, 20f), new Vector3(1f, 2f, 1f), fragile, material);
            Wall(parent, "Wall_ShotgunBlock", new Vector3(22f, 1.25f, 0f), new Vector3(1f, 2.5f, 4f), material);
            Target(parent, "Shotgun_Blocked", new Vector3(22f, 1f, 6f), new Vector3(1f, 2f, 1f), standard, material);
            Target(parent, "Rocket_Open", new Vector3(12f, 1f, 2f), new Vector3(1f, 2f, 1f), standard, material);
            Wall(parent, "Wall_RocketCover", new Vector3(12f, 1.25f, 10f), new Vector3(5f, 2.5f, 1f), material);
            Target(parent, "Rocket_Covered", new Vector3(12f, 1f, 13f), new Vector3(1f, 2f, 1f), fragile, material);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void Target(Transform parent, string name, Vector3 position, Vector3 scale, HealthProfile profile, Material material)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = name;
            body.transform.SetParent(parent, true);
            body.transform.position = position;
            body.transform.localScale = scale;
            HealthComponent health = body.AddComponent<HealthComponent>();
            CombatTargetLabel label = body.AddComponent<CombatTargetLabel>();
            Set(health, "profile", profile);
            Set(health, "teamId", 0);
            Set(label, "health", health);
            Assign(body, material);
        }

        static void Wall(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = name;
            body.transform.SetParent(parent, true);
            body.transform.position = position;
            body.transform.localScale = scale;
            Assign(body, material);
        }

        static void Assign(GameObject target, Material material)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        static T GetOrAdd<T>(GameObject root) where T : Component
        {
            T component = root.GetComponent<T>();
            return component != null ? component : root.AddComponent<T>();
        }

        static void Set(Object target, string property, Object value)
        {
            SerializedObject data = new SerializedObject(target);
            data.FindProperty(property).objectReferenceValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Set(Object target, string property, int value)
        {
            SerializedObject data = new SerializedObject(target);
            data.FindProperty(property).intValue = value;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static void ExitBatch(int code)
        {
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(code);
            }
        }
    }
}
