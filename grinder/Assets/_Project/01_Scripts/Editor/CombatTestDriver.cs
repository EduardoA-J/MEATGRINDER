using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Meatgrinder.Editor
{
    [InitializeOnLoad]
    public static class CombatTestDriver
    {
        const string RequestPath = "Logs/combat-test-request.txt";
        const string SummaryPath = "Logs/combat-test-summary.txt";

        static int waits;
        static string mode = string.Empty;
        static TestRunnerApi api;

        static CombatTestDriver()
        {
            if (File.Exists(RequestPath) == false)
            {
                return;
            }

            EditorApplication.update += Tick;
        }

        static void Tick()
        {
            waits++;
            if (waits < 60 || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            EditorApplication.update -= Tick;
            mode = File.ReadAllText(RequestPath).Trim();
            File.Delete(RequestPath);
            if (mode == "install")
            {
                EditorSceneManagerSaveAndInstall();
                return;
            }

            RunEditMode();
        }

        static void RunEditMode()
        {
            api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new EditCallbacks());
            api.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode,
                assemblyNames = new[] { "Meatgrinder.Tests.EditMode" }
            })
            {
                runSynchronously = true
            });
        }

        static void RunPlayMode()
        {
            api = ScriptableObject.CreateInstance<TestRunnerApi>();
            api.RegisterCallbacks(new PlayCallbacks());
            api.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.PlayMode,
                assemblyNames = new[] { "Meatgrinder.Tests.PlayMode" }
            }));
        }

        static void WriteSummary(string title, ITestResultAdaptor result)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(title);
            builder.AppendLine("passed=" + result.PassCount + " failed=" + result.FailCount + " skipped=" + result.SkipCount);
            AppendFailures(result, builder);
            File.AppendAllText(SummaryPath, builder.ToString());
            Debug.Log("[Combat] " + title + " passed=" + result.PassCount + " failed=" + result.FailCount);
        }

        static void AppendFailures(ITestResultAdaptor result, StringBuilder builder)
        {
            if (result.HasChildren)
            {
                foreach (ITestResultAdaptor child in result.Children)
                {
                    AppendFailures(child, builder);
                }

                return;
            }

            if (result.TestStatus == TestStatus.Failed)
            {
                builder.AppendLine(result.FullName);
                builder.AppendLine(result.Message);
            }
        }

        sealed class EditCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                WriteSummary("EDITMODE", result);
                if (result.FailCount == 0)
                {
                    RunPlayMode();
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }

        sealed class PlayCallbacks : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                WriteSummary("PLAYMODE", result);
                if (result.FailCount == 0)
                {
                    EditorSceneManagerSaveAndInstall();
                }
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }

        static void EditorSceneManagerSaveAndInstall()
        {
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            CombatGreyboxBuilder.Install();
        }
    }
}
