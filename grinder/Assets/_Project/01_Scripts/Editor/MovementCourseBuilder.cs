using Meatgrinder.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class MovementCourseBuilder
    {
        const string ScenePath = "Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity";

        [MenuItem("Meatgrinder/Movement/Install Greybox Course")]
        public static void Install()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Vector3 close = Position("Shotgun_Close");
            Vector3 far = Position("Shotgun_Far");
            Vector3 blocked = Position("Shotgun_Blocked");
            Vector3 tough = Position("Target_Tough");
            Vector3 cover = Position("Wall_ShotgunBlock");
            GameObject previous = GameObject.Find("MovementCourse");
            if (previous != null)
            {
                Object.DestroyImmediate(previous);
            }

            Material trim = Load("Assets/_Project/05_ArtModels/Materials/Presentation/Trim.mat");
            Material mark = Load("Assets/_Project/05_ArtModels/Materials/Presentation/LaneMark.mat");
            Material pad = Load("Assets/_Project/05_ArtModels/Materials/Presentation/Pad.mat");
            GameObject root = new GameObject("MovementCourse");
            Transform parent = root.transform;
            Solid(parent, "Course_Wall_Inner", new Vector3(-16.5f, 0.7f, -4f), new Vector3(0.4f, 1.4f, 32f), trim);
            Solid(parent, "Course_Wall_Outer", new Vector3(-27f, 0.7f, -4f), new Vector3(0.4f, 1.4f, 32f), trim);
            Solid(parent, "Course_Ramp", new Vector3(-24f, 1.64f, -28f), new Vector3(3f, 0.5f, 12f), trim, Quaternion.Euler(-18f, 0f, 0f));
            Solid(parent, "Course_Step_A", new Vector3(-26f, 0.15f, 16f), new Vector3(3f, 0.3f, 3f), pad);
            Solid(parent, "Course_Step_B", new Vector3(-26f, 0.45f, 19.2f), new Vector3(3f, 0.3f, 3f), pad);
            Solid(parent, "Course_Platform_A", new Vector3(-20f, 0.45f, 24f), new Vector3(2.4f, 0.3f, 2.4f), pad);
            Solid(parent, "Course_Platform_B", new Vector3(-20f, 0.75f, 27.6f), new Vector3(2.4f, 0.3f, 2.4f), pad);
            for (int i = 0; i < 8; i++)
            {
                Decal(parent, "Course_Tick_" + i, new Vector3(-21.8f, 0.02f, -18f + (i * 4f)), new Vector3(2.2f, 0.01f, 0.08f), mark);
            }

            Decal(parent, "Course_Straight", new Vector3(-21.8f, 0.02f, -2f), new Vector3(0.12f, 0.01f, 28f), mark);
            Decal(parent, "Course_Hop", new Vector3(-13.2f, 0.02f, -4f), new Vector3(0.12f, 0.01f, 24f), mark);
            Sign(parent, "Sign_Course", "PISTA", new Vector3(-8f, 2.2f, -16f));
            Sign(parent, "Sign_Straight", "RECTA", new Vector3(-21.8f, 2.2f, -18f));
            Sign(parent, "Sign_Hop", "SALTOS", new Vector3(-13.2f, 2.2f, -12f));
            Sign(parent, "Sign_Strafe", "STRAFE", new Vector3(-21.8f, 2.2f, 12f));
            Sign(parent, "Sign_Ramp", "RAMPA", new Vector3(-24f, 2.8f, -30f));
            Sign(parent, "Sign_Steps", "ESCALONES", new Vector3(-26f, 2.4f, 14f));
            Sign(parent, "Sign_Platforms", "PLATAFORMAS", new Vector3(-20f, 2.6f, 22f));
            if (Moved("Shotgun_Close", close) || Moved("Shotgun_Far", far) || Moved("Shotgun_Blocked", blocked) || Moved("Target_Tough", tough) || Moved("Wall_ShotgunBlock", cover))
            {
                Debug.LogError("[Movement] The course moved a combat target.");
                EditorSceneManager.SaveScene(scene);
                ExitBatch(1);
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Movement] Greybox course installed.");
            ExitBatch(0);
        }

        static Vector3 Position(string name)
        {
            GameObject target = GameObject.Find(name);
            return target != null ? target.transform.position : new Vector3(float.NaN, float.NaN, float.NaN);
        }

        static bool Moved(string name, Vector3 before)
        {
            GameObject target = GameObject.Find(name);
            if (target == null || float.IsNaN(before.x))
            {
                return true;
            }

            return (target.transform.position - before).sqrMagnitude > 0.0001f;
        }

        static Material Load(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(path);
        }

        static void Solid(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            Solid(parent, name, position, scale, material, Quaternion.identity);
        }

        static void Solid(Transform parent, string name, Vector3 position, Vector3 scale, Material material, Quaternion rotation)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = name;
            body.transform.SetParent(parent, true);
            body.transform.position = position;
            body.transform.rotation = rotation;
            body.transform.localScale = scale;
            Paint(body, material);
        }

        static void Decal(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = name;
            Object.DestroyImmediate(slab.GetComponent<Collider>());
            slab.transform.SetParent(parent, true);
            slab.transform.position = position;
            slab.transform.localScale = scale;
            Paint(slab, material);
        }

        static void Paint(GameObject body, Material material)
        {
            if (material == null)
            {
                return;
            }

            Renderer renderer = body.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        static void Sign(Transform parent, string name, string text, Vector3 position)
        {
            GameObject sign = new GameObject(name);
            sign.transform.SetParent(parent, true);
            sign.transform.position = position;
            ZoneSign zone = sign.AddComponent<ZoneSign>();
            SerializedObject data = new SerializedObject(zone);
            data.FindProperty("title").stringValue = text;
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
