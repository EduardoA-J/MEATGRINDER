using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Meatgrinder.Editor
{
    [ScriptedImporter(1, "jfif")]
    public sealed class JfifImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(context.assetPath), true))
            {
                Object.DestroyImmediate(texture);
                context.LogImportError("Could not decode JFIF image: " + context.assetPath);
                return;
            }
            texture.name = Path.GetFileNameWithoutExtension(context.assetPath);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            context.AddObjectToAsset("Background", texture);
            context.SetMainObject(texture);
        }
    }
}
