using System;
using System.IO;
using Meatgrinder.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    public static class UiStyleInstaller
    {
        [MenuItem("Meatgrinder/Product/Apply shared interface style")]
        public static void Install()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before authoring UI assets.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save open scenes before applying the interface style.");
            var setup = EditorSceneManager.GetSceneManagerSetup();
            int signs = 0;
            try
            {
                for (int i = 0; i < ProductSceneInstaller.MapNames.Length; i++)
                {
                    string path = ProductAssets.Root + "00_Scenes/Maps/M0" + (i + 1) + "/" + ProductSceneInstaller.MapNames[i] + ".unity";
                    var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                    foreach (var root in scene.GetRootGameObjects())
                        foreach (var text in root.GetComponentsInChildren<TextMesh>(true))
                        { ProductAssets.ApplySignFont(text); EditorUtility.SetDirty(text); signs++; }
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
                for (int i = 0; i < 3; i++)
                {
                    string path = ProductAssets.Root + "03_Prefabs/Players/TrainingTarget_" + i + ".prefab";
                    var root = PrefabUtility.LoadPrefabContents(path);
                    try
                    {
                        foreach (var text in root.GetComponentsInChildren<TextMesh>(true)) ProductAssets.ApplySignFont(text);
                        PrefabUtility.SaveAsPrefabAsset(root, path);
                    }
                    finally { PrefabUtility.UnloadPrefabContents(root); }
                }
                AssetDatabase.SaveAssets();
                string output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Documentation/test-results/ui-redesign"));
                Directory.CreateDirectory(output);
                File.WriteAllText(Path.Combine(output, "installation.txt"), "Styled " + signs + " map text meshes and three target prefabs; original background artwork preserved.\n" + DateTime.UtcNow.ToString("O"));
                Debug.Log("[UiStyle] Installed shared interface style.");
            }
            finally { EditorSceneManager.RestoreSceneManagerSetup(setup); }
        }
    }
}
