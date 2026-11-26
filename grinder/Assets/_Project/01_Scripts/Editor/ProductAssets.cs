using System;
using System.IO;
using Meatgrinder.Pickups;
using Meatgrinder.Presentation;
using UnityEditor;
using UnityEngine;

namespace Meatgrinder.Editor
{
    public static class ProductAssets
    {
        public const string Root = "Assets/_Project/";
        public static string Art => Root + "05_ArtModels/Product/";
        public static T Asset<T>(string path) where T : UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(path);
        internal static void ApplySignFont(TextMesh sign)
        {
            sign.font = GameUi.FontFor(UiFontRole.Industrial);
            sign.color = GameUi.Paper;
            sign.GetComponent<MeshRenderer>().sharedMaterial = sign.font.material;
        }
        public static T Data<T>(string path) where T : ScriptableObject
        {
            var value = Asset<T>(path);
            if (value != null) return value;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            value = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(value, path); return value;
        }
        public static void Set(UnityEngine.Object target, string name, object value)
        {
            var data = new SerializedObject(target); var p = data.FindProperty(name);
            if (p == null) throw new InvalidOperationException(target.name + " has no property " + name);
            switch (value)
            {
                case UnityEngine.Object o: p.objectReferenceValue = o; break;
                case float f: p.floatValue = f; break;
                case int i: p.intValue = i; break;
                case bool b: p.boolValue = b; break;
                case string s: p.stringValue = s; break;
                case Color c: p.colorValue = c; break;
                case Vector3 v: p.vector3Value = v; break;
                case null: p.objectReferenceValue = null; break;
                default: throw new InvalidOperationException("Unsupported property value: " + name);
            }
            data.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(target);
        }
        public static void Array(UnityEngine.Object target, string name, UnityEngine.Object[] values)
        {
            var data = new SerializedObject(target); var array = data.FindProperty(name); array.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++) array.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            data.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(target);
        }
        public static Material Material(string name, Color color, int pattern = -1, float metal = 0, Color? glow = null)
        {
            Directory.CreateDirectory(Art);
            string path = Art + name + ".mat";
            var material = Asset<Material>(path);
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
            material.SetColor("_BaseColor", color); material.SetFloat("_Metallic", metal); material.SetFloat("_Smoothness", metal > 0 ? .35f : .12f);
            if (pattern >= 0) material.SetTexture("_BaseMap", Texture(name, pattern));
            if (glow.HasValue) { material.EnableKeyword("_EMISSION"); material.SetColor("_EmissionColor", glow.Value * 2); }
            EditorUtility.SetDirty(material); return material;
        }
        static Texture2D Texture(string name, int pattern)
        {
            string path = Art + name + ".png";
            var texture = Asset<Texture2D>(path); if (texture != null) return texture;
            var image = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            var random = new System.Random(1781 + pattern);
            for (int y = 0; y < 256; y++) for (int x = 0; x < 256; x++)
            {
                float noise = .72f + (float)random.NextDouble() * .2f;
                bool seam;
                if (pattern == 0) seam = x % 128 < 3 || y % 128 < 3;
                else if (pattern == 1) seam = y % 64 < 4 || (x + (y / 64 % 2) * 64) % 128 < 4;
                else seam = x % 32 < 3 || y % 32 < 3;
                if (seam) noise *= .38f;
                if (pattern == 0 && ((x % 128 - 10) * (x % 128 - 10) + (y % 128 - 10) * (y % 128 - 10) < 9)) noise = .3f;
                image.SetPixel(x, y, new Color(noise, noise, noise));
            }
            image.Apply(); File.WriteAllBytes(path, image.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(image);
            AssetDatabase.ImportAsset(path); return Asset<Texture2D>(path);
        }
        public static Sprite Icon(PowerUpType type)
        {
            Directory.CreateDirectory(Art); string path = Art + "Icon_" + type + ".png";
            var sprite = Asset<Sprite>(path); if (sprite != null) return sprite;
            var image = new Texture2D(96, 96, TextureFormat.RGBA32, false);
            for (int y = 0; y < 96; y++) for (int x = 0; x < 96; x++)
            {
                float u = (x - 48) / 40f, v = (y - 48) / 40f;
                bool pixel = false;
                switch (type)
                {
                    case PowerUpType.QuadDamage: pixel = Mathf.Abs(Mathf.Abs(u) - .42f) < .18f && Mathf.Abs(Mathf.Abs(v) - .42f) < .18f; break;
                    case PowerUpType.Megahealth: pixel = (Mathf.Abs(u) < .19f && Mathf.Abs(v) < .72f) || (Mathf.Abs(v) < .19f && Mathf.Abs(u) < .72f); break;
                    case PowerUpType.Haste: pixel = v > 0 ? u > -.3f && u < .15f + .5f * v : u > -.15f + .5f * v && u < .3f; pixel &= Mathf.Abs(v) < .8f; break;
                    case PowerUpType.Invulnerability: pixel = Mathf.Abs(u) < .68f && v < .72f && v > -1 + Mathf.Abs(u) * .8f && (Mathf.Abs(u) > .43f || v > .44f || v < -.68f + Mathf.Abs(u) * .8f); break;
                    case PowerUpType.Berserk: pixel = (Mathf.Abs(u) < .58f && v > -.25f && v < .58f) || (Mathf.Abs(u) < .32f && v > -.8f && v < 0); if (v > .25f && Mathf.Abs(u % .23f) < .045f) pixel = false; break;
                    case PowerUpType.Regeneration: float r = new Vector2(u, v).magnitude; pixel = r > .48f && r < .72f && !(u > .15f && v > .25f); pixel |= u > .1f && u < .7f && v > .15f && v < .5f && v < u; break;
                }
                image.SetPixel(x, y, pixel ? Color.white : Color.clear);
            }
            image.Apply(); File.WriteAllBytes(path, image.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(image);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path); importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true; importer.mipmapEnabled = false; importer.spritePixelsPerUnit = 96; importer.SaveAndReimport();
            return Asset<Sprite>(path);
        }
        public static AudioClip Sound(int kind)
        {
            string folder = Root + "04_Audio/SFX/Product/"; Directory.CreateDirectory(folder);
            string path = folder + "Feedback_" + kind + ".wav"; var clip = Asset<AudioClip>(path); if (clip != null) return clip;
            const int rate = 22050; float duration = kind == 8 ? .8f : kind == 5 ? .65f : kind == 0 || kind == 1 ? .32f : .16f;
            int count = (int)(rate * duration); var random = new System.Random(kind + 42);
            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + count * 2); writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16); writer.Write((short)1); writer.Write((short)1); writer.Write(rate); writer.Write(rate * 2); writer.Write((short)2); writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(count * 2);
                float low = 0;
                for (int i = 0; i < count; i++)
                {
                    float t = (float)i / rate, env = Mathf.Pow(1 - (float)i / count, 2);
                    float noise = (float)random.NextDouble() * 2 - 1; low = Mathf.Lerp(low, noise, .17f);
                    float frequency = kind == 6 || kind == 9 ? 420 + 1500 * t : kind == 5 ? 100 - 80 * t : 90;
                    float tone = Mathf.Sin(t * frequency * 2 * Mathf.PI);
                    float value = kind == 6 || kind == 9 ? tone * .28f : (kind == 8 ? low * 2.5f : noise * .4f + tone * .35f);
                    if (kind == 2) value *= Mathf.Sin(t * 45) > .2f ? 1 : .1f;
                    writer.Write((short)(Mathf.Clamp(value * env, -.9f, .9f) * 32767));
                }
            }
            AssetDatabase.ImportAsset(path); return Asset<AudioClip>(path);
        }
        public static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, bool collider = false)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale; go.GetComponent<Renderer>().sharedMaterial = material;
            if (!collider) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }
        public static GameObject SavePrefab(GameObject root, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)); var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root); return prefab;
        }
    }
}
