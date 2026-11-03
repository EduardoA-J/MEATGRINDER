using System.Collections.Generic;
using System.IO;
using Fusion;
using Meatgrinder.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class NetworkTestAssetBuilder
    {
        private const string ScenePath = "Assets/_Project/00_Scenes/NetworkTest.unity";
        private const string PrefabPath = "Assets/_Project/03_Prefabs/Players/NetworkTestPlayer.prefab";

        [MenuItem("Meatgrinder/Network Test/Create Scene And Prefab")]
        public static void CreateAssets()
        {
            Directory.CreateDirectory("Assets/_Project/00_Scenes");
            Directory.CreateDirectory("Assets/_Project/03_Prefabs/Players");

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "NetworkTestPlayer";
            cube.AddComponent<NetworkObject>();
            cube.AddComponent<NetworkTransform>();
            cube.AddComponent<NetworkTestAvatar>();
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cube, PrefabPath);
            Object.DestroyImmediate(cube);

            Fusion.Editor.NetworkProjectConfigUtilities.RebuildPrefabTable();

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Camera camera = Camera.main;
            if (camera != null)
            {
                camera.transform.SetPositionAndRotation(new Vector3(2f, 4f, -10f), Quaternion.identity);
                camera.transform.LookAt(new Vector3(2f, 1f, 0f));
            }

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.localScale = new Vector3(2f, 1f, 2f);

            GameObject sessionObject = new GameObject("NetworkSession");
            NetworkSessionController session = sessionObject.AddComponent<NetworkSessionController>();
            NetworkPlayerSpawner spawner = sessionObject.AddComponent<NetworkPlayerSpawner>();
            sessionObject.AddComponent<NetworkTestHud>();

            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(prefab));
            NetworkPrefabRef prefabRef = new System.Guid(guid);
            spawner.AssignPrefab(prefabRef);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddSceneToBuildSettings(ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[NetworkTest] Scene and player prefab are ready.");
        }

        [MenuItem("Meatgrinder/Network Test/Build Windows Client")]
        public static void BuildWindowsClient()
        {
            CreateAssets();
            string output = Path.GetFullPath("Builds/NetworkTest/MeatgrinderProbe.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(output) ?? "Builds/NetworkTest");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = output,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development
            };

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("[NetworkTest] Build failed: " + report.summary.result);
                EditorApplication.Exit(1);
                return;
            }

            Debug.Log("[NetworkTest] Build written to " + output);
            if (Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        public static void PlayAsHost()
        {
            if (File.Exists(ScenePath) == false || File.Exists(PrefabPath) == false)
            {
                CreateAssets();
            }

            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.EnterPlaymode();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path != scenePath)
                {
                    scenes.Add(existing);
                }
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
