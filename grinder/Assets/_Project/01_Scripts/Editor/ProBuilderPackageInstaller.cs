using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Meatgrinder.Editor
{
    public static class ProBuilderPackageInstaller
    {
        const string PackageId = "com.unity.probuilder@6.1.4";
        const double TimeoutSeconds = 600;

        static AddRequest request;
        static double deadline;

        public static void Install()
        {
            Debug.Log("[ProBuilder] Adding " + PackageId);
            request = Client.Add(PackageId);
            deadline = EditorApplication.timeSinceStartup + TimeoutSeconds;
            EditorApplication.update += Poll;
        }

        static void Poll()
        {
            if (request == null)
            {
                return;
            }

            if (request.IsCompleted == false)
            {
                if (EditorApplication.timeSinceStartup > deadline)
                {
                    EditorApplication.update -= Poll;
                    Debug.LogError("[ProBuilder] Timed out waiting for UPM.");
                    EditorApplication.Exit(2);
                }

                return;
            }

            EditorApplication.update -= Poll;
            if (request.Status != StatusCode.Success)
            {
                Debug.LogError("[ProBuilder] Failed: " + request.Error?.message);
                EditorApplication.Exit(1);
                return;
            }

            Debug.Log("[ProBuilder] Resolved " + request.Result.name + "@" + request.Result.version);
            int imported = ImportUrpSample();
            Debug.Log("[ProBuilder] URP samples imported: " + imported);
            EditorApplication.Exit(imported > 0 ? 0 : 3);
        }

        static int ImportUrpSample()
        {
            int imported = 0;
            foreach (Sample sample in Sample.FindByPackage("com.unity.probuilder", "6.1.4"))
            {
                Debug.Log("[ProBuilder] Sample '" + sample.displayName + "' imported=" + sample.isImported);
                string name = sample.displayName ?? string.Empty;
                bool urp = name.IndexOf("Universal", System.StringComparison.OrdinalIgnoreCase) >= 0
                    || name.IndexOf("URP", System.StringComparison.OrdinalIgnoreCase) >= 0;
                if (urp == false)
                {
                    continue;
                }

                if (sample.isImported == false && sample.Import(Sample.ImportOptions.HideImportWindow) == false)
                {
                    Debug.LogError("[ProBuilder] Failed to import sample " + name);
                    continue;
                }

                imported++;
            }

            return imported;
        }
    }
}
