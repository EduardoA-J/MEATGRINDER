using System.IO;
using System.Linq;
using Meatgrinder.Combat;
using Meatgrinder.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using static Meatgrinder.Editor.ProductAssets;

namespace Meatgrinder.Editor
{
    public static class ProductMapPresentation
    {
        public static void Dress(int index, Transform root, Transform[] spawns, Material dark, Material steel, Material rust, Material glow)
        {
            Material stone = Material("CastleBasalt", new Color(.32f, .35f, .4f), 1);
            Material sand = Material("AncientStone", new Color(.62f, .49f, .32f), 1);
            Material floor = Material("DeckGrating", new Color(.27f, .29f, .31f), 2, .45f);
            Material voidStone = Material("VoidObsidian", new Color(.2f, .22f, .3f), 1, .3f);
            foreach (var renderer in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
            {
                if (!renderer.name.StartsWith("PB_")) continue;
                string name = renderer.name.ToLowerInvariant();
                if (name.Contains("_gem")) continue;
                Material selected = index == 0 ? dark : index == 1 ? stone : index == 2 ? sand : voidStone;
                if (index == 0 && (name.Contains("floor") || name.Contains("deck") || name.Contains("apron") || name.Contains("dock") || name.Contains("mezz") || name.Contains("landing"))) selected = floor;
                if (name.Contains("pipe") || name.Contains("beam") || name.Contains("rail")) selected = index == 0 ? rust : steel;
                renderer.sharedMaterial = selected;
            }
            if (root.Find("EnvironmentArt") == null)
            {
                var art = new GameObject("EnvironmentArt").transform; art.SetParent(root, false);
                if (index == 0) Industry(art, dark, steel, rust, glow);
                else if (index == 1) Gothic(art, stone, dark, rust, glow);
                else if (index == 2) Ruins(art, sand, dark, glow);
                else Void(art, voidStone, steel, glow);
                if (spawns.Length > 0)
                {
                    Sign(art, "ARENA / M0" + (index + 1), spawns[0].position + new Vector3(0, 3.5f, 7), Quaternion.identity, index == 0 ? glow.color : Color.white);
                    Target(art, spawns[0].position + new Vector3(0, 0, 9), 0);
                    Target(art, spawns[0].position + new Vector3(4, 0, 12), 1);
                    Target(art, spawns[0].position + new Vector3(-4, 0, 12), 2);
                }
            }
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = index == 2 ? new Color(.43f, .38f, .3f) : new Color(.22f, .25f, .31f);
            RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = index == 1 ? .004f : index == 3 ? .008f : .007f;
            RenderSettings.fogColor = index == 2 ? new Color(.46f, .39f, .29f) : index == 3 ? new Color(.018f, .025f, .055f) : new Color(.09f, .105f, .13f);
            if (index != 2) RenderSettings.skybox = null;
        }
        static void Industry(Transform parent, Material dark, Material steel, Material rust, Material glow)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector3 p = new Vector3(side * 25, 0, -42 + i * 18);
                    Part(parent, "ProcessTank", PrimitiveType.Cylinder, p + Vector3.up * 2, new Vector3(3.2f, 2, 3.2f), steel, true);
                    for (int ring = 0; ring < 4; ring++) Part(parent, "TankBand", PrimitiveType.Cylinder, p + Vector3.up * (.5f + ring), new Vector3(3.3f, .06f, 3.3f), rust);
                    Part(parent, "TankControl", PrimitiveType.Cube, p + new Vector3(-side * 1.6f, 1.5f, 0), new Vector3(.12f, .55f, .8f), dark);
                    Part(parent, "ControlStatus", PrimitiveType.Cube, p + new Vector3(-side * 1.68f, 1.5f, 0), new Vector3(.03f, .2f, .5f), glow);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                float x = -28 + i * 8;
                Part(parent, "ProcessRail", PrimitiveType.Cube, new Vector3(x, 7, 60), new Vector3(.16f, .2f, 32), steel);
                Part(parent, "Hanger", PrimitiveType.Cylinder, new Vector3(x, 5.7f, 62), new Vector3(.07f, 1.2f, .07f), steel);
                Part(parent, "Hook", PrimitiveType.Cube, new Vector3(x + .18f, 4.5f, 62), new Vector3(.42f, .08f, .08f), rust);
                Part(parent, "ProcessingLight", PrimitiveType.Cube, new Vector3(x, 6.8f, 54), new Vector3(3, .08f, .35f), glow);
            }
            Sign(parent, "01 / RECEPCIÓN", new Vector3(0, 3, -49.6f), Quaternion.Euler(0, 180, 0), new Color(1, .55f, .2f));
            Sign(parent, "02 / PROCESADO", new Vector3(0, 5.5f, 79.6f), Quaternion.identity, new Color(1, .55f, .2f));
            Sign(parent, "CÁMARAS FRÍAS  >", new Vector3(36, 2.4f, 20), Quaternion.identity, new Color(.3f, .8f, 1));
        }
        static void Gothic(Transform parent, Material stone, Material dark, Material rust, Material glow)
        {
            Material glass = Material("ChapelGlass", new Color(.45f, .055f, .09f), -1, 0, new Color(.6f, .04f, .02f));
            for (int side = -1; side <= 1; side += 2)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector3 p = new Vector3(side * 72, 0, -50 + i * 24);
                    Part(parent, "ButtressDetail", PrimitiveType.Cube, p + Vector3.up * 5, new Vector3(2, 10, 2), dark);
                    Part(parent, "GothicWindow", PrimitiveType.Cube, p + new Vector3(-side * 1.02f, 6, 0), new Vector3(.08f, 5, 1.5f), glass);
                    for (int bar = 0; bar < 3; bar++) Part(parent, "WindowMullion", PrimitiveType.Cube, p + new Vector3(-side * 1.1f, 4.5f + bar * 1.5f, 0), new Vector3(.12f, .09f, 1.6f), stone);
                    Part(parent, "TorchBowl", PrimitiveType.Cylinder, p + new Vector3(-side * 2, 2.5f, 0), new Vector3(.65f, .2f, .65f), rust);
                    Part(parent, "TorchEmber", PrimitiveType.Sphere, p + new Vector3(-side * 2, 2.8f, 0), new Vector3(.35f, .6f, .35f), glow);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4;
                Vector3 p = new Vector3(Mathf.Cos(angle) * 22, .07f, Mathf.Sin(angle) * 22 - 12);
                var marking = Part(parent, "CourtInlay", PrimitiveType.Cube, p, new Vector3(8, .025f, .2f), rust); marking.transform.localRotation = Quaternion.Euler(0, -i * 45, 0);
            }
            Sign(parent, "CRYPTE  /  CHAPELLE", new Vector3(0, 5, 80), Quaternion.identity, new Color(.85f, .7f, .4f));
        }
        static void Ruins(Transform parent, Material stone, Material dark, Material glow)
        {
            for (int side = -1; side <= 1; side += 2) for (int i = 0; i < 7; i++)
            {
                Vector3 p = new Vector3(side * 25, 0, 16 + i * 8);
                float height = i % 3 == 0 ? 2.4f : 5.5f;
                Part(parent, "ColumnFoot", PrimitiveType.Cube, p + Vector3.up * .3f, new Vector3(2.6f, .6f, 2.6f), dark);
                Part(parent, "AncientColumn", PrimitiveType.Cylinder, p + Vector3.up * (height * .5f + .6f), new Vector3(1.2f, height * .5f, 1.2f), stone, true);
                Part(parent, "Capital", PrimitiveType.Cube, p + Vector3.up * (height + .7f), new Vector3(1.8f, .35f, 1.8f), stone);
                if (i % 3 == 0)
                {
                    var broken = Part(parent, "FallenColumn", PrimitiveType.Cylinder, p + new Vector3(side * 2.6f, .6f, 1.5f), new Vector3(1.15f, 1.6f, 1.15f), stone, true);
                    broken.transform.localRotation = Quaternion.Euler(70, i * 37, 0);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                var slab = Part(parent, "TempleRelief", PrimitiveType.Cube, new Vector3(-18 + i * 4, 5.5f, 87), new Vector3(2.4f, 2.4f, .2f), dark);
                slab.transform.localRotation = Quaternion.Euler(0, 0, 45);
                Part(parent, "ReliefCore", PrimitiveType.Cube, new Vector3(-18 + i * 4, 5.5f, 86.85f), new Vector3(.3f, 1.2f, .08f), glow);
            }
            Sign(parent, "TEMPLE / III", new Vector3(0, 7, 84), Quaternion.identity, new Color(.9f, .76f, .45f));
        }
        static void Void(Transform parent, Material stone, Material steel, Material glow)
        {
            Material energy = Material("VoidEnergy", new Color(.2f, .55f, 1), -1, 0, new Color(.08f, .25f, 1));
            Vector3[] islands = { new Vector3(0, 6, 0), new Vector3(44, 10.2f, 2), new Vector3(-40, 3, -2), new Vector3(0, 10.5f, 42), new Vector3(6, 6, -38) };
            foreach (var p in islands)
            {
                for (int i = 0; i < 4; i++)
                {
                    var rock = Part(parent, "SuspendedFoundation", PrimitiveType.Cube, p + new Vector3(0, -2 - i * 2, 0), new Vector3(12 - i * 2.5f, 2.5f, 12 - i * 2.5f), stone);
                    rock.transform.localRotation = Quaternion.Euler(0, i * 17, 0);
                }
                for (int side = -1; side <= 1; side += 2)
                {
                    Part(parent, "AnchorObelisk", PrimitiveType.Cube, p + new Vector3(side * 7, 2, 0), new Vector3(.8f, 4, .8f), steel, true);
                    Part(parent, "AnchorEnergy", PrimitiveType.Cube, p + new Vector3(side * 7, 2, -.42f), new Vector3(.12f, 3, .04f), energy);
                }
                Part(parent, "IslandBeacon", PrimitiveType.Sphere, p + Vector3.up * 7, new Vector3(.8f, .8f, .8f), energy);
            }
            Sign(parent, "THE LONGEST VOID", new Vector3(0, 10, 9), Quaternion.identity, new Color(.4f, .75f, 1));
        }
        static void Sign(Transform parent, string text, Vector3 position, Quaternion rotation, Color color)
        {
            var go = new GameObject("Sign_" + text); go.transform.SetParent(parent, false); go.transform.SetPositionAndRotation(position, rotation);
            var mesh = go.AddComponent<TextMesh>(); mesh.text = text; mesh.fontSize = 64; mesh.characterSize = .055f; mesh.anchor = TextAnchor.MiddleCenter; mesh.color = color;
        }
        static void Target(Transform parent, Vector3 position, int index)
        {
            string[] names = { "20261110234906_b0c92559", "20261111001453_9a59cb6b", "20261111002122_967857ff" };
            string path = Root + "03_Prefabs/Players/TrainingTarget_" + index + ".prefab";
            var prefab = Asset<GameObject>(path);
            if (prefab == null)
            {
                var root = new GameObject("TrainingTarget_" + index); root.AddComponent<HealthComponent>();
                var body = root.AddComponent<CapsuleCollider>(); body.height = 2; body.center = Vector3.up; body.radius = .5f;
                var socket = new GameObject("ModelSocket"); socket.transform.SetParent(root.transform, false);
                var model = (GameObject)PrefabUtility.InstantiatePrefab(Asset<GameObject>(Root + "05_ArtModels/Imported/" + names[index] + ".fbx"));
                model.transform.SetParent(socket.transform, false);
                var renderers = model.GetComponentsInChildren<Renderer>(); Bounds bounds = renderers[0].bounds;
                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                float scale = 2 / bounds.size.y; model.transform.localScale = Vector3.one * scale;
                model.transform.localPosition = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z) * scale;
                var material = Material("TargetBronze" + index, index == 1 ? new Color(.48f, .36f, .22f) : new Color(.22f, .4f, .46f), -1, .55f);
                foreach (var renderer in renderers) renderer.sharedMaterial = material;
                Set(root.AddComponent<TargetPresentation>(), "modelSocket", socket.transform);
                Sign(root.transform, "BLANCO LOCAL", new Vector3(0, 2.4f, 0), Quaternion.identity, new Color(1, .6f, .2f));
                prefab = SavePrefab(root, path);
            }
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab); instance.transform.SetParent(parent, false); instance.transform.position = position;
        }
        public static void CaptureMaps()
        {
            Directory.CreateDirectory(ProductVerificationRunner.Output);
            for (int i = 0; i < 4; i++)
            {
                EditorSceneManager.OpenScene(Root + "00_Scenes/Maps/M0" + (i + 1) + "/" + ProductSceneInstaller.MapNames[i] + ".unity");
                var cameraObject = new GameObject("VerificationCamera", typeof(Camera)); var camera = cameraObject.GetComponent<Camera>();
                camera.fieldOfView = 64; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = RenderSettings.fogColor;
                Vector3[] eye = { new Vector3(55, 60, -55), new Vector3(180, 200, -210), new Vector3(65, 65, -35), new Vector3(50, 60, -65) };
                Vector3[] target = { new Vector3(0, 0, 20), new Vector3(0, 0, 40), new Vector3(0, 0, 35), Vector3.zero };
                camera.transform.position = eye[i]; camera.transform.LookAt(target[i]);
                var rt = new RenderTexture(1600, 1000, 24); camera.targetTexture = rt; camera.Render();
                RenderTexture.active = rt; var texture = new Texture2D(1600, 1000, TextureFormat.RGB24, false); texture.ReadPixels(new Rect(0, 0, 1600, 1000), 0, 0); texture.Apply();
                File.WriteAllBytes(Path.Combine(ProductVerificationRunner.Output, "M0" + (i + 1) + "-overview.png"), texture.EncodeToPNG());
                RenderTexture.active = null; camera.targetTexture = null; UnityEngine.Object.DestroyImmediate(rt); UnityEngine.Object.DestroyImmediate(texture); UnityEngine.Object.DestroyImmediate(cameraObject);
            }
        }
    }
}
