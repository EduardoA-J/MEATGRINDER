using System.Collections.Generic;
using System.IO;
using Meatgrinder.Movement;
using Meatgrinder.Players;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class MovementGreyboxBuilder
    {
        const string ScenePath = "Assets/_Project/00_Scenes/Maps/Greybox/Greybox.unity";
        const string PrefabPath = "Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab";
        const string TuningPath = "Assets/_Project/02_ScriptableObjects/Movement/MovementTuning.asset";
        const string ActionsPath = "Assets/_Project/02_ScriptableObjects/Input/MeatgrinderActions.inputactions";
        const string MaterialPath = "Assets/_Project/05_ArtModels/Materials/Greybox.mat";

        [MenuItem("Meatgrinder/Movement/Create Greybox")]
        public static void CreateAssets()
        {
            Directory.CreateDirectory("Assets/_Project/00_Scenes/Maps/Greybox");
            Directory.CreateDirectory("Assets/_Project/02_ScriptableObjects/Movement");
            Directory.CreateDirectory("Assets/_Project/03_Prefabs/Players");
            Directory.CreateDirectory("Assets/_Project/05_ArtModels/Materials");

            Material material = CreateMaterial();
            MovementTuning tuning = CreateTuning();
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
            if (actions == null)
            {
                Debug.LogError("[Movement] Missing " + ActionsPath);
                ExitBatch(1);
                return;
            }

            GameObject prefab = CreatePlayerPrefab(tuning, actions);
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Camera leftover = Camera.main;
            if (leftover != null)
            {
                Object.DestroyImmediate(leftover.gameObject);
            }

            BuildArena(material);
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            player.name = "LocalPlayer";
            player.transform.position = new Vector3(0f, 0.05f, -18f);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Movement] Greybox ready at " + ScenePath);
            ExitBatch(0);
        }

        static Material CreateMaterial()
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material != null)
            {
                return material;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            material = new Material(shader);
            material.SetColor("_BaseColor", new Color(0.55f, 0.55f, 0.55f, 1f));
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }

        static MovementTuning CreateTuning()
        {
            MovementTuning tuning = AssetDatabase.LoadAssetAtPath<MovementTuning>(TuningPath);
            if (tuning != null)
            {
                return tuning;
            }

            tuning = ScriptableObject.CreateInstance<MovementTuning>();
            AssetDatabase.CreateAsset(tuning, TuningPath);
            return tuning;
        }

        static GameObject CreatePlayerPrefab(MovementTuning tuning, InputActionAsset actions)
        {
            GameObject player = new GameObject("LocalPlayer");
            CharacterController body = player.AddComponent<CharacterController>();
            PlayerInputReader reader = player.AddComponent<PlayerInputReader>();
            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            PlayerLook look = player.AddComponent<PlayerLook>();
            MovementProbeReadout readout = player.AddComponent<MovementProbeReadout>();

            GameObject cameraObject = new GameObject("View");
            cameraObject.transform.SetParent(player.transform, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.nearClipPlane = 0.05f;

            SerializedObject readerObject = new SerializedObject(reader);
            readerObject.FindProperty("actions").objectReferenceValue = actions;
            readerObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject movementObject = new SerializedObject(movement);
            movementObject.FindProperty("tuning").objectReferenceValue = tuning;
            movementObject.FindProperty("planarInputSource").objectReferenceValue = reader;
            movementObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject lookObject = new SerializedObject(look);
            lookObject.FindProperty("tuning").objectReferenceValue = tuning;
            lookObject.FindProperty("input").objectReferenceValue = reader;
            lookObject.FindProperty("view").objectReferenceValue = camera;
            lookObject.ApplyModifiedPropertiesWithoutUndo();

            SerializedObject readoutObject = new SerializedObject(readout);
            readoutObject.FindProperty("movement").objectReferenceValue = movement;
            readoutObject.ApplyModifiedPropertiesWithoutUndo();

            body.height = tuning.Height;
            body.radius = tuning.Radius;
            body.center = new Vector3(0f, tuning.Height * 0.5f, 0f);
            body.stepOffset = tuning.StepOffset;
            body.slopeLimit = tuning.SlopeLimit;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(player, PrefabPath);
            Object.DestroyImmediate(player);
            return prefab;
        }

        static void BuildArena(Material material)
        {
            CreateBox("Floor", new Vector3(0f, -0.5f, 10f), new Vector3(64f, 1f, 96f), material);
            CreateBox("Wall_North", new Vector3(0f, 2f, 58f), new Vector3(64f, 4f, 1f), material);
            CreateBox("Wall_South", new Vector3(0f, 2f, -38f), new Vector3(64f, 4f, 1f), material);
            CreateBox("Wall_East", new Vector3(32f, 2f, 10f), new Vector3(1f, 4f, 96f), material);
            CreateBox("Wall_West", new Vector3(-32f, 2f, 10f), new Vector3(1f, 4f, 96f), material);
            CreateBox("Wall_Lane", new Vector3(-6f, 1.5f, 8f), new Vector3(1f, 3f, 10f), material);
            CreateBox("Step_Low", new Vector3(0f, 0.15f, 22f), new Vector3(4f, 0.3f, 4f), material);
            CreateBox("Step_Blocked", new Vector3(6f, 0.45f, 22f), new Vector3(4f, 0.9f, 4f), material);
            CreateBox("Platform_Low", new Vector3(-10f, 0.5f, 40f), new Vector3(6f, 1f, 6f), material);
            CreateBox("Platform_High", new Vector3(-10f, 1.25f, 50f), new Vector3(6f, 2.5f, 6f), material);
            CreateRamp(material);
        }

        static void CreateRamp(Material material)
        {
            const float angle = 18f;
            const float length = 12f;
            const float thickness = 0.5f;
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ramp";
            ramp.transform.localScale = new Vector3(4f, thickness, length);
            ramp.transform.rotation = Quaternion.Euler(-angle, 0f, 0f);
            float low = (-Mathf.Sin(angle * Mathf.Deg2Rad) * (length * 0.5f))
                + (Mathf.Cos(angle * Mathf.Deg2Rad) * (thickness * 0.5f));
            ramp.transform.position = new Vector3(10f, -low + 0.02f, 36f);
            Assign(ramp, material);
        }

        static void CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = position;
            box.transform.localScale = scale;
            Assign(box, material);
        }

        static void Assign(GameObject target, Material material)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null && material != null)
            {
                renderer.sharedMaterial = material;
            }
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
