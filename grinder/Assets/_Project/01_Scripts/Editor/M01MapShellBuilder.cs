using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class M01MapShellBuilder
    {
        const string ScenePath = "Assets/_Project/00_Scenes/Maps/M01/M01_TheSlaughterhouse.unity";
        const string PlayerPath = "Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab";
        const string MaterialAssetPath = "Assets/_Project/00_Scenes/Maps/M01/M01_Blockout.mat";
        const string FallbackMaterialPath = "Assets/_Project/05_ArtModels/Materials/Greybox.mat";

        [MenuItem("Meatgrinder/Maps/Create M01 Shell")]
        public static void Create()
        {
            if (File.Exists(ScenePath))
            {
                Debug.LogError("[M01] Create M01 Shell refused. The scene already exists and holds the ramp that was walked by hand. Expand it with Meatgrinder/Maps/Expand M01.");
                ExitBatch(2);
                return;
            }

            Directory.CreateDirectory("Assets/_Project/00_Scenes/Maps/M01");
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPath);
            if (playerPrefab == null)
            {
                Debug.LogError("[M01] Missing " + PlayerPath);
                ExitBatch(1);
                return;
            }

            Material material = ResolveMaterial();
            if (material == null || IsErrorShader(material))
            {
                Debug.LogError("[M01] No usable URP material.");
                ExitBatch(1);
                return;
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Camera leftover = Camera.main;
            if (leftover != null)
            {
                Object.DestroyImmediate(leftover.gameObject);
            }

            Light sun = Object.FindAnyObjectByType<Light>();
            if (sun != null)
            {
                sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                sun.intensity = 1.1f;
                sun.color = new Color(1f, 0.96f, 0.9f);
            }

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.35f, 0.35f, 0.38f);

            GameObject root = new GameObject("M01_Blockout");
            CreateCube("PB_Floor", root.transform, new Vector3(28f, 0.4f, 32f), new Vector3(0f, -0.2f, 2f), Quaternion.identity, material);
            CreateRamp(root.transform, material);
            CreateCube("PB_Step", root.transform, new Vector3(3f, 0.3f, 3f), new Vector3(-4f, 0.15f, 2f), Quaternion.identity, material);
            CreateCube("PB_Wall", root.transform, new Vector3(28f, 3f, 0.4f), new Vector3(0f, 1.5f, 18f), Quaternion.identity, material);

            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.name = "LocalPlayer";
            player.transform.position = new Vector3(0f, 0.05f, -8f);
            player.transform.rotation = Quaternion.identity;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[M01] Shell ready at " + ScenePath + " shader=" + material.shader.name);
            ExitBatch(0);
        }

        static void CreateRamp(Transform parent, Material material)
        {
            const float angle = 18f;
            const float length = 8f;
            const float thickness = 0.08f;
            float radians = angle * Mathf.Deg2Rad;
            float centerY = 0.02f
                + (Mathf.Sin(radians) * (length * 0.5f))
                + (Mathf.Cos(radians) * (thickness * 0.5f));
            ProBuilderMesh mesh = CreateCube(
                "PB_Ramp",
                parent,
                new Vector3(4f, thickness, length),
                new Vector3(3.5f, centerY, 4f),
                Quaternion.Euler(angle, 0f, 0f),
                material);
            MeshCollider meshCollider = mesh.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                Object.DestroyImmediate(meshCollider);
            }

            BoxCollider box = mesh.gameObject.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = new Vector3(4f, thickness, length);
        }

        static ProBuilderMesh CreateCube(
            string name,
            Transform parent,
            Vector3 size,
            Vector3 position,
            Quaternion rotation,
            Material material)
        {
            ProBuilderMesh mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            mesh.gameObject.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.SetPositionAndRotation(position, rotation);
            FinishMesh(mesh, material);
            return mesh;
        }

        static void FinishMesh(ProBuilderMesh mesh, Material material)
        {
            mesh.ToMesh();
            mesh.Refresh();
            mesh.SetMaterial(mesh.faces, material);

            MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            MeshCollider collider = mesh.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = mesh.gameObject.AddComponent<MeshCollider>();
            }

            MeshFilter filter = mesh.GetComponent<MeshFilter>();
            collider.sharedMesh = null;
            collider.convex = true;
            collider.sharedMesh = filter.sharedMesh;
        }

        static Material ResolveMaterial()
        {
            Material packageDefault = Resources.Load<Material>("Materials/ProBuilderDefault");
            Material source = packageDefault != null && IsErrorShader(packageDefault) == false
                ? packageDefault
                : AssetDatabase.LoadAssetAtPath<Material>(FallbackMaterialPath);
            if (source == null || IsErrorShader(source))
            {
                return source;
            }

            Material owned = AssetDatabase.LoadAssetAtPath<Material>(MaterialAssetPath);
            if (owned == null)
            {
                owned = new Material(source.shader);
                owned.CopyPropertiesFromMaterial(source);
                AssetDatabase.CreateAsset(owned, MaterialAssetPath);
            }
            else
            {
                owned.shader = source.shader;
                owned.CopyPropertiesFromMaterial(source);
                EditorUtility.SetDirty(owned);
            }

            if (packageDefault == null || IsErrorShader(packageDefault))
            {
                Debug.LogWarning("[M01] ProBuilderDefault is missing or magenta. Using Greybox.mat.");
            }

            return owned;
        }

        static bool IsErrorShader(Material material)
        {
            return material.shader == null
                || material.shader.isSupported == false
                || material.shader.name == "Hidden/InternalErrorShader";
        }

        static void AddSceneToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            bool found = false;
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path == scenePath)
                {
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    found = true;
                }
                else
                {
                    scenes.Add(existing);
                }
            }

            if (found == false)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
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
