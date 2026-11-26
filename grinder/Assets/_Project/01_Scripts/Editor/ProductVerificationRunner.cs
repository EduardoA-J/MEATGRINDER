using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Meatgrinder.Editor
{
    // An opt-in command file lets automation use the already open editor without losing scenes.
    [InitializeOnLoad]
    public static class ProductVerificationRunner
    {
        static readonly string CommandPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Temp/MeatgrinderCommand.txt"));
        public static readonly string Output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Documentation/test-results/product"));
        static double nextPoll;
        static ProductVerificationRunner() => EditorApplication.update += Poll;
        static void Poll()
        {
            if (EditorApplication.timeSinceStartup < nextPoll || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            nextPoll = EditorApplication.timeSinceStartup + 1;
            if (!File.Exists(CommandPath)) return;
            string command = File.ReadAllText(CommandPath).Trim();
            File.Delete(CommandPath);
            Directory.CreateDirectory(Output);
            try
            {
                if (command == "inspect") InspectModels();
                else if (command == "media") SessionMediaInstaller.Install();
                else if (command == "style") UiStyleInstaller.Install();
                else if (command == "install") typeof(ProductVerificationRunner).Assembly.GetType("Meatgrinder.Editor.ProductSceneInstaller").GetMethod("Install").Invoke(null, null);
                else if (command == "editTests" || command == "playTests" || command == "mediaEditTests" || command == "mediaPlayTests")
                {
                    var api = ScriptableObject.CreateInstance<TestRunnerApi>();
                    api.RegisterCallbacks(new Results(command));
                    api.Execute(new ExecutionSettings(new Filter { testMode = command == "editTests" || command == "mediaEditTests" ? TestMode.EditMode : TestMode.PlayMode }));
                }
                else if (command == "capture") typeof(ProductVerificationRunner).Assembly.GetType("Meatgrinder.Editor.ProductSceneInstaller").GetMethod("Capture").Invoke(null, null);
                else if (command == "refresh") AssetDatabase.Refresh();
                else throw new InvalidOperationException("Unknown verification command: " + command);
                File.WriteAllText(Path.Combine(Output, "command.txt"), command + " completed " + DateTime.UtcNow.ToString("O"));
            }
            catch (Exception ex) { File.WriteAllText(Path.Combine(Output, "command.txt"), command + " FAILED\n" + ex); Debug.LogException(ex); }
        }
        sealed class Results : ICallbacks
        {
            readonly string name;
            public Results(string value) => name = value;
            public void RunStarted(ITestAdaptor testsToRun) { }
            public void TestStarted(ITestAdaptor test) { }
            public void TestFinished(ITestResultAdaptor result) { }
            public void RunFinished(ITestResultAdaptor result)
            {
                TestRunnerApi.SaveResultToFile(result, Path.Combine(Output, name + ".xml"));
                File.WriteAllText(Path.Combine(Output, name + ".txt"), result.TestStatus + " Passed=" + result.PassCount + " Failed=" + result.FailCount);
            }
        }
        public static void InspectModels()
        {
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            var lines = new System.Text.StringBuilder();
            foreach (string guid in AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project/05_ArtModels/Imported" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var preview = new PreviewRenderUtility();
                var instance = UnityEngine.Object.Instantiate(prefab);
                preview.AddSingleGO(instance);
                var renderers = instance.GetComponentsInChildren<Renderer>();
                Bounds bounds = new Bounds();
                bool first = true;
                foreach (var renderer in renderers) { if (first) { bounds = renderer.bounds; first = false; } else bounds.Encapsulate(renderer.bounds); }
                lines.AppendLine(path + " bounds=" + bounds + " meshes=" + renderers.Length + " vertices=" + instance.GetComponentsInChildren<MeshFilter>().Sum(x => x.sharedMesh != null ? x.sharedMesh.vertexCount : 0));
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                lines.AppendLine("  skinnedRenderers=" + instance.GetComponentsInChildren<SkinnedMeshRenderer>().Length +
                    " animators=" + instance.GetComponentsInChildren<Animator>().Length +
                    " animationClips=" + AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Count(c => !c.name.StartsWith("__preview__")) +
                    " importerAnimationType=" + (importer != null ? importer.animationType.ToString() : "unknown"));
                foreach (var r in renderers) lines.AppendLine("  " + r.name + " materials=" + string.Join(",", r.sharedMaterials.Select(m => m != null ? m.name : "NULL")));
                float radius = Mathf.Max(0.1f, bounds.extents.magnitude);
                preview.camera.transform.position = bounds.center + new Vector3(1, .65f, -1.2f).normalized * radius * 3;
                preview.camera.transform.LookAt(bounds.center);
                preview.camera.nearClipPlane = radius * .01f;
                preview.camera.fieldOfView = 60;
                preview.camera.farClipPlane = radius * 10;
                preview.camera.clearFlags = CameraClearFlags.SolidColor;
                preview.camera.backgroundColor = new Color(.12f, .13f, .16f);
                preview.lights[0].intensity = 1.4f;
                preview.lights[0].transform.rotation = Quaternion.Euler(40, 30, 0);
                preview.lights[1].intensity = .8f;
                preview.BeginStaticPreview(new Rect(0, 0, 768, 768));
                preview.camera.Render();
                var texture = preview.EndStaticPreview();
                File.WriteAllBytes(Path.Combine(Output, Path.GetFileNameWithoutExtension(path) + ".png"), texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                preview.Cleanup();
            }
            File.WriteAllText(Path.Combine(Output, "models.txt"), lines.ToString());
        }
    }
}
