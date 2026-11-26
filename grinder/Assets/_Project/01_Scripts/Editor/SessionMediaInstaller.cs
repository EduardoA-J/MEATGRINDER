using System;
using Meatgrinder.Core;
using Meatgrinder.Presentation;
using UnityEditor;
using UnityEngine;
using static Meatgrinder.Editor.ProductAssets;

namespace Meatgrinder.Editor
{
    public static class SessionMediaInstaller
    {
        const string Images = Root + "05_ArtModels/Materials/Menu/";
        const string Music = Root + "04_Audio/Music/";

        [MenuItem("Meatgrinder/Product/Assign menu and level media")]
        public static void Install()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode before assigning media.");
            foreach (string name in new[] { "PrincipalMenu", "level1", "level2", "level3", "level4", "afterFirstSong" })
            {
                var importer = (AudioImporter)AssetImporter.GetAtPath(Music + name + ".mp3");
                if (importer == null) throw new InvalidOperationException("Missing music importer: " + name);
                var settings = importer.defaultSampleSettings;
                if (settings.loadType == AudioClipLoadType.Streaming) continue;
                settings.loadType = AudioClipLoadType.Streaming;
                importer.defaultSampleSettings = settings;
                importer.SaveAndReimport();
            }
            var menu = Required<Texture2D>(Images + "PrincipalMenu.jfif");
            for (int i = 1; i <= 4; i++)
            {
                var map = Required<MapDefinition>(Root + "02_ScriptableObjects/Maps/M0" + i + ".asset");
                Set(map, "loadingBackground", Required<Texture2D>(Images + "level" + i + ".jfif"));
                Set(map, "firstSong", Required<AudioClip>(Music + "level" + i + ".mp3"));
            }
            var theme = Required<PresentationTheme>(Root + "02_ScriptableObjects/Presentation/ProductTheme.asset");
            Set(theme, "deathSound", Required<AudioClip>(Root + "04_Audio/SFX/DeathSfx.mp3"));
            string path = Root + "03_Prefabs/UI/LocalGameSession.prefab";
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Set(root.GetComponent<SessionMenu>(), "menuBackground", menu);
                Set(root.GetComponent<SessionLoadingScreen>(), "menuBackground", menu);
                var music = root.GetComponent<SessionMusic>() ?? root.AddComponent<SessionMusic>();
                Set(music, "menuMusic", Required<AudioClip>(Music + "PrincipalMenu.mp3"));
                Set(music, "afterFirstSong", Required<AudioClip>(Music + "afterFirstSong.mp3"));
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            AssetDatabase.SaveAssets();
        }
        static T Required<T>(string path) where T : UnityEngine.Object
        {
            if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<T>(path) ?? throw new InvalidOperationException("Missing media: " + path);
        }
    }
}
