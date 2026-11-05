using System.IO;
using Meatgrinder.Combat;
using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Weapons;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    [InitializeOnLoad]
    public static class PresentationGreyboxBuilder
    {
        const string RequestPath = "Logs/presentation-request.txt";

        static PresentationGreyboxBuilder()
        {
            if (File.Exists(RequestPath) == false)
            {
                return;
            }

            EditorApplication.delayCall += RunRequested;
        }

        static void RunRequested()
        {
            if (File.Exists(RequestPath) == false)
            {
                return;
            }

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RunRequested;
                return;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            File.Delete(RequestPath);
            Dress();
        }

        const string ScenePath = "Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity";
        const string PrefabPath = "Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab";
        const string ViewPath = "Assets/_Project/03_Prefabs/Weapons/ShotgunView.prefab";
        const string SourceMaterial = "Assets/_Project/05_ArtModels/Materials/Greybox.mat";
        const string MaterialDir = "Assets/_Project/05_ArtModels/Materials/Presentation";

        [MenuItem("Meatgrinder/Presentation/Dress Greybox")]
        public static void Dress()
        {
            if (File.Exists(ScenePath) == false || File.Exists(PrefabPath) == false || File.Exists(SourceMaterial) == false)
            {
                Debug.LogError("[Presentation] Greybox, LocalPlayer or the base material is missing.");
                ExitBatch(1);
                return;
            }

            Directory.CreateDirectory(MaterialDir);
            Directory.CreateDirectory("Assets/_Project/03_Prefabs/Weapons");
            Material metal = Make("GunMetal", new Color(0.22f, 0.23f, 0.26f, 1f));
            Material wood = Make("GunWood", new Color(0.28f, 0.15f, 0.08f, 1f));
            Material flash = Make("MuzzleFlash", new Color(1f, 0.78f, 0.25f, 1f));
            Material floor = Make("Floor", new Color(0.11f, 0.11f, 0.12f, 1f));
            Material wall = Make("Wall", new Color(0.24f, 0.22f, 0.2f, 1f));
            Material trim = Make("Trim", new Color(0.55f, 0.12f, 0.1f, 1f));
            Material mark = Make("LaneMark", new Color(0.78f, 0.58f, 0.12f, 1f));
            Material close = Make("TargetClose", new Color(0.82f, 0.42f, 0.12f, 1f));
            Material far = Make("TargetFar", new Color(0.72f, 0.66f, 0.22f, 1f));
            Material tough = Make("TargetTough", new Color(0.42f, 0.08f, 0.08f, 1f));
            Material pad = Make("Pad", new Color(0.16f, 0.16f, 0.18f, 1f));
            BuildViewPrefab(metal, wood, flash);
            DressPlayer();
            DressScene(floor, wall, trim, mark, pad, close, far, tough);
            AssetDatabase.SaveAssets();
            Debug.Log("[Presentation] Greybox dressed.");
            ExitBatch(0);
        }

        static Material Make(string name, Color color)
        {
            string path = MaterialDir + "/" + name + ".mat";
            if (File.Exists(path) == false)
            {
                AssetDatabase.CopyAsset(SourceMaterial, path);
            }

            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            material.name = name;
            material.SetColor("_BaseColor", color);
            material.SetColor("_Color", color);
            EditorUtility.SetDirty(material);
            return material;
        }

        static void BuildViewPrefab(Material metal, Material wood, Material flash)
        {
            GameObject root = new GameObject("ShotgunView");
            GameObject model = new GameObject("Model");
            model.transform.SetParent(root.transform, false);
            Part(model.transform, "Stock", wood, new Vector3(0.02f, -0.03f, -0.16f), new Vector3(0.07f, 0.1f, 0.24f), Quaternion.Euler(-14f, 0f, 0f));
            Part(model.transform, "Receiver", metal, new Vector3(0f, 0.01f, 0.02f), new Vector3(0.06f, 0.07f, 0.18f), Quaternion.identity);
            Part(model.transform, "Grip", wood, new Vector3(0f, -0.08f, -0.02f), new Vector3(0.045f, 0.12f, 0.05f), Quaternion.Euler(18f, 0f, 0f));
            Part(model.transform, "Guard", metal, new Vector3(0f, -0.04f, 0.05f), new Vector3(0.05f, 0.07f, 0.04f), Quaternion.identity);
            Part(model.transform, "Pump", wood, new Vector3(0f, 0.02f, 0.14f), new Vector3(0.055f, 0.05f, 0.11f), Quaternion.identity);
            Part(model.transform, "Barrel", metal, new Vector3(0f, 0.035f, 0.28f), new Vector3(0.03f, 0.03f, 0.36f), Quaternion.identity);
            GameObject muzzle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            muzzle.name = "MuzzleFlash";
            Object.DestroyImmediate(muzzle.GetComponent<Collider>());
            muzzle.transform.SetParent(model.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0.035f, 0.48f);
            muzzle.transform.localScale = Vector3.one * 0.05f;
            Renderer flashRenderer = muzzle.GetComponent<Renderer>();
            flashRenderer.sharedMaterial = flash;
            flashRenderer.enabled = false;
            ShotgunView view = root.AddComponent<ShotgunView>();
            SerializedObject data = new SerializedObject(view);
            data.FindProperty("model").objectReferenceValue = model.transform;
            data.FindProperty("muzzleFlash").objectReferenceValue = flashRenderer;
            data.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, ViewPath);
            Object.DestroyImmediate(root);
        }

        static void Part(Transform parent, string name, Material material, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            Object.DestroyImmediate(part.GetComponent<Collider>());
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localRotation = rotation;
            part.transform.localScale = scale;
            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        static void DressPlayer()
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            Camera camera = root.GetComponentInChildren<Camera>(true);
            PlayerImpactFeedback feedback = root.GetComponent<PlayerImpactFeedback>();
            WeaponController weapons = root.GetComponent<WeaponController>();
            CombatReadout readout = root.GetComponent<CombatReadout>();
            if (readout != null)
            {
                readout.enabled = false;
            }

            Transform old = camera != null ? camera.transform.Find("ShotgunView") : null;
            if (old != null)
            {
                Object.DestroyImmediate(old.gameObject);
            }

            GameObject view = camera != null
                ? (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(ViewPath), camera.transform)
                : null;
            if (view != null)
            {
                view.transform.localPosition = Vector3.zero;
                view.transform.localRotation = Quaternion.identity;
                ShotgunView component = view.GetComponent<ShotgunView>();
                SerializedObject data = new SerializedObject(component);
                data.FindProperty("feedback").objectReferenceValue = feedback;
                data.FindProperty("weapons").objectReferenceValue = weapons;
                data.ApplyModifiedPropertiesWithoutUndo();
            }

            CombatHud hud = root.GetComponent<CombatHud>();
            if (hud == null)
            {
                hud = root.AddComponent<CombatHud>();
            }

            SerializedObject hudData = new SerializedObject(hud);
            hudData.FindProperty("weapons").objectReferenceValue = weapons;
            hudData.FindProperty("health").objectReferenceValue = root.GetComponent<HealthComponent>();
            hudData.FindProperty("feedback").objectReferenceValue = feedback;
            hudData.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        static void DressScene(
            Material floor,
            Material wall,
            Material trim,
            Material mark,
            Material pad,
            Material close,
            Material far,
            Material tough)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Paint("Floor", floor);
            Paint("Wall_East", wall);
            Paint("Wall_West", wall);
            Paint("Wall_North", wall);
            Paint("Wall_South", wall);
            Paint("Wall_Lane", trim);
            Paint("Ramp", trim);
            Paint("Step_Low", pad);
            Paint("Step_Blocked", pad);
            Paint("Platform_Low", pad);
            Paint("Platform_High", pad);
            Paint("Wall_ShotgunBlock", trim);
            DressTarget("Shotgun_Close", "Cercano", close, false);
            DressTarget("Shotgun_Far", "Lejano", far, false);
            DressTarget("Shotgun_Blocked", "Cobertura", close, false);
            DressTarget("Target_Tough", "Resistente", tough, true);
            GameObject previous = GameObject.Find("ArenaDress");
            if (previous != null)
            {
                Object.DestroyImmediate(previous);
            }

            GameObject dress = new GameObject("ArenaDress");
            Transform parent = dress.transform;
            Decal(parent, "Lane_A", new Vector3(16f, 0.02f, -8f), new Vector3(0.12f, 0.01f, 28f), mark);
            Decal(parent, "Lane_B", new Vector3(20f, 0.02f, 8f), new Vector3(0.12f, 0.01f, 22f), mark);
            Decal(parent, "Range_10", new Vector3(14f, 0.02f, -8f), new Vector3(2.4f, 0.01f, 0.08f), mark);
            Decal(parent, "Range_20", new Vector3(14f, 0.02f, 2f), new Vector3(2.4f, 0.01f, 0.08f), mark);
            Decal(parent, "Range_30", new Vector3(14f, 0.02f, 12f), new Vector3(2.4f, 0.01f, 0.08f), mark);
            Pad(parent, "Pad_Close", new Vector3(18f, 0.02f, -6f), pad);
            Pad(parent, "Pad_Far", new Vector3(18f, 0.02f, 20f), pad);
            Pad(parent, "Pad_Blocked", new Vector3(22f, 0.02f, 6f), pad);
            Pad(parent, "Pad_Tough", new Vector3(18f, 0.02f, -12f), pad);
            Sign(parent, "Sign_Move", "1  MOVIMIENTO", new Vector3(0f, 2.2f, -12f), new Color(0.85f, 0.85f, 0.8f, 1f));
            Sign(parent, "Sign_Close", "2  CERCA", new Vector3(14f, 2.4f, -6f), new Color(0.9f, 0.5f, 0.15f, 1f));
            Sign(parent, "Sign_Far", "3  LEJOS", new Vector3(14f, 2.4f, 20f), new Color(0.9f, 0.8f, 0.3f, 1f));
            Sign(parent, "Sign_Cover", "4  COBERTURA", new Vector3(22f, 2.6f, 0f), new Color(0.55f, 0.75f, 0.9f, 1f));
            Sign(parent, "Sign_Tough", "5  RESISTENTE", new Vector3(14f, 2.4f, -12f), new Color(0.85f, 0.25f, 0.2f, 1f));
            Light lamp = Lamp(parent, "Lamp_Red", new Vector3(8f, 4.5f, -4f), new Color(0.8f, 0.18f, 0.1f, 1f));
            Light fill = Lamp(parent, "Lamp_Fill", new Vector3(4f, 5f, 16f), new Color(0.75f, 0.7f, 0.6f, 1f));
            lamp.intensity = 3.5f;
            fill.intensity = 2.2f;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        static void Paint(string name, Material material)
        {
            GameObject target = GameObject.Find(name);
            if (target == null)
            {
                return;
            }

            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        static void DressTarget(string name, string title, Material material, bool bulky)
        {
            GameObject target = GameObject.Find(name);
            if (target == null)
            {
                return;
            }

            CombatTargetLabel legacy = target.GetComponent<CombatTargetLabel>();
            if (legacy != null)
            {
                legacy.enabled = false;
            }

            TargetBillboard board = target.GetComponent<TargetBillboard>();
            if (board == null)
            {
                board = target.AddComponent<TargetBillboard>();
            }

            SerializedObject data = new SerializedObject(board);
            data.FindProperty("health").objectReferenceValue = target.GetComponent<HealthComponent>();
            data.FindProperty("displayName").stringValue = title;
            data.FindProperty("bodyMaterial").objectReferenceValue = material;
            data.FindProperty("bodyColor").colorValue = material.GetColor("_BaseColor");
            data.FindProperty("bulky").boolValue = bulky;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Decal(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = name;
            Object.DestroyImmediate(slab.GetComponent<Collider>());
            slab.transform.SetParent(parent, true);
            slab.transform.position = position;
            slab.transform.localScale = scale;
            slab.GetComponent<Renderer>().sharedMaterial = material;
        }

        static void Pad(Transform parent, string name, Vector3 position, Material material)
        {
            Decal(parent, name, position, new Vector3(1.8f, 0.02f, 1.8f), material);
        }

        static void Sign(Transform parent, string title, string text, Vector3 position, Color color)
        {
            GameObject sign = new GameObject(title);
            sign.transform.SetParent(parent, true);
            sign.transform.position = position;
            ZoneSign zone = sign.AddComponent<ZoneSign>();
            SerializedObject data = new SerializedObject(zone);
            data.FindProperty("title").stringValue = text;
            data.FindProperty("color").colorValue = color;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static Light Lamp(Transform parent, string name, Vector3 position, Color color)
        {
            GameObject lamp = new GameObject(name);
            lamp.transform.SetParent(parent, true);
            lamp.transform.position = position;
            Light light = lamp.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = 18f;
            light.shadows = LightShadows.None;
            return light;
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
