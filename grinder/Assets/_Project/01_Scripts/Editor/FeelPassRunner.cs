using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    [InitializeOnLoad]
    public static class FeelPassRunner
    {
        const string RequestPath = "Logs/feel-test-request.txt";
        const string ActiveKey = "Meatgrinder.Feel.Active";
        const string StageKey = "Meatgrinder.Feel.Stage";
        const string EditStartedKey = "Meatgrinder.Feel.EditStarted";
        const string PlayStartedKey = "Meatgrinder.Feel.PlayStarted";
        const string EditResults = "F:/MAPAUNI/MEATGRINDER/Documentation/test-results/feel-editmode.xml";
        const string PlayResults = "F:/MAPAUNI/MEATGRINDER/Documentation/test-results/feel-playmode.xml";

        static bool callbacksReady;

        static FeelPassRunner()
        {
            if (SessionState.GetString(StageKey, string.Empty) == "play")
            {
                Register();
            }

            EditorApplication.delayCall += Kick;
        }

        [MenuItem("Meatgrinder/Tests/Run Feel Pass")]
        public static void Request()
        {
            Directory.CreateDirectory("Logs");
            File.WriteAllText(RequestPath, "feel");
            SessionState.EraseBool(ActiveKey);
            Kick();
        }

        static void Kick()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += Kick;
                return;
            }

            if (File.Exists(RequestPath))
            {
                File.Delete(RequestPath);
                SessionState.SetBool(ActiveKey, true);
                SessionState.SetString(StageKey, "edit");
                SessionState.EraseBool(EditStartedKey);
                SessionState.EraseBool(PlayStartedKey);
            }

            if (SessionState.GetBool(ActiveKey, false) == false)
            {
                return;
            }

            Register();
            string stage = SessionState.GetString(StageKey, "edit");
            if (stage == "edit")
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || SessionState.GetBool(EditStartedKey, false))
                {
                    return;
                }

                SessionState.SetBool(EditStartedKey, true);
                Execute(TestMode.EditMode);
                return;
            }

            if (stage == "play")
            {
                if (SessionState.GetBool(PlayStartedKey, false))
                {
                    return;
                }

                SessionState.SetBool(PlayStartedKey, true);
                Execute(TestMode.PlayMode);
                return;
            }

            if (stage == "course")
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    EditorApplication.delayCall += Kick;
                    return;
                }

                SessionState.EraseBool(ActiveKey);
                SessionState.EraseString(StageKey);
                Scene scene = EditorSceneManager.GetActiveScene();
                if (scene.path.EndsWith("Greybox.unity") && scene.isDirty)
                {
                    EditorSceneManager.SaveScene(scene);
                }

                MovementCourseBuilder.Install();
            }
        }

        static void Register()
        {
            if (callbacksReady)
            {
                return;
            }

            TestRunnerApi api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new FeelCallbacks());
            callbacksReady = true;
        }

        static void Execute(TestMode mode)
        {
            string assembly = mode == TestMode.EditMode
                ? "Meatgrinder.Tests.EditMode"
                : "Meatgrinder.Tests.PlayMode";
            Filter filter = new Filter
            {
                testMode = mode,
                assemblyNames = new[] { assembly }
            };
            TestRunnerApi api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.Execute(new ExecutionSettings(filter));
            Debug.Log("[Feel] " + mode + " started.");
        }

        sealed class FeelCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                string stage = SessionState.GetString(StageKey, string.Empty);
                string path = stage == "play" ? PlayResults : EditResults;
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, result.ToXml().OuterXml);
                Debug.Log("[Feel] " + stage + " passed " + result.PassCount + " failed " + result.FailCount);
                if (stage == "edit")
                {
                    SessionState.SetString(StageKey, "play");
                    SessionState.EraseBool(PlayStartedKey);
                    EditorApplication.delayCall += Kick;
                    return;
                }

                SessionState.SetString(StageKey, "course");
                EditorApplication.delayCall += Kick;
            }
        }
    }
}
