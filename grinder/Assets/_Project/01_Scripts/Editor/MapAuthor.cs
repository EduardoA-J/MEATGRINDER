using System.Collections.Generic;
using System.IO;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Editor
{
    // Saves real ProBuilder meshes into map scenes. Pieces stay editable in the editor.
    // M01 never rebuilds PB_Ramp, PB_Floor, PB_Step or PB_Wall.
    public static class MapAuthor
    {
        const string PlayerPath = "Assets/_Project/03_Prefabs/Players/LocalPlayer.prefab";
        const string BlockoutMaterial = "Assets/_Project/00_Scenes/Maps/M01/M01_Blockout.mat";
        const string PickupFolder = "Assets/_Project/02_ScriptableObjects/Pickups";
        const string RocketPath = "Assets/_Project/02_ScriptableObjects/Weapons/RocketLauncher.asset";

        [MenuItem("Meatgrinder/Maps/Expand M01")]
        public static void ExpandM01()
        {
            BuildAll(false, true, false, false, false);
        }

        [MenuItem("Meatgrinder/Maps/Build M02 Castle")]
        public static void BuildM02()
        {
            BuildAll(false, false, true, false, false);
        }

        [MenuItem("Meatgrinder/Maps/Build M03 Ruins")]
        public static void BuildM03()
        {
            BuildAll(false, false, false, true, false);
        }

        [MenuItem("Meatgrinder/Maps/Build M04 Void")]
        public static void BuildM04()
        {
            BuildAll(false, false, false, false, true);
        }

        public static void BuildAll()
        {
            BuildAll(true, true, true, true, true);
        }

        static void BuildAll(bool exit, bool m01, bool m02, bool m03, bool m04)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(BlockoutMaterial);
            if (material == null || material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
            {
                Debug.LogError("[Maps] Missing blockout material.");
                if (exit)
                {
                    EditorApplication.Exit(1);
                }

                return;
            }

            Directory.CreateDirectory(PickupFolder);
            PickupKit kit = PickupKit.LoadOrCreate();
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPath);
            if (playerPrefab == null)
            {
                Debug.LogError("[Maps] Missing LocalPlayer.");
                if (exit)
                {
                    EditorApplication.Exit(1);
                }

                return;
            }

            if (m01)
            {
                ExpandSlaughterhouse(material, kit);
            }

            if (m02)
            {
                BuildCastle(material, kit, playerPrefab);
            }

            if (m03)
            {
                BuildRuins(material, kit, playerPrefab);
            }

            if (m04)
            {
                BuildVoid(material, kit, playerPrefab);
            }

            AssetDatabase.SaveAssets();
            if (exit && Application.isBatchMode)
            {
                EditorApplication.Exit(0);
            }
        }

        static void ExpandSlaughterhouse(Material material, PickupKit kit)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M01/M01_TheSlaughterhouse.unity";
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            GameObject ramp = GameObject.Find("PB_Ramp");
            if (ramp == null)
            {
                Debug.LogError("[Maps] PB_Ramp is missing. Expansion aborted.");
                return;
            }

            Vector3 rampPosition = ramp.transform.position;
            LightMood(new Color(1f, 0.72f, 0.48f), new Color(0.16f, 0.13f, 0.12f), 1.15f, new Color(0.22f, 0.14f, 0.1f), 0.012f, new Vector3(42f, -35f, 0f));
            ReplaceRoot("M01_Expansion");
            GameObject root = new GameObject("M01_Expansion");
            MapGeometry.Slaughterhouse(root.transform, material, kit);

            GameObject player = GameObject.Find("LocalPlayer");
            if (player != null)
            {
                Block.BindSpawn(player, root.transform.Find("M01_Spawns/Spawn_Human_A"));
            }

            if (GameObject.Find("PB_Ramp") == null || Vector3.Distance(GameObject.Find("PB_Ramp").transform.position, rampPosition) > 0.01f)
            {
                Debug.LogError("[Maps] PB_Ramp moved during expansion.");
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AddScene(path);
            Debug.Log("[Maps] M01 expanded. Ramp kept at " + rampPosition);
        }

        static void BuildCastle(Material material, PickupKit kit, GameObject playerPrefab)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M02/M02_CastleOfTheDamned.unity";
            Scene scene = NewScene(path);
            LightMood(new Color(0.62f, 0.68f, 0.78f), new Color(0.1f, 0.11f, 0.13f), 0.85f, new Color(0.08f, 0.09f, 0.11f), 0.011f, new Vector3(28f, 140f, 0f));
            GameObject root = new GameObject("M02_Castle");
            MapGeometry.Castle(root.transform, material, kit);
            PlacePlayer(playerPrefab, new Vector3(0f, 0.2f, -10f), root.transform.Find("M02_Spawns/Spawn_Human_A"));
            Finish(scene, path, null);
            Debug.Log("[Maps] M02 castle saved.");
        }

        static void BuildRuins(Material material, PickupKit kit, GameObject playerPrefab)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M03/M03_RuinsOfTheForgotten.unity";
            Scene scene = NewScene(path);
            LightMood(new Color(1f, 0.9f, 0.72f), new Color(0.42f, 0.34f, 0.24f), 1.35f, new Color(0.72f, 0.58f, 0.4f), 0.008f, new Vector3(54f, -24f, 0f));
            GameObject root = new GameObject("M03_Ruins");
            MapGeometry.Ruins(root.transform, material, kit);
            PlacePlayer(playerPrefab, new Vector3(-28f, 2.8f, 6f), root.transform.Find("M03_Spawns/Spawn_Human_A"));
            Finish(scene, path, null);
            Debug.Log("[Maps] M03 ruins saved.");
        }

        static void BuildVoid(Material material, PickupKit kit, GameObject playerPrefab)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M04/M04_TheLongestVoid.unity";
            Scene scene = NewScene(path);
            RenderSettings.skybox = null;
            LightMood(new Color(0.55f, 0.7f, 0.9f), new Color(0.02f, 0.03f, 0.05f), 0.7f, new Color(0.01f, 0.015f, 0.03f), 0.022f, new Vector3(36f, 18f, 0f));
            GameObject root = new GameObject("M04_Void");
            MapGeometry.Void(root.transform, material, kit);
            GameObject player = PlacePlayer(playerPrefab, new Vector3(0f, 6.4f, -2f), root.transform.Find("M04_Spawns/Spawn_Human_A"));
            Camera view = player.GetComponentInChildren<Camera>();
            if (view != null)
            {
                view.clearFlags = CameraClearFlags.SolidColor;
                view.backgroundColor = new Color(0.01f, 0.012f, 0.02f);
            }

            Finish(scene, path, null);
            Debug.Log("[Maps] M04 void saved. No ground plane.");
        }

        static Scene NewScene(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "Assets");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            Camera leftover = Camera.main;
            if (leftover != null)
            {
                Object.DestroyImmediate(leftover.gameObject);
            }

            return scene;
        }

        static void LightMood(Color sunColor, Color ambient, float intensity, Color fog, float density, Vector3 euler)
        {
            Light sun = Object.FindAnyObjectByType<Light>();
            if (sun != null)
            {
                sun.color = sunColor;
                sun.intensity = intensity;
                sun.shadows = LightShadows.Soft;
                sun.transform.rotation = Quaternion.Euler(euler);
            }

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = ambient;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = fog;
            RenderSettings.fogDensity = density;
        }

        static GameObject PlacePlayer(GameObject prefab, Vector3 position, Transform spawn)
        {
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            player.name = "LocalPlayer";
            player.transform.SetPositionAndRotation(position, Quaternion.identity);
            Block.BindSpawn(player, spawn);
            return player;
        }

        static void Finish(Scene scene, string path, GameObject ignored)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, path);
            AddScene(path);
        }

        static void ReplaceRoot(string name)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }
        }

        static void AddScene(string scenePath)
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            bool found = false;
            foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
            {
                if (existing.path == scenePath)
                {
                    scenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    found = true;
                }
                else
                {
                    scenes.Add(existing);
                }
            }

            if (found == false)
            {
                scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        internal sealed class PickupKit
        {
            public PickupDefinition Health;
            public PickupDefinition Armor;
            public PickupDefinition Quad;
            public PickupDefinition Berserk;
            public PickupDefinition Rockets;

            public static PickupKit LoadOrCreate()
            {
                WeaponDefinition rocket = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(RocketPath);
                return new PickupKit
                {
                    Health = Define("Health_Small", "health_small", "Salud", PickupType.Health, 25f, PowerUpType.QuadDamage, 0f, 0f, null, 0),
                    Armor = Define("Armor_Small", "armor_small", "Armadura", PickupType.Armor, 25f, PowerUpType.QuadDamage, 0f, 0f, null, 0),
                    Quad = Define("Quad", "quad", "Quad Damage", PickupType.PowerUp, 0f, PowerUpType.QuadDamage, 30f, 2f, null, 0),
                    Berserk = Define("Berserk", "berserk", "Berserk", PickupType.PowerUp, 0f, PowerUpType.Berserk, 30f, 2f, null, 0),
                    Rockets = Define("Rocket_Ammo", "rocket_ammo", "Cohetes", PickupType.Weapon, 0f, PowerUpType.QuadDamage, 0f, 0f, rocket, 4)
                };
            }

            static PickupDefinition Define(
                string file,
                string id,
                string label,
                PickupType type,
                float amount,
                PowerUpType power,
                float duration,
                float effect,
                WeaponDefinition weapon,
                int ammo)
            {
                string path = PickupFolder + "/" + file + ".asset";
                PickupDefinition asset = AssetDatabase.LoadAssetAtPath<PickupDefinition>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<PickupDefinition>();
                    AssetDatabase.CreateAsset(asset, path);
                }

                SerializedObject data = new SerializedObject(asset);
                data.FindProperty("pickupId").stringValue = id;
                data.FindProperty("displayName").stringValue = label;
                data.FindProperty("type").enumValueIndex = (int)type;
                data.FindProperty("amount").floatValue = amount;
                data.FindProperty("respawnTime").floatValue = 30f;
                data.FindProperty("canPickupWhenFull").boolValue = type == PickupType.Weapon || type == PickupType.PowerUp;
                data.FindProperty("powerUpType").enumValueIndex = (int)power;
                data.FindProperty("duration").floatValue = duration;
                data.FindProperty("effectValue").floatValue = effect;
                data.FindProperty("weapon").objectReferenceValue = weapon;
                data.FindProperty("ammoAmount").intValue = ammo;
                data.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(asset);
                return asset;
            }
        }
    }

    static class Block
    {
        public static void Slab(Transform parent, string name, float x, float z, float yTop, float width, float depth, float thickness, Material material, Color color)
        {
            Cube(parent, name, new Vector3(width, thickness, depth), new Vector3(x, yTop - (thickness * 0.5f), z), Quaternion.identity, material, color);
        }

        public static void WallX(Transform parent, string name, float x, float z, float length, float height, float yBase, float thickness, Material material, Color color)
        {
            Cube(parent, name, new Vector3(thickness, height, length), new Vector3(x, yBase + (height * 0.5f), z), Quaternion.identity, material, color);
        }

        public static void WallZ(Transform parent, string name, float x, float z, float length, float height, float yBase, float thickness, Material material, Color color)
        {
            Cube(parent, name, new Vector3(length, height, thickness), new Vector3(x, yBase + (height * 0.5f), z), Quaternion.identity, material, color);
        }

        public static void Column(Transform parent, string name, float x, float z, float height, Material material, Color color)
        {
            ColumnOn(parent, name, x, z, height, 0f, material, color);
        }

        public static void ColumnOn(Transform parent, string name, float x, float z, float height, float yBase, Material material, Color color)
        {
            Cylinder(parent, name, 0.55f, height, new Vector3(x, yBase + (height * 0.5f), z), Quaternion.identity, material, color);
        }

        public static void Pillar(Transform parent, string name, float x, float z, float yBase, float height, float radius, Material material, Color shaft, Color trim)
        {
            float baseHeight = 0.45f;
            float capitalHeight = 0.35f;
            Cube(parent, name + "_Base", new Vector3(radius * 2.6f, baseHeight, radius * 2.6f), new Vector3(x, yBase + (baseHeight * 0.5f), z), Quaternion.identity, material, trim);
            float shaftHeight = Mathf.Max(0.8f, height - baseHeight - capitalHeight);
            Cylinder(parent, name + "_Shaft", radius, shaftHeight, new Vector3(x, yBase + baseHeight + (shaftHeight * 0.5f), z), Quaternion.identity, material, shaft);
            Cube(parent, name + "_Capital", new Vector3(radius * 3f, capitalHeight, radius * 3f), new Vector3(x, yBase + height - (capitalHeight * 0.5f), z), Quaternion.identity, material, trim);
        }

        public static void Cylinder(Transform parent, string name, float radius, float height, Vector3 position, Quaternion rotation, Material material, Color color)
        {
            ProBuilderMesh mesh = ShapeGenerator.GenerateCylinder(PivotLocation.Center, 10, radius, height, 1, 24);
            Finish(mesh, parent, name, new Vector3(radius * 2f, height, radius * 2f), position, rotation, material, color);
        }

        public static void Pipe(Transform parent, string name, Vector3 center, Vector3 axis, float radius, float length, Material material, Color color)
        {
            Vector3 direction = axis.sqrMagnitude > 0.001f ? axis.normalized : Vector3.forward;
            Cylinder(parent, name, radius, length, center, Quaternion.FromToRotation(Vector3.up, direction), material, color);
        }

        public static void Machine(Transform parent, string name, Vector3 position, Material material, Color body, Color drum)
        {
            Cube(parent, name + "_Bed", new Vector3(3.6f, 0.9f, 2.6f), position + new Vector3(0f, 0.45f, 0f), Quaternion.identity, material, body);
            Cylinder(parent, name + "_Drum", 0.7f, 2.4f, position + new Vector3(0f, 1.55f, 0f), Quaternion.Euler(0f, 0f, 90f), material, drum);
            Cube(parent, name + "_Hopper", new Vector3(1.5f, 1.15f, 1.35f), position + new Vector3(0f, 2.45f, 0.15f), Quaternion.identity, material, body);
        }

        public static void Buttress(Transform parent, string name, Vector3 basePosition, Vector3 outward, float height, Material material, Color color)
        {
            Vector3 direction = outward.sqrMagnitude > 0.001f ? outward.normalized : Vector3.forward;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(-16f, 0f, 0f);
            Vector3 center = basePosition + (Vector3.up * (height * 0.42f)) + (direction * (height * 0.16f));
            Cube(parent, name, new Vector3(1.15f, height, 0.7f), center, rotation, material, color);
        }

        public static void Lamp(Transform parent, string name, Vector3 position, Color color, float range, float intensity)
        {
            GameObject lamp = new GameObject(name);
            lamp.transform.SetParent(parent, false);
            lamp.transform.position = position;
            Light light = lamp.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.range = range;
            light.intensity = intensity;
            light.shadows = LightShadows.None;
        }

        public static void Beam(Transform parent, string name, float x, float z, float y, float width, float depth, Material material, Color color)
        {
            Cube(parent, name, new Vector3(width, 0.45f, depth), new Vector3(x, y, z), Quaternion.identity, material, color);
        }

        public static void Cover(Transform parent, string name, float x, float z, Material material, Color color)
        {
            CoverOn(parent, name, x, z, 0f, material, color);
        }

        public static void CoverOn(Transform parent, string name, float x, float z, float yBase, Material material, Color color)
        {
            Cube(parent, name, new Vector3(2.2f, 1.2f, 1.4f), new Vector3(x, yBase + 0.6f, z), Quaternion.identity, material, color);
        }

        public static void Stairs(Transform parent, string name, Vector3 start, Vector3 direction, int steps, float rise, float run, float width, Material material, Color color)
        {
            Vector3 flat = direction.sqrMagnitude > 0.001f ? direction.normalized : Vector3.forward;
            bool alongX = Mathf.Abs(flat.x) > Mathf.Abs(flat.z);
            for (int i = 0; i < steps; i++)
            {
                Vector3 center = start + (flat * (run * (i + 0.5f))) + (Vector3.up * (rise * (i + 0.5f)));
                Vector3 size = alongX ? new Vector3(run, rise * (i + 1), width) : new Vector3(width, rise * (i + 1), run);
                center.y = start.y + (rise * (i + 1) * 0.5f);
                Cube(parent, name + "_" + i, size, center, Quaternion.identity, material, color);
            }
        }

        public static void Arch(Transform parent, string name, Vector3 baseCenter, Vector3 normal, float width, float depth, Material material, Color color, float jambHeight = 3.3f)
        {
            Vector3 forward = normal.sqrMagnitude > 0.001f ? normal.normalized : Vector3.forward;
            Vector3 side = Vector3.Cross(Vector3.up, forward).normalized;
            bool spanX = Mathf.Abs(side.x) >= Mathf.Abs(side.z);
            Vector3 jamb = spanX ? new Vector3(0.95f, jambHeight, depth) : new Vector3(depth, jambHeight, 0.95f);
            Cube(parent, name + "_L", jamb, baseCenter - (side * (width * 0.5f)) + (Vector3.up * (jambHeight * 0.5f)), Quaternion.identity, material, color);
            Cube(parent, name + "_R", jamb, baseCenter + (side * (width * 0.5f)) + (Vector3.up * (jambHeight * 0.5f)), Quaternion.identity, material, color);
            int segments = 5;
            float radius = width * 0.48f;
            float spring = jambHeight * 0.78f;
            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Lerp(18f, 162f, i / (float)(segments - 1)) * Mathf.Deg2Rad;
                Vector3 radial = (side * Mathf.Cos(angle)) + (Vector3.up * Mathf.Sin(angle));
                Vector3 point = baseCenter + (Vector3.up * spring) + (radial * radius);
                Quaternion rotation = Quaternion.LookRotation(forward, radial);
                float thickness = i == segments / 2 ? 0.85f : 0.55f;
                Vector3 stone = spanX ? new Vector3(1.05f, thickness, depth) : new Vector3(depth, thickness, 1.05f);
                Cube(parent, name + "_Seg" + i, stone, point, rotation, material, color);
            }
        }

        public static void Marker(Transform parent, string name, Vector3 position)
        {
            GameObject marker = new GameObject(name);
            marker.transform.SetParent(parent, false);
            marker.transform.position = position;
        }

        public static void Pickup(Transform parent, string name, Vector3 position, PickupDefinition definition, Material material, Color color)
        {
            GameObject item = new GameObject(name);
            item.transform.SetParent(parent, false);
            item.transform.position = position;
            SphereCollider trigger = item.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.85f;
            ProBuilderMesh gem = Cube(item.transform, name + "_Gem", new Vector3(0.45f, 0.45f, 0.45f), new Vector3(0f, 0.7f, 0f), Quaternion.identity, material, color);
            PickupController pickup = item.AddComponent<PickupController>();
            SerializedObject data = new SerializedObject(pickup);
            data.FindProperty("definition").objectReferenceValue = definition;
            data.FindProperty("visual").objectReferenceValue = gem.transform;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void BindSpawn(GameObject player, Transform spawn)
        {
            if (player == null || spawn == null)
            {
                return;
            }

            PlayerLife life = player.GetComponent<PlayerLife>();
            if (life == null)
            {
                life = player.AddComponent<PlayerLife>();
            }

            SerializedObject data = new SerializedObject(life);
            data.FindProperty("spawnPoint").objectReferenceValue = spawn;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        static ProBuilderMesh Cube(Transform parent, string name, Vector3 size, Vector3 position, Quaternion rotation, Material material, Color color)
        {
            return Finish(ShapeGenerator.GenerateCube(PivotLocation.Center, size), parent, name, size, position, rotation, material, color);
        }

        static ProBuilderMesh Finish(ProBuilderMesh mesh, Transform parent, string name, Vector3 colliderSize, Vector3 position, Quaternion rotation, Material material, Color color)
        {
            mesh.gameObject.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.SetPositionAndRotation(position, rotation);
            mesh.SetMaterial(mesh.faces, material);
            IList<Vector3> positions = mesh.positions;
            Color[] colors = new Color[mesh.vertexCount];
            for (int i = 0; i < colors.Length; i++)
            {
                Vector3 world = mesh.transform.TransformPoint(positions[i]);
                float shade = 0.84f + (Hash(world) * 0.24f);
                colors[i] = new Color(
                    Mathf.Clamp01(color.r * shade),
                    Mathf.Clamp01(color.g * shade),
                    Mathf.Clamp01(color.b * shade),
                    1f);
            }

            mesh.colors = colors;
            mesh.ToMesh();
            mesh.Refresh();
            MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = ShadowCastingMode.On;
            }

            StaticEditorFlags flags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccludeeStatic;
            if (colliderSize.x > 6f || colliderSize.y > 4f || colliderSize.z > 6f)
            {
                flags |= StaticEditorFlags.OccluderStatic;
            }

            GameObjectUtility.SetStaticEditorFlags(mesh.gameObject, flags);
            MeshCollider meshCollider = mesh.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                Object.DestroyImmediate(meshCollider);
            }

            BoxCollider box = mesh.gameObject.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = colliderSize;
            return mesh;
        }

        static float Hash(Vector3 point)
        {
            float noise = Mathf.Sin((point.x * 12.9898f) + (point.y * 78.233f) + (point.z * 37.719f)) * 43758.5453f;
            return noise - Mathf.Floor(noise);
        }
    }
}
