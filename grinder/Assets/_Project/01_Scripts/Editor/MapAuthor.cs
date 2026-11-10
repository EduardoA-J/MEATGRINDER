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
            ReplaceRoot("M01_Expansion");
            GameObject root = new GameObject("M01_Expansion");
            Transform yard = root.transform;

            Color concrete = new Color(0.28f, 0.24f, 0.22f);
            Color rust = new Color(0.45f, 0.18f, 0.12f);
            Color metal = new Color(0.16f, 0.16f, 0.17f);
            Color blood = new Color(0.42f, 0.05f, 0.06f);
            Color cover = new Color(0.22f, 0.2f, 0.18f);

            // East and west flanks around the tested pad. The north wall of that pad stays.
            Block.Slab(yard, "PB_EastFlank", 24f, 2f, 0f, 20f, 28f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_WestFlank", -24f, 2f, 0f, 20f, 28f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_EastLink", 24f, 24f, 0f, 12f, 16f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_WestLink", -24f, 24f, 0f, 12f, 16f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_Arena", 0f, 46f, 0f, 40f, 32f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_SouthBayLeft", -20f, -12f, 0f, 18f, 12f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_SouthBayRight", 20f, -12f, 0f, 18f, 12f, 0.4f, material, concrete);

            Block.WallZ(yard, "PB_ArenaNorth", 0f, 62f, 40f, 6f, 0f, 0.5f, material, metal);
            Block.WallZ(yard, "PB_ArenaSouthLeft", -14f, 30f, 12f, 5f, 0f, 0.5f, material, metal);
            Block.WallZ(yard, "PB_ArenaSouthRight", 14f, 30f, 12f, 5f, 0f, 0.5f, material, metal);
            Block.WallX(yard, "PB_ArenaWest", -20f, 46f, 32f, 5f, 0f, 0.5f, material, metal);
            Block.WallX(yard, "PB_ArenaEast", 20f, 46f, 32f, 5f, 0f, 0.5f, material, metal);
            Block.WallX(yard, "PB_WestHall", -18f, 8f, 22f, 4f, 0f, 0.4f, material, rust);
            Block.WallX(yard, "PB_EastYard", 30f, 8f, 24f, 4f, 0f, 0.4f, material, rust);
            Block.WallZ(yard, "PB_SouthDivide", 0f, -16f, 16f, 3.5f, 0f, 0.4f, material, metal);

            Block.Column(yard, "PB_ColA", -12f, 38f, 6f, material, metal);
            Block.Column(yard, "PB_ColB", 12f, 38f, 6f, material, metal);
            Block.Column(yard, "PB_ColC", -12f, 54f, 6f, material, metal);
            Block.Column(yard, "PB_ColD", 12f, 54f, 6f, material, metal);
            Block.Beam(yard, "PB_BeamNS", 0f, 46f, 5.6f, 0.5f, 28f, material, rust);
            Block.Beam(yard, "PB_BeamEW", 0f, 46f, 5.6f, 36f, 0.5f, material, rust);
            Block.Pipe(yard, "PB_PipeEast", 28.5f, 10f, 3.2f, 0.45f, 18f, material, blood);
            Block.Pipe(yard, "PB_PipeWest", -28.5f, 10f, 2.4f, 0.45f, 16f, material, blood);

            Block.Stairs(yard, "PB_EastStairs", new Vector3(24f, 0f, 18f), Vector3.forward, 10, 0.3f, 0.45f, 4f, material, concrete);
            Block.Slab(yard, "PB_EastLanding", 20f, 30f, 3f, 12f, 16f, 0.3f, material, metal);
            Block.Stairs(yard, "PB_WestStairs", new Vector3(-24f, 0f, 28f), Vector3.forward, 10, 0.3f, 0.45f, 4f, material, concrete);
            Block.Slab(yard, "PB_WestLanding", -20f, 42f, 3f, 12f, 20f, 0.3f, material, metal);
            Block.Slab(yard, "PB_Catwalk", 0f, 46f, 3f, 28f, 4f, 0.3f, material, metal);
            Block.Slab(yard, "PB_CatwalkEast", 16f, 40f, 3f, 4f, 14f, 0.3f, material, metal);
            Block.Slab(yard, "PB_CatwalkWest", -16f, 52f, 3f, 4f, 12f, 0.3f, material, metal);
            Block.WallX(yard, "PB_CatwalkRailE", 18.2f, 40f, 14f, 1.1f, 3f, 0.2f, material, rust);
            Block.WallX(yard, "PB_CatwalkRailW", -18.2f, 52f, 12f, 1.1f, 3f, 0.2f, material, rust);

            Block.Cover(yard, "PB_CoverA", -6f, 42f, material, cover);
            Block.Cover(yard, "PB_CoverB", 6f, 50f, material, cover);
            Block.Cover(yard, "PB_CoverC", 0f, 46f, material, cover);
            Block.Cover(yard, "PB_CoverD", -8f, 54f, material, cover);

            Transform spawns = new GameObject("M01_Spawns").transform;
            spawns.SetParent(root.transform, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-22f, 0.1f, -12f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(22f, 0.1f, -12f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-12f, 0.1f, 56f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(12f, 0.1f, 56f));

            Transform items = new GameObject("M01_Pickups").transform;
            items.SetParent(root.transform, false);
            Block.Pickup(items, "Pickup_Health_West", new Vector3(-24f, 0.4f, 6f), kit.Health, material, new Color(0.7f, 0.15f, 0.15f));
            Block.Pickup(items, "Pickup_Armor_Catwalk", new Vector3(0f, 3.4f, 46f), kit.Armor, material, new Color(0.2f, 0.45f, 0.75f));
            Block.Pickup(items, "Pickup_Quad_Arena", new Vector3(0f, 0.4f, 50f), kit.Quad, material, new Color(0.95f, 0.85f, 0.2f));
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-24f, 0.4f, 20f), kit.Berserk, material, new Color(0.7f, 0.1f, 0.35f));
            Block.Pickup(items, "Pickup_Rockets_East", new Vector3(24f, 0.4f, 8f), kit.Rockets, material, new Color(0.85f, 0.4f, 0.15f));

            GameObject player = GameObject.Find("LocalPlayer");
            if (player != null)
            {
                Block.BindSpawn(player, spawns.Find("Spawn_Human_A"));
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
            LightMood(new Color(0.55f, 0.58f, 0.62f), new Color(0.22f, 0.22f, 0.24f), 1.05f);
            GameObject root = new GameObject("M02_Castle");
            Transform yard = root.transform;
            Color stone = new Color(0.38f, 0.36f, 0.33f);
            Color dark = new Color(0.2f, 0.18f, 0.17f);
            Color moss = new Color(0.28f, 0.32f, 0.24f);

            Block.Slab(yard, "PB_Courtyard", 0f, 0f, 0f, 36f, 36f, 0.5f, material, stone);
            Block.Slab(yard, "PB_NorthHall", 0f, 28f, 0f, 18f, 16f, 0.5f, material, dark);
            Block.Slab(yard, "PB_SouthYard", 0f, -28f, 0f, 22f, 14f, 0.5f, material, moss);
            Block.WallZ(yard, "PB_NorthWallL", -12f, 18f, 12f, 8f, 0f, 1.2f, material, dark);
            Block.WallZ(yard, "PB_NorthWallR", 12f, 18f, 12f, 8f, 0f, 1.2f, material, dark);
            Block.WallZ(yard, "PB_SouthWallL", -12f, -18f, 12f, 7f, 0f, 1.2f, material, dark);
            Block.WallZ(yard, "PB_SouthWallR", 12f, -18f, 12f, 7f, 0f, 1.2f, material, dark);
            Block.WallX(yard, "PB_WestCurtain", -18f, 0f, 24f, 8f, 0f, 1.2f, material, dark);
            Block.WallX(yard, "PB_EastCurtain", 18f, 0f, 24f, 8f, 0f, 1.2f, material, dark);
            Block.Arch(yard, "PB_NorthGate", new Vector3(0f, 0f, 18f), Vector3.forward, 4.5f, 2.2f, material, stone);
            Block.Arch(yard, "PB_SouthGate", new Vector3(0f, 0f, -18f), Vector3.forward, 4.5f, 2.2f, material, stone);

            Tower(yard, "PB_TowerNW", new Vector3(-18f, 0f, 18f), material, stone, dark);
            Tower(yard, "PB_TowerNE", new Vector3(18f, 0f, 18f), material, stone, dark);
            Tower(yard, "PB_TowerSW", new Vector3(-18f, 0f, -18f), material, stone, dark);
            Tower(yard, "PB_TowerSE", new Vector3(18f, 0f, -18f), material, stone, dark);
            Block.Slab(yard, "PB_Bridge", 0f, 18f, 3f, 36f, 3.2f, 0.4f, material, dark);
            Block.WallZ(yard, "PB_BridgeRailN", 0f, 19.6f, 28f, 1.1f, 3f, 0.25f, material, stone);
            Block.WallZ(yard, "PB_BridgeRailS", 0f, 16.4f, 28f, 1.1f, 3f, 0.25f, material, stone);
            Block.Stairs(yard, "PB_HallStairs", new Vector3(-6f, 0f, 22f), Vector3.forward, 10, 0.3f, 0.45f, 3.5f, material, stone);
            Block.Slab(yard, "PB_HallLoft", 0f, 30f, 3f, 14f, 8f, 0.35f, material, dark);
            Block.Slab(yard, "PB_HallRoof", 0f, 28f, 6.5f, 18f, 16f, 0.4f, material, dark);
            Block.Cover(yard, "PB_CourtCoverA", -6f, -4f, material, moss);
            Block.Cover(yard, "PB_CourtCoverB", 7f, 6f, material, moss);
            Block.Column(yard, "PB_HallColA", -6f, 26f, 6f, material, stone);
            Block.Column(yard, "PB_HallColB", 6f, 26f, 6f, material, stone);

            Transform spawns = new GameObject("M02_Spawns").transform;
            spawns.SetParent(root.transform, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-16f, 0.1f, -16f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(16f, 0.1f, -16f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-6f, 0.1f, 30f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(6f, 0.1f, 30f));
            Transform items = new GameObject("M02_Pickups").transform;
            items.SetParent(root.transform, false);
            Block.Pickup(items, "Pickup_Health_South", new Vector3(0f, 0.4f, -28f), kit.Health, material, new Color(0.7f, 0.15f, 0.15f));
            Block.Pickup(items, "Pickup_Armor_Bridge", new Vector3(0f, 3.5f, 18f), kit.Armor, material, new Color(0.2f, 0.45f, 0.75f));
            Block.Pickup(items, "Pickup_Quad_Court", new Vector3(0f, 0.4f, 0f), kit.Quad, material, new Color(0.95f, 0.85f, 0.2f));
            Block.Pickup(items, "Pickup_Berserk_Hall", new Vector3(0f, 0.4f, 30f), kit.Berserk, material, new Color(0.7f, 0.1f, 0.35f));
            Block.Pickup(items, "Pickup_Rockets_Yard", new Vector3(8f, 0.4f, -28f), kit.Rockets, material, new Color(0.85f, 0.4f, 0.15f));

            GameObject player = PlacePlayer(playerPrefab, new Vector3(0f, 0.2f, -10f), spawns.Find("Spawn_Human_A"));
            Finish(scene, path, player);
            Debug.Log("[Maps] M02 castle saved.");
        }

        static void BuildRuins(Material material, PickupKit kit, GameObject playerPrefab)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M03/M03_RuinsOfTheForgotten.unity";
            Scene scene = NewScene(path);
            LightMood(new Color(1f, 0.93f, 0.8f), new Color(0.45f, 0.4f, 0.32f), 1.25f);
            GameObject root = new GameObject("M03_Ruins");
            Transform yard = root.transform;
            Color sand = new Color(0.62f, 0.52f, 0.38f);
            Color bone = new Color(0.75f, 0.7f, 0.58f);
            Color shadow = new Color(0.33f, 0.28f, 0.22f);

            Block.Slab(yard, "PB_PlazaNorth", 0f, 19f, 0f, 56f, 26f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaSouth", 0f, -19f, 0f, 56f, 26f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaEast", 17f, 0f, 0f, 22f, 12f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaWest", -17f, 0f, 0f, 22f, 12f, 0.5f, material, sand);
            Block.Slab(yard, "PB_Bowl", 0f, 0f, -1.6f, 16f, 16f, 0.4f, material, shadow);
            Block.Stairs(yard, "PB_BowlNorth", new Vector3(0f, -1.6f, 6f), Vector3.forward, 6, 0.28f, 0.5f, 4f, material, sand);
            Block.Stairs(yard, "PB_BowlSouth", new Vector3(0f, -1.6f, -6f), Vector3.back, 6, 0.28f, 0.5f, 4f, material, sand);
            Block.Slab(yard, "PB_TempleFloor", 0f, 34f, 3f, 22f, 16f, 0.45f, material, bone);
            Block.Stairs(yard, "PB_TempleStairs", new Vector3(0f, 0f, 24f), Vector3.forward, 10, 0.3f, 0.55f, 8f, material, bone);
            Block.Slab(yard, "PB_WestTerrace", -28f, 6f, 2.4f, 14f, 22f, 0.4f, material, sand);
            Block.Slab(yard, "PB_EastTerrace", 28f, -4f, 2.4f, 14f, 20f, 0.4f, material, sand);
            Block.Stairs(yard, "PB_WestTerraceStairs", new Vector3(-18f, 0f, 6f), Vector3.left, 8, 0.3f, 0.5f, 4f, material, sand);
            Block.Stairs(yard, "PB_EastTerraceStairs", new Vector3(18f, 0f, -4f), Vector3.right, 8, 0.3f, 0.5f, 4f, material, sand);
            Block.Slab(yard, "PB_SkyBridge", 0f, 34f, 6.2f, 20f, 3f, 0.35f, material, shadow);
            Block.Column(yard, "PB_GiantA", -8f, 34f, 9f, material, bone);
            Block.Column(yard, "PB_GiantB", 8f, 34f, 7f, material, bone);
            Block.Column(yard, "PB_BrokenA", -16f, 8f, 4f, material, bone);
            Block.Column(yard, "PB_BrokenB", 14f, -12f, 3.2f, material, bone);
            Block.Column(yard, "PB_BrokenC", -6f, -18f, 5f, material, bone);
            Block.Arch(yard, "PB_WestArch", new Vector3(-20f, 0f, 6f), Vector3.right, 5f, 3f, material, bone);
            Block.Arch(yard, "PB_EastArch", new Vector3(20f, 0f, -4f), Vector3.right, 5f, 3f, material, bone);
            Block.WallZ(yard, "PB_TempleBack", 0f, 42f, 22f, 5f, 3f, 0.8f, material, shadow);
            Block.WallZ(yard, "PB_TempleFrontL", -10f, 30f, 8f, 3.2f, 3f, 0.6f, material, shadow);
            Block.WallZ(yard, "PB_TempleFrontR", 10f, 30f, 8f, 3.2f, 3f, 0.6f, material, shadow);
            Block.Cover(yard, "PB_RubbleA", -8f, -6f, material, shadow);
            Block.Cover(yard, "PB_RubbleB", 10f, 8f, material, shadow);
            Block.Cover(yard, "PB_RubbleC", 4f, -16f, material, shadow);

            Transform spawns = new GameObject("M03_Spawns").transform;
            spawns.SetParent(root.transform, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-28f, 2.5f, 12f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(28f, 2.5f, -10f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-6f, 3.1f, 36f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(6f, 3.1f, 36f));
            Transform items = new GameObject("M03_Pickups").transform;
            items.SetParent(root.transform, false);
            Block.Pickup(items, "Pickup_Health_Bowl", new Vector3(0f, -1.1f, 0f), kit.Health, material, new Color(0.7f, 0.15f, 0.15f));
            Block.Pickup(items, "Pickup_Armor_Temple", new Vector3(0f, 3.5f, 36f), kit.Armor, material, new Color(0.2f, 0.45f, 0.75f));
            Block.Pickup(items, "Pickup_Quad_Bridge", new Vector3(0f, 6.7f, 34f), kit.Quad, material, new Color(0.95f, 0.85f, 0.2f));
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-28f, 2.9f, 0f), kit.Berserk, material, new Color(0.7f, 0.1f, 0.35f));
            Block.Pickup(items, "Pickup_Rockets_East", new Vector3(28f, 2.9f, 2f), kit.Rockets, material, new Color(0.85f, 0.4f, 0.15f));

            PlacePlayer(playerPrefab, new Vector3(-28f, 2.8f, 6f), spawns.Find("Spawn_Human_A"));
            Finish(scene, path, null);
            Debug.Log("[Maps] M03 ruins saved.");
        }

        static void BuildVoid(Material material, PickupKit kit, GameObject playerPrefab)
        {
            const string path = "Assets/_Project/00_Scenes/Maps/M04/M04_TheLongestVoid.unity";
            Scene scene = NewScene(path);
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.03f, 0.04f, 0.07f);
            Light sun = Object.FindAnyObjectByType<Light>();
            if (sun != null)
            {
                sun.color = new Color(0.65f, 0.75f, 0.9f);
                sun.intensity = 0.85f;
                sun.transform.rotation = Quaternion.Euler(38f, 20f, 0f);
            }

            GameObject root = new GameObject("M04_Void");
            Transform yard = root.transform;
            Color hull = new Color(0.08f, 0.09f, 0.11f);
            Color trim = new Color(0.15f, 0.55f, 0.7f);
            Color deck = new Color(0.12f, 0.13f, 0.16f);

            Block.Slab(yard, "PB_Core", 0f, 0f, 4f, 16f, 16f, 0.5f, material, deck);
            Block.Slab(yard, "PB_NorthIsle", 0f, 26f, 4f, 12f, 10f, 0.45f, material, hull);
            Block.Slab(yard, "PB_SouthIsle", 0f, -26f, 4f, 12f, 10f, 0.45f, material, hull);
            Block.Slab(yard, "PB_EastIsle", 26f, 0f, 4f, 10f, 12f, 0.45f, material, hull);
            Block.Slab(yard, "PB_WestIsle", -26f, 0f, 4f, 10f, 12f, 0.45f, material, hull);
            Block.Slab(yard, "PB_BridgeNorth", 0f, 16f, 4f, 3.2f, 18f, 0.3f, material, trim);
            Block.Slab(yard, "PB_BridgeSouth", 0f, -16f, 4f, 3.2f, 18f, 0.3f, material, trim);
            Block.Slab(yard, "PB_BridgeEast", 16f, 0f, 4f, 18f, 3.2f, 0.3f, material, trim);
            Block.Slab(yard, "PB_BridgeWest", -16f, 0f, 4f, 18f, 3.2f, 0.3f, material, trim);
            Block.Slab(yard, "PB_HighNorth", 0f, 26f, 8.5f, 8f, 6f, 0.35f, material, trim);
            Block.WallZ(yard, "PB_HighBlind", 0f, 24f, 8f, 2.4f, 8.5f, 0.4f, material, trim);
            Block.Slab(yard, "PB_HighEast", 26f, 0f, 8.5f, 8f, 8f, 0.35f, material, trim);
            Block.Stairs(yard, "PB_NorthStairs", new Vector3(-2f, 4f, 22f), Vector3.forward, 15, 0.3f, 0.32f, 2.4f, material, deck);
            Block.Stairs(yard, "PB_EastStairs", new Vector3(23f, 4f, -2f), Vector3.right, 15, 0.3f, 0.32f, 2.4f, material, deck);
            Block.ColumnOn(yard, "PB_MastA", -5f, -5f, 3.4f, 4f, material, trim);
            Block.ColumnOn(yard, "PB_MastB", 5f, 5f, 3.4f, 4f, material, trim);
            Block.ColumnOn(yard, "PB_SightNorth", 0f, 22f, 4f, 4f, material, trim);
            Block.CoverOn(yard, "PB_CoreCoverA", -3f, 2f, 4f, material, hull);
            Block.CoverOn(yard, "PB_CoreCoverB", 3f, -2f, 4f, material, hull);
            Block.Slab(yard, "PB_LowWest", -26f, 10f, 1f, 8f, 6f, 0.4f, material, hull);
            Block.Stairs(yard, "PB_LowStairs", new Vector3(-26f, 1f, 8f), Vector3.back, 10, 0.3f, 0.4f, 2.4f, material, deck);

            Transform spawns = new GameObject("M04_Spawns").transform;
            spawns.SetParent(root.transform, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-4f, 4.1f, -4f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(4f, 4.1f, 4f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(0f, 8.6f, 26f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(26f, 8.6f, 0f));
            Transform items = new GameObject("M04_Pickups").transform;
            items.SetParent(root.transform, false);
            Block.Pickup(items, "Pickup_Health_Core", new Vector3(0f, 4.5f, 0f), kit.Health, material, new Color(0.7f, 0.15f, 0.15f));
            Block.Pickup(items, "Pickup_Armor_High", new Vector3(0f, 9f, 26f), kit.Armor, material, new Color(0.2f, 0.45f, 0.75f));
            Block.Pickup(items, "Pickup_Quad_East", new Vector3(26f, 4.5f, 0f), kit.Quad, material, new Color(0.95f, 0.85f, 0.2f));
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-26f, 4.5f, 0f), kit.Berserk, material, new Color(0.7f, 0.1f, 0.35f));
            Block.Pickup(items, "Pickup_Rockets_South", new Vector3(0f, 4.5f, -26f), kit.Rockets, material, new Color(0.85f, 0.4f, 0.15f));

            GameObject player = PlacePlayer(playerPrefab, new Vector3(0f, 4.4f, -2f), spawns.Find("Spawn_Human_A"));
            Camera view = player.GetComponentInChildren<Camera>();
            if (view != null)
            {
                view.clearFlags = CameraClearFlags.SolidColor;
                view.backgroundColor = new Color(0.01f, 0.01f, 0.03f);
            }

            Finish(scene, path, null);
            Debug.Log("[Maps] M04 void saved. No ground plane.");
        }

        static void Tower(Transform parent, string name, Vector3 origin, Material material, Color stone, Color dark)
        {
            float x = origin.x;
            float z = origin.z;
            Block.WallX(parent, name + "_W", x - 3.2f, z, 6.4f, 9f, 0f, 0.6f, material, stone);
            Block.WallX(parent, name + "_E", x + 3.2f, z, 6.4f, 9f, 0f, 0.6f, material, stone);
            Block.WallZ(parent, name + "_N", x, z + 3.2f, 6.4f, 9f, 0f, 0.6f, material, stone);
            Block.WallZ(parent, name + "_S", x, z - 3.2f, 5f, 9f, 0f, 0.6f, material, stone);
            Block.Slab(parent, name + "_Mid", x, z, 3f, 5.2f, 5.2f, 0.3f, material, dark);
            Block.Slab(parent, name + "_Top", x, z, 6f, 5.2f, 5.2f, 0.3f, material, dark);
            Block.Stairs(parent, name + "_Stairs", origin + new Vector3(-1.2f, 0f, -1.5f), Vector3.forward, 10, 0.3f, 0.35f, 2f, material, stone);
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

        static void LightMood(Color sunColor, Color ambient, float intensity)
        {
            Light sun = Object.FindAnyObjectByType<Light>();
            if (sun != null)
            {
                sun.color = sunColor;
                sun.intensity = intensity;
                sun.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
            }

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = ambient;
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

        sealed class PickupKit
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
            Cube(parent, name, new Vector3(1.1f, height, 1.1f), new Vector3(x, yBase + (height * 0.5f), z), Quaternion.identity, material, color);
        }

        public static void Beam(Transform parent, string name, float x, float z, float y, float width, float depth, Material material, Color color)
        {
            Cube(parent, name, new Vector3(width, 0.45f, depth), new Vector3(x, y, z), Quaternion.identity, material, color);
        }

        public static void Pipe(Transform parent, string name, float x, float z, float y, float radius, float length, Material material, Color color)
        {
            Cube(parent, name, new Vector3(radius, radius, length), new Vector3(x, y, z), Quaternion.identity, material, color);
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

        public static void Arch(Transform parent, string name, Vector3 baseCenter, Vector3 normal, float width, float depth, Material material, Color color)
        {
            Vector3 side = Vector3.Cross(Vector3.up, normal.sqrMagnitude > 0.001f ? normal.normalized : Vector3.forward).normalized;
            Cube(parent, name + "_L", new Vector3(0.7f, 3.2f, depth), baseCenter - (side * (width * 0.5f)) + (Vector3.up * 1.6f), Quaternion.identity, material, color);
            Cube(parent, name + "_R", new Vector3(0.7f, 3.2f, depth), baseCenter + (side * (width * 0.5f)) + (Vector3.up * 1.6f), Quaternion.identity, material, color);
            Cube(parent, name + "_Lint", new Vector3(width + 0.8f, 0.6f, depth), baseCenter + (Vector3.up * 3.5f), Quaternion.identity, material, color);
            int segments = 5;
            float radius = width * 0.45f;
            for (int i = 0; i < segments; i++)
            {
                float angle = Mathf.Lerp(25f, 155f, i / (float)(segments - 1)) * Mathf.Deg2Rad;
                Vector3 point = baseCenter + (Vector3.up * (1.2f + (Mathf.Sin(angle) * radius))) + (side * (Mathf.Cos(angle) * radius));
                Cube(parent, name + "_Seg" + i, new Vector3(0.7f, 0.45f, depth), point, Quaternion.identity, material, color);
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
            ProBuilderMesh mesh = ShapeGenerator.GenerateCube(PivotLocation.Center, size);
            mesh.gameObject.name = name;
            mesh.transform.SetParent(parent, false);
            mesh.transform.SetPositionAndRotation(position, rotation);
            mesh.SetMaterial(mesh.faces, material);
            Color[] colors = new Color[mesh.vertexCount];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = color;
            }

            mesh.colors = colors;
            mesh.ToMesh();
            mesh.Refresh();
            MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }

            MeshCollider meshCollider = mesh.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                Object.DestroyImmediate(meshCollider);
            }

            BoxCollider box = mesh.gameObject.AddComponent<BoxCollider>();
            box.center = Vector3.zero;
            box.size = size;
            return mesh;
        }
    }
}
