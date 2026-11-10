using UnityEngine;

namespace Meatgrinder.Editor
{
    static class MapGeometry
    {
        static readonly Color HealthTint = new Color(0.7f, 0.15f, 0.15f);
        static readonly Color ArmorTint = new Color(0.2f, 0.45f, 0.75f);
        static readonly Color QuadTint = new Color(0.95f, 0.85f, 0.2f);
        static readonly Color BerserkTint = new Color(0.7f, 0.1f, 0.35f);
        static readonly Color RocketTint = new Color(0.85f, 0.4f, 0.15f);

        public static void Slaughterhouse(Transform yard, Material material, MapAuthor.PickupKit kit)
        {
            Color concrete = new Color(0.34f, 0.31f, 0.28f);
            Color cold = new Color(0.48f, 0.54f, 0.56f);
            Color rust = new Color(0.52f, 0.2f, 0.1f);
            Color metal = new Color(0.14f, 0.14f, 0.15f);
            Color blood = new Color(0.46f, 0.07f, 0.06f);
            Color cover = new Color(0.24f, 0.18f, 0.16f);

            Block.Slab(yard, "PB_EastApron", 31f, 2f, 0f, 34f, 36f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_WestApron", -31f, 2f, 0f, 34f, 36f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_SouthLip", 0f, -18f, 0f, 24f, 10f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_SouthDock", 0f, -36f, 0f, 64f, 28f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_EastCold", 59f, 14f, 0f, 28f, 48f, 0.4f, material, cold);
            Block.Slab(yard, "PB_WestHall", -52f, 8f, 0f, 22f, 56f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_WestPit", -71f, 14f, -1.2f, 20f, 18f, 0.4f, material, blood);
            Block.Slab(yard, "PB_EastThroat", 32f, 31f, 0f, 28f, 26f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_WestThroat", -32f, 31f, 0f, 24f, 26f, 0.4f, material, concrete);
            Block.Slab(yard, "PB_KillFloor", 0f, 60f, 0f, 80f, 40f, 0.4f, material, concrete);

            Flight(yard, "PB_PitStairs", new Vector3(-66f, -1.2f, 14f), Vector3.right, 4, 0.7f, 6f, material, concrete);
            Block.WallX(yard, "PB_PitCurbN", -63.2f, 20f, 6f, 1.3f, -1.2f, 0.4f, material, metal);
            Block.WallX(yard, "PB_PitCurbS", -63.2f, 8f, 6f, 1.3f, -1.2f, 0.4f, material, metal);

            Flight(yard, "PB_EastStairs", new Vector3(30f, 0f, 22f), Vector3.forward, 12, 0.5f, 4f, material, concrete);
            Block.Slab(yard, "PB_EastMezz", 34f, 41f, 3.6f, 10f, 32f, 0.35f, material, metal);
            Flight(yard, "PB_WestStairs", new Vector3(-34f, 0f, 26f), Vector3.forward, 12, 0.5f, 4f, material, concrete);
            Block.Slab(yard, "PB_WestMezz", -34f, 46f, 3.6f, 10f, 32f, 0.35f, material, metal);
            Block.Slab(yard, "PB_Catwalk", 0f, 63f, 3.6f, 52f, 8f, 0.3f, material, metal);
            Block.Slab(yard, "PB_CatwalkEast", 26f, 60f, 3.6f, 24f, 10f, 0.3f, material, metal);
            Block.Slab(yard, "PB_CatwalkWest", -26f, 60f, 3.6f, 24f, 10f, 0.3f, material, metal);
            Flight(yard, "PB_CraneStairs", new Vector3(0f, 3.6f, 64f), Vector3.forward, 8, 0.55f, 4f, material, concrete);
            Block.Slab(yard, "PB_CraneDeck", 0f, 74f, 6f, 18f, 12f, 0.35f, material, metal);
            Block.WallZ(yard, "PB_CatwalkRailNL", -18f, 66.6f, 24f, 1.05f, 3.6f, 0.16f, material, rust);
            Block.WallZ(yard, "PB_CatwalkRailNR", 18f, 66.6f, 24f, 1.05f, 3.6f, 0.16f, material, rust);
            Block.WallZ(yard, "PB_CatwalkRailS", 0f, 59.2f, 48f, 1.05f, 3.6f, 0.16f, material, rust);

            Block.WallZ(yard, "PB_KillNorth", 0f, 80f, 78f, 7f, 0f, 0.6f, material, metal);
            Block.WallZ(yard, "PB_ScreenWest", -16f, 68f, 14f, 4.5f, 0f, 0.55f, material, metal);
            Block.WallZ(yard, "PB_ScreenEast", 22f, 66f, 12f, 4.5f, 0f, 0.55f, material, metal);
            Block.WallX(yard, "PB_KillWest", -39f, 62f, 30f, 6f, 0f, 0.5f, material, rust);
            Block.WallX(yard, "PB_KillEast", 39f, 58f, 26f, 6f, 0f, 0.5f, material, rust);
            Block.WallZ(yard, "PB_ColdDoorA", 52f, 6f, 10f, 4f, 0f, 0.4f, material, cold);
            Block.WallZ(yard, "PB_ColdDoorB", 69f, 6f, 6f, 4f, 0f, 0.4f, material, cold);
            Block.WallZ(yard, "PB_ColdDoorC", 49f, 22f, 6f, 4f, 0f, 0.4f, material, cold);
            Block.WallZ(yard, "PB_ColdDoorD", 64f, 22f, 12f, 4f, 0f, 0.4f, material, cold);
            Block.WallX(yard, "PB_ColdOuter", 73f, 14f, 40f, 5f, 0f, 0.45f, material, metal);
            Block.WallX(yard, "PB_HallOuter", -63.5f, 8f, 40f, 5.5f, 0f, 0.45f, material, rust);
            Block.WallZ(yard, "PB_DockSouth", 0f, -50f, 60f, 4f, 0f, 0.45f, material, metal);

            Block.Pillar(yard, "PB_KillColA", -22f, 50f, 0f, 7f, 0.6f, material, metal, rust);
            Block.Pillar(yard, "PB_KillColB", 16f, 48f, 0f, 7f, 0.55f, material, metal, rust);
            Block.Pillar(yard, "PB_KillColC", -10f, 60f, 0f, 7.5f, 0.65f, material, metal, rust);
            Block.Pillar(yard, "PB_KillColD", 24f, 62f, 0f, 7f, 0.55f, material, metal, rust);
            Block.Pillar(yard, "PB_KillColE", -26f, 72f, 0f, 6.5f, 0.5f, material, metal, rust);
            Block.Pillar(yard, "PB_KillColF", 8f, 74f, 0f, 8f, 0.7f, material, metal, rust);
            Block.Pillar(yard, "PB_DockColA", -20f, -30f, 0f, 6f, 0.45f, material, metal, concrete);
            Block.Pillar(yard, "PB_DockColB", 18f, -34f, 0f, 6f, 0.45f, material, metal, concrete);
            Block.Pillar(yard, "PB_DockColC", -8f, -44f, 0f, 5.5f, 0.4f, material, metal, concrete);
            Block.Beam(yard, "PB_DockCrane", -1f, -32f, 5.6f, 42f, 0.55f, material, rust);
            Block.Beam(yard, "PB_KillBeam", 0f, 60f, 6.4f, 48f, 0.45f, material, rust);
            Block.Pipe(yard, "PB_PipeCold", new Vector3(58f, 4.8f, 16f), Vector3.forward, 0.32f, 30f, material, blood);
            Block.Pipe(yard, "PB_PipeKill", new Vector3(0f, 6.6f, 58f), Vector3.right, 0.28f, 46f, material, rust);
            Block.Pipe(yard, "PB_PipeHall", new Vector3(-52f, 4.4f, 10f), Vector3.forward, 0.3f, 26f, material, metal);
            Block.Machine(yard, "PB_PressA", new Vector3(-52f, 0f, -4f), material, metal, rust);
            Block.Machine(yard, "PB_PressB", new Vector3(-50f, 0f, 22f), material, metal, blood);
            Block.Machine(yard, "PB_PressC", new Vector3(62f, 0f, 28f), material, cold, metal);
            Block.Cover(yard, "PB_CoverA", -8f, 52f, material, cover);
            Block.Cover(yard, "PB_CoverB", 12f, 64f, material, cover);
            Block.Cover(yard, "PB_CoverC", 4f, 72f, material, cover);
            Block.Cover(yard, "PB_CoverD", -20f, 66f, material, cover);
            Block.Arch(yard, "PB_ThroatArch", new Vector3(24f, 0f, 22f), Vector3.forward, 5f, 1.6f, material, rust);

            Transform lights = new GameObject("M01_Lights").transform;
            lights.SetParent(yard, false);
            Block.Lamp(lights, "Lamp_Kill", new Vector3(0f, 8.5f, 60f), new Color(1f, 0.72f, 0.42f), 30f, 2.4f);
            Block.Lamp(lights, "Lamp_Cold", new Vector3(60f, 5.5f, 14f), new Color(0.45f, 0.75f, 0.85f), 18f, 1.7f);
            Block.Lamp(lights, "Lamp_Hall", new Vector3(-52f, 5.5f, 12f), new Color(0.85f, 0.28f, 0.16f), 16f, 1.5f);
            Block.Lamp(lights, "Lamp_Dock", new Vector3(0f, 6.5f, -36f), new Color(1f, 0.78f, 0.5f), 22f, 1.3f);

            Transform spawns = new GameObject("M01_Spawns").transform;
            spawns.SetParent(yard, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-16f, 0.1f, -40f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(22f, 0.1f, -42f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-16f, 0.1f, 74f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(22f, 0.1f, 72f));

            Transform items = new GameObject("M01_Pickups").transform;
            items.SetParent(yard, false);
            Block.Pickup(items, "Pickup_Health_West", new Vector3(-54f, 0.45f, 2f), kit.Health, material, HealthTint);
            Block.Pickup(items, "Pickup_Armor_Catwalk", new Vector3(0f, 6.45f, 74f), kit.Armor, material, ArmorTint);
            Block.Pickup(items, "Pickup_Quad_Arena", new Vector3(2f, 0.45f, 64f), kit.Quad, material, QuadTint);
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-74f, -0.75f, 14f), kit.Berserk, material, BerserkTint);
            Block.Pickup(items, "Pickup_Rockets_East", new Vector3(62f, 0.45f, 8f), kit.Rockets, material, RocketTint);
        }

        public static void Castle(Transform yard, Material material, MapAuthor.PickupKit kit)
        {
            Color stone = new Color(0.5f, 0.47f, 0.42f);
            Color dark = new Color(0.18f, 0.16f, 0.15f);
            Color moss = new Color(0.3f, 0.34f, 0.24f);
            Color iron = new Color(0.28f, 0.24f, 0.21f);

            Block.Slab(yard, "PB_Bailey", 0f, 8f, 0f, 200f, 200f, 0.5f, material, stone);
            Block.Slab(yard, "PB_WardWest", -180f, 10f, 0f, 170f, 380f, 0.5f, material, moss);
            Block.Slab(yard, "PB_WardEast", 180f, 20f, 0f, 170f, 360f, 0.5f, material, stone);
            Block.Slab(yard, "PB_WardNorth", 0f, 190f, 0f, 540f, 180f, 0.5f, material, dark);
            Block.Slab(yard, "PB_WardSouthWest", -95f, -160f, 0f, 170f, 150f, 0.5f, material, moss);
            Block.Slab(yard, "PB_WardSouthEast", 100f, -160f, 0f, 180f, 150f, 0.5f, material, stone);
            Block.Slab(yard, "PB_SouthBarbican", 2f, -200f, 0f, 36f, 70f, 0.5f, material, dark);
            Block.Slab(yard, "PB_SouthNeck", 2f, -140f, 0f, 16f, 50f, 0.45f, material, stone);
            Block.Slab(yard, "PB_Narthex", 0f, 52f, 0f, 28f, 24f, 0.5f, material, dark);
            Block.Slab(yard, "PB_GreatHall", 0f, 78f, 0f, 56f, 44f, 0.5f, material, dark);
            Block.Slab(yard, "PB_Chapel", 46f, 8f, 0.6f, 18f, 16f, 0.6f, material, stone);
            Block.Slab(yard, "PB_Crypt", 2f, -108f, -2.4f, 22f, 20f, 0.45f, material, dark);
            Flight(yard, "PB_ChapelStairs", new Vector3(36f, 0f, 8f), Vector3.right, 2, 0.9f, 4f, material, stone);
            Flight(yard, "PB_CryptStairs", new Vector3(2f, -2.4f, -100f), Vector3.forward, 8, 1.5f, 4f, material, stone);

            Block.WallZ(yard, "PB_CryptNorthL", -4f, -98.2f, 8f, 2.6f, -2.4f, 0.45f, material, stone);
            Block.WallZ(yard, "PB_CryptNorthR", 8f, -98.2f, 8f, 2.6f, -2.4f, 0.45f, material, stone);
            Block.WallZ(yard, "PB_CryptSouth", 2f, -118f, 22f, 3.2f, -2.4f, 0.5f, material, stone);
            Block.WallX(yard, "PB_CryptWest", -9f, -108f, 20f, 3.2f, -2.4f, 0.5f, material, stone);
            Block.WallX(yard, "PB_CryptEast", 13f, -108f, 20f, 3.2f, -2.4f, 0.5f, material, stone);
            Block.WallX(yard, "PB_CryptStairW", -0.4f, -94f, 12f, 2.5f, -2.4f, 0.35f, material, stone);
            Block.WallX(yard, "PB_CryptStairE", 4.4f, -94f, 12f, 2.5f, -2.4f, 0.35f, material, stone);

            GiantTower(yard, "PB_TowerNW", new Vector3(-150f, 0f, 150f), true, true, false, material, stone, dark);
            GiantTower(yard, "PB_TowerNE", new Vector3(150f, 0f, 150f), true, false, true, material, stone, dark);
            GiantTower(yard, "PB_TowerSW", new Vector3(-150f, 0f, -140f), false, true, false, material, stone, dark);
            GiantTower(yard, "PB_TowerSE", new Vector3(150f, 0f, -140f), false, false, false, material, stone, dark);
            GiantTower(yard, "PB_Keep", new Vector3(-55f, 0f, -62f), false, false, true, material, stone, dark);
            Block.Slab(yard, "PB_Battlement", 0f, 150f, 12f, 280f, 14f, 0.4f, material, dark);
            Block.WallZ(yard, "PB_BattlementRailN", 0f, 156.6f, 250f, 1.1f, 12f, 0.22f, material, stone);
            Block.WallZ(yard, "PB_BattlementRailS", 0f, 143.4f, 250f, 1.1f, 12f, 0.22f, material, stone);
            Block.Slab(yard, "PB_HighWest", -105f, -62f, 12f, 80f, 10f, 0.4f, material, dark);
            Block.Slab(yard, "PB_HighSouth", -124f, -101f, 12f, 12f, 86f, 0.4f, material, dark);
            Block.Slab(yard, "PB_HighJoin", -134f, -140f, 12f, 16f, 14f, 0.4f, material, stone);

            WatchTower(yard, "PB_WatchNorth", new Vector3(80f, 0f, 250f), true, material, stone, dark);
            WatchTower(yard, "PB_WatchWest", new Vector3(-230f, 0f, 70f), true, material, stone, dark);
            WatchTower(yard, "PB_WatchEast", new Vector3(230f, 0f, -30f), true, material, stone, iron);
            WatchTower(yard, "PB_WatchSouth", new Vector3(70f, 0f, -200f), false, material, stone, dark);
            RuinTower(yard, "PB_RuinA", new Vector3(-110f, 0f, 60f), material, stone, moss);
            RuinTower(yard, "PB_RuinB", new Vector3(120f, 0f, 130f), material, stone, moss);
            RuinTower(yard, "PB_RuinC", new Vector3(-130f, 0f, -70f), material, stone, iron);
            RuinTower(yard, "PB_RuinD", new Vector3(130f, 0f, 40f), material, moss, stone);
            RuinTower(yard, "PB_RuinE", new Vector3(-24f, 0f, 230f), material, stone, dark);
            RuinTower(yard, "PB_RuinF", new Vector3(40f, 0f, -200f), material, moss, stone);

            Block.Slab(yard, "PB_WestRampart", -242f, 10f, 6f, 16f, 100f, 0.4f, material, dark);
            Flight(yard, "PB_WestRampartStairs", new Vector3(-226f, 0f, 8f), Vector3.left, 20, 0.48f, 4f, material, stone);
            Block.WallX(yard, "PB_WestRampartRail", -249.4f, 10f, 90f, 1.05f, 6f, 0.2f, material, stone);
            Block.Slab(yard, "PB_EastRampart", 242f, -16f, 6f, 16f, 80f, 0.4f, material, dark);
            Flight(yard, "PB_EastRampartStairs", new Vector3(226f, 0f, -16f), Vector3.right, 20, 0.48f, 4f, material, stone);
            Block.WallX(yard, "PB_EastRampartRail", 249.4f, -16f, 70f, 1.05f, 6f, 0.2f, material, stone);
            Block.Slab(yard, "PB_CloisterDeck", -78f, 3f, 3.6f, 8f, 38f, 0.35f, material, dark);
            Flight(yard, "PB_CloisterDeckStairs", new Vector3(-78f, 0f, -20f), Vector3.forward, 12, 0.5f, 3.2f, material, stone);
            Block.Slab(yard, "PB_ChapelLoft", 54f, 8f, 4.2f, 12f, 10f, 0.35f, material, iron);
            Flight(yard, "PB_ChapelLoftStairs", new Vector3(50f, 0.6f, 8f), Vector3.right, 12, 0.45f, 3f, material, stone);
            Block.Slab(yard, "PB_NorthDeck", 0f, 226f, 4.2f, 18f, 22f, 0.4f, material, dark);
            Flight(yard, "PB_NorthDeckStairs", new Vector3(0f, 0f, 208f), Vector3.forward, 14, 0.55f, 4f, material, stone);
            Block.Slab(yard, "PB_PierWest", -36f, 210f, 5.4f, 12f, 12f, 0.4f, material, stone);
            Block.Slab(yard, "PB_PierEast", 36f, 210f, 5.4f, 12f, 12f, 0.4f, material, stone);
            Block.Slab(yard, "PB_RuinSpan", 0f, 210f, 5.4f, 64f, 5f, 0.35f, material, dark);
            Flight(yard, "PB_PierWestStairs", new Vector3(-36f, 0f, 198f), Vector3.forward, 18, 0.42f, 3.4f, material, stone);
            Flight(yard, "PB_PierEastStairs", new Vector3(36f, 0f, 198f), Vector3.forward, 18, 0.42f, 3.4f, material, stone);
            Block.WallZ(yard, "PB_SpanRailN", 0f, 212.2f, 60f, 1f, 5.4f, 0.16f, material, stone);
            Block.WallZ(yard, "PB_SpanRailS", 0f, 207.8f, 60f, 1f, 5.4f, 0.16f, material, stone);

            Block.Slab(yard, "PB_GalleryWest", -16f, 80f, 3.6f, 5f, 28f, 0.3f, material, dark);
            Block.Slab(yard, "PB_GalleryEast", 16f, 76f, 3.6f, 5f, 22f, 0.3f, material, dark);
            Flight(yard, "PB_GalleryWestStairs", new Vector3(-16f, 0f, 66f), Vector3.forward, 12, 0.45f, 3f, material, stone);
            Flight(yard, "PB_GalleryEastStairs", new Vector3(16f, 0f, 66f), Vector3.forward, 12, 0.45f, 3f, material, stone);
            Block.WallZ(yard, "PB_HallScreen", -18f, 54f, 16f, 6f, 0f, 0.8f, material, stone);
            Block.WallZ(yard, "PB_Apse", 0f, 100f, 56f, 10f, 0f, 1f, material, dark);
            Block.WallX(yard, "PB_HallWest", -28f, 78f, 40f, 10f, 0f, 0.9f, material, stone);
            Block.WallX(yard, "PB_HallEast", 28f, 76f, 36f, 10f, 0f, 0.9f, material, stone);
            Block.WallX(yard, "PB_CurtainWestA", -100f, -50f, 36f, 7f, 0f, 1.3f, material, dark);
            Block.WallX(yard, "PB_CurtainWestB", -100f, 24f, 28f, 5.5f, 0f, 1.3f, material, dark);
            Block.WallX(yard, "PB_CurtainWestC", -100f, 78f, 22f, 4f, 0f, 1.2f, material, moss);
            Block.WallX(yard, "PB_CurtainEastA", 100f, -40f, 34f, 6.5f, 0f, 1.3f, material, dark);
            Block.WallX(yard, "PB_CurtainEastB", 100f, 36f, 26f, 5f, 0f, 1.3f, material, dark);
            Block.WallX(yard, "PB_CurtainEastC", 100f, 90f, 18f, 3.4f, 0f, 1.1f, material, moss);
            Block.WallZ(yard, "PB_CurtainSouthL", -40f, -92f, 70f, 8f, 0f, 1.4f, material, dark);
            Block.WallZ(yard, "PB_CurtainSouthR", 42f, -92f, 60f, 8f, 0f, 1.4f, material, dark);
            Block.WallX(yard, "PB_OuterWestN", -258f, 97f, 182f, 11f, 0f, 1.6f, material, dark);
            Block.WallX(yard, "PB_OuterWestS", -258f, -94f, 156f, 11f, 0f, 1.6f, material, dark);
            Block.WallX(yard, "PB_OuterEastN", 258f, 120f, 160f, 11f, 0f, 1.6f, material, dark);
            Block.WallX(yard, "PB_OuterEastS", 258f, -70f, 180f, 11f, 0f, 1.6f, material, dark);
            Block.WallZ(yard, "PB_OuterNorthL", -110f, 272f, 200f, 10f, 0f, 1.6f, material, dark);
            Block.WallZ(yard, "PB_OuterNorthR", 120f, 272f, 200f, 10f, 0f, 1.6f, material, dark);
            Block.WallZ(yard, "PB_OuterSouthL", -90f, -228f, 150f, 9f, 0f, 1.5f, material, dark);
            Block.WallZ(yard, "PB_OuterSouthR", 100f, -228f, 150f, 9f, 0f, 1.5f, material, dark);
            Block.WallZ(yard, "PB_NorthRuinA", -70f, 170f, 18f, 3.2f, 0f, 0.8f, material, moss);
            Block.WallZ(yard, "PB_NorthRuinB", 48f, 176f, 14f, 2.4f, 0f, 0.7f, material, stone);
            Block.WallX(yard, "PB_WestRuin", -190f, -20f, 16f, 2.8f, 0f, 0.8f, material, moss);
            Block.Arch(yard, "PB_SouthGate", new Vector3(0f, 0f, -92f), Vector3.forward, 10f, 2.6f, material, stone, 7f);
            Block.Arch(yard, "PB_NarthexArch", new Vector3(8f, 0f, 48f), Vector3.forward, 9f, 2.4f, material, stone, 6.5f);
            Block.Arch(yard, "PB_ChapelArch", new Vector3(46f, 0.6f, 1f), Vector3.forward, 7f, 2f, material, iron, 5f);
            Block.Arch(yard, "PB_ArchNW", new Vector3(-150f, 0f, 134f), Vector3.forward, 12f, 3f, material, stone, 8f);
            Block.Arch(yard, "PB_ArchNE", new Vector3(150f, 0f, 134f), Vector3.forward, 12f, 3f, material, stone, 8f);
            Block.Arch(yard, "PB_ArchSW", new Vector3(-150f, 0f, -124f), Vector3.forward, 12f, 3f, material, stone, 8f);
            Block.Arch(yard, "PB_ArchSE", new Vector3(150f, 0f, -124f), Vector3.forward, 11f, 2.8f, material, iron, 7.5f);
            Block.Arch(yard, "PB_KeepArch", new Vector3(-55f, 0f, -46f), Vector3.forward, 10f, 2.6f, material, stone, 7f);
            Block.Arch(yard, "PB_WestGate", new Vector3(-258f, 0f, -5f), Vector3.right, 12f, 3f, material, stone, 8f);
            Block.Arch(yard, "PB_EastGate", new Vector3(258f, 0f, 30f), Vector3.right, 12f, 3f, material, stone, 8f);
            Block.Arch(yard, "PB_NorthGate", new Vector3(0f, 0f, 272f), Vector3.forward, 14f, 3.2f, material, stone, 9f);
            Block.Arch(yard, "PB_BarbicanArch", new Vector3(0f, 0f, 200f), Vector3.forward, 10f, 2.4f, material, iron, 6.5f);
            Block.Arch(yard, "PB_ArcadeA", new Vector3(-32f, 0f, -16f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            Block.Arch(yard, "PB_ArcadeB", new Vector3(-32f, 0f, 0f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            Block.Arch(yard, "PB_ArcadeC", new Vector3(-32f, 0f, 16f), Vector3.forward, 6.5f, 2f, material, moss, 5f);
            Block.Arch(yard, "PB_ArcadeD", new Vector3(24f, 0f, -40f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            Block.Arch(yard, "PB_ArcadeE", new Vector3(24f, 0f, -24f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            Block.Arch(yard, "PB_ArcadeF", new Vector3(24f, 0f, -8f), Vector3.forward, 6.5f, 2f, material, moss, 5f);

            Block.Pillar(yard, "PB_HallColA", -10f, 68f, 0f, 10f, 0.85f, material, stone, dark);
            Block.Pillar(yard, "PB_HallColB", 12f, 72f, 0f, 10f, 0.8f, material, stone, dark);
            Block.Pillar(yard, "PB_HallColC", -10f, 86f, 0f, 10f, 0.85f, material, stone, dark);
            Block.Pillar(yard, "PB_HallColD", 12f, 92f, 0f, 9f, 0.75f, material, stone, dark);
            Block.Beam(yard, "PB_RibA", 0f, 74f, 9.2f, 40f, 0.5f, material, dark);
            Block.Beam(yard, "PB_RibB", 0f, 90f, 9.2f, 40f, 0.5f, material, dark);
            Block.Buttress(yard, "PB_ButtressW1", new Vector3(-30f, 0f, 70f), Vector3.left, 8f, material, stone);
            Block.Buttress(yard, "PB_ButtressW2", new Vector3(-30f, 0f, 88f), Vector3.left, 7f, material, stone);
            Block.Buttress(yard, "PB_ButtressE1", new Vector3(30f, 0f, 68f), Vector3.right, 8f, material, stone);
            Block.Buttress(yard, "PB_ButtressE2", new Vector3(30f, 0f, 86f), Vector3.right, 7f, material, moss);
            for (int i = 0; i < 6; i++)
            {
                Block.Pillar(yard, "PB_Cloister" + i, -42f, -18f + (i * 7f), 0f, 6f, 0.5f, material, stone, moss);
            }

            Block.Beam(yard, "PB_CloisterBeam", -42f, 2f, 6.2f, 0.45f, 42f, material, dark);
            Block.Cylinder(yard, "PB_FallenA", 0.7f, 11f, new Vector3(70f, 0.8f, 150f), Quaternion.Euler(0f, 18f, 84f), material, stone);
            Block.Cylinder(yard, "PB_FallenB", 0.55f, 8f, new Vector3(-160f, 0.7f, 20f), Quaternion.Euler(0f, -30f, 80f), material, moss);
            Block.Cylinder(yard, "PB_FallenC", 0.6f, 9f, new Vector3(24f, 0.75f, -180f), Quaternion.Euler(0f, 40f, 86f), material, stone);
            Block.Cylinder(yard, "PB_FallenD", 0.5f, 7f, new Vector3(96f, 0.65f, 24f), Quaternion.Euler(0f, 10f, 78f), material, iron);
            Block.Cover(yard, "PB_BaileyCoverA", -14f, -8f, material, moss);
            Block.Cover(yard, "PB_BaileyCoverB", 16f, 14f, material, moss);
            Block.Cover(yard, "PB_BaileyCoverC", -22f, 18f, material, stone);
            Block.Cover(yard, "PB_RubbleNorth", -8f, 168f, material, moss);
            Block.Cover(yard, "PB_RubbleWest", -200f, 40f, material, stone);
            Block.Cover(yard, "PB_RubbleEast", 168f, 60f, material, moss);
            Block.Cover(yard, "PB_RubbleSouth", -40f, -190f, material, dark);

            Transform lights = new GameObject("M02_Lights").transform;
            lights.SetParent(yard, false);
            Block.Lamp(lights, "Lamp_Bailey", new Vector3(6f, 5f, -8f), new Color(1f, 0.62f, 0.32f), 18f, 1.6f);
            Block.Lamp(lights, "Lamp_Hall", new Vector3(0f, 6f, 56f), new Color(1f, 0.5f, 0.28f), 20f, 1.8f);
            Block.Lamp(lights, "Lamp_Chapel", new Vector3(36f, 4f, 12f), new Color(0.95f, 0.72f, 0.4f), 12f, 1.3f);
            Block.Lamp(lights, "Lamp_Crypt", new Vector3(26f, 0.2f, -54f), new Color(0.45f, 0.55f, 0.7f), 10f, 1.1f);
            Block.Lamp(lights, "Lamp_North", new Vector3(0f, 8f, 214f), new Color(1f, 0.55f, 0.28f), 22f, 1.5f);
            Block.Lamp(lights, "Lamp_West", new Vector3(-190f, 7f, 20f), new Color(0.7f, 0.45f, 0.28f), 18f, 1.2f);

            Transform spawns = new GameObject("M02_Spawns").transform;
            spawns.SetParent(yard, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-48f, 0.1f, -30f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(52f, 0.1f, -28f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-10f, 0.1f, 86f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(12f, 0.1f, 90f));

            Transform items = new GameObject("M02_Pickups").transform;
            items.SetParent(yard, false);
            Block.Pickup(items, "Pickup_Health_South", new Vector3(0f, 0.45f, -62f), kit.Health, material, HealthTint);
            Block.Pickup(items, "Pickup_Armor_Bridge", new Vector3(0f, 12.45f, 150f), kit.Armor, material, ArmorTint);
            Block.Pickup(items, "Pickup_Quad_Court", new Vector3(10f, 0.45f, -6f), kit.Quad, material, QuadTint);
            Block.Pickup(items, "Pickup_Berserk_Hall", new Vector3(0f, 0.45f, 92f), kit.Berserk, material, BerserkTint);
            Block.Pickup(items, "Pickup_Rockets_Yard", new Vector3(46f, 1.05f, 10f), kit.Rockets, material, RocketTint);
        }

        public static void Ruins(Transform yard, Material material, MapAuthor.PickupKit kit)
        {
            Color sand = new Color(0.72f, 0.58f, 0.4f);
            Color bone = new Color(0.84f, 0.78f, 0.64f);
            Color shadow = new Color(0.38f, 0.3f, 0.22f);
            Color clay = new Color(0.55f, 0.32f, 0.2f);

            Block.Slab(yard, "PB_PlazaNorth", 0f, 32f, 0f, 108f, 48f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaSouth", 0f, -26f, 0f, 108f, 40f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaEast", 30f, 1f, 0f, 48f, 18f, 0.5f, material, sand);
            Block.Slab(yard, "PB_PlazaWest", -30f, 1f, 0f, 48f, 18f, 0.5f, material, sand);
            Block.Slab(yard, "PB_Bowl", 0f, 1f, -1.8f, 16f, 20f, 0.45f, material, shadow);
            Flight(yard, "PB_BowlSouth", new Vector3(0f, -1.8f, -4f), Vector3.back, 6, 0.6f, 5f, material, sand);
            Flight(yard, "PB_BowlNorth", new Vector3(0f, -1.8f, 5f), Vector3.forward, 6, 0.6f, 5f, material, sand);

            Block.Slab(yard, "PB_TempleFloor", 0f, 68f, 3.6f, 40f, 32f, 0.5f, material, bone);
            Flight(yard, "PB_TempleStairs", new Vector3(0f, 0f, 48f), Vector3.forward, 12, 0.6f, 12f, material, bone);
            Block.Slab(yard, "PB_WestTerrace", -36f, 16f, 2.4f, 22f, 44f, 0.4f, material, sand);
            Flight(yard, "PB_WestTerraceStairs", new Vector3(-30f, 0f, -12f), Vector3.forward, 8, 0.85f, 5f, material, sand);
            Block.Slab(yard, "PB_EastTerrace", 40f, -8f, 3.3f, 18f, 22f, 0.45f, material, clay);
            Flight(yard, "PB_EastTerraceStairs", new Vector3(28f, 0f, -8f), Vector3.right, 11, 0.5f, 4f, material, sand);
            Block.Slab(yard, "PB_SkyBridge", 0f, 70f, 7.2f, 28f, 4f, 0.35f, material, shadow);
            Flight(yard, "PB_BridgeStairsL", new Vector3(-8f, 3.6f, 64f), Vector3.forward, 12, 0.45f, 3f, material, bone);
            Flight(yard, "PB_BridgeStairsR", new Vector3(8f, 3.6f, 64f), Vector3.forward, 12, 0.45f, 3f, material, bone);

            Block.Pillar(yard, "PB_GiantA", -12f, 64f, 3.6f, 12f, 1.15f, material, bone, shadow);
            Block.Pillar(yard, "PB_GiantB", 12f, 66f, 3.6f, 12f, 1.1f, material, bone, shadow);
            Block.Pillar(yard, "PB_GiantC", -12f, 78f, 3.6f, 11f, 1.05f, material, bone, shadow);
            Block.Pillar(yard, "PB_GiantD", 10f, 80f, 3.6f, 9f, 0.95f, material, bone, clay);
            Block.Pillar(yard, "PB_BrokenA", -8f, 74f, 3.6f, 4.5f, 0.8f, material, bone, shadow);
            Block.Pillar(yard, "PB_BrokenB", 14f, 70f, 3.6f, 3.2f, 0.7f, material, clay, shadow);
            Block.Pillar(yard, "PB_ColonnadeA", -30f, -16f, 0f, 6f, 0.7f, material, bone, sand);
            Block.Pillar(yard, "PB_ColonnadeB", -18f, -22f, 0f, 4.2f, 0.6f, material, bone, sand);
            Block.Pillar(yard, "PB_ColonnadeC", 24f, -24f, 0f, 7f, 0.75f, material, bone, clay);
            Block.Pillar(yard, "PB_ColonnadeD", 38f, -16f, 0f, 3f, 0.55f, material, sand, shadow);
            Block.Cylinder(yard, "PB_FallenColumn", 0.8f, 9f, new Vector3(-16f, 0.85f, -20f), Quaternion.Euler(0f, 24f, 82f), material, bone);

            Block.Arch(yard, "PB_WestArch", new Vector3(-24f, 0f, 14f), Vector3.right, 5.5f, 2.2f, material, bone);
            Block.Arch(yard, "PB_EastArch", new Vector3(32f, 0f, -14f), Vector3.right, 4.5f, 2f, material, clay);
            Block.WallZ(yard, "PB_TempleFrontL", -14f, 54f, 8f, 4.2f, 3.6f, 0.7f, material, shadow);
            Block.WallZ(yard, "PB_TempleFrontR", 14f, 54f, 8f, 4.2f, 3.6f, 0.7f, material, shadow);
            Block.WallZ(yard, "PB_TempleBack", 0f, 84f, 40f, 6f, 3.6f, 0.8f, material, shadow);
            Block.WallX(yard, "PB_TempleWest", -20f, 70f, 24f, 5f, 3.6f, 0.6f, material, shadow);
            Block.WallX(yard, "PB_TempleEast", 20f, 74f, 16f, 4f, 3.6f, 0.6f, material, clay);
            Block.WallX(yard, "PB_WestRail", -25.2f, 16f, 36f, 1.05f, 2.4f, 0.2f, material, sand);
            Block.Cover(yard, "PB_RubbleA", 16f, -18f, material, shadow);
            Block.Cover(yard, "PB_RubbleB", -8f, -30f, material, clay);
            Block.Cover(yard, "PB_RubbleC", 22f, 6f, material, shadow);
            Block.Cover(yard, "PB_RubbleD", -4f, 44f, material, bone);
            Block.Beam(yard, "PB_Lintel", 0f, 76f, 12.2f, 22f, 0.6f, material, shadow);

            Transform lights = new GameObject("M03_Lights").transform;
            lights.SetParent(yard, false);
            Block.Lamp(lights, "Lamp_Bowl", new Vector3(0f, 2f, 1f), new Color(1f, 0.78f, 0.5f), 14f, 1.2f);
            Block.Lamp(lights, "Lamp_Temple", new Vector3(0f, 8f, 70f), new Color(1f, 0.7f, 0.45f), 22f, 1.8f);
            Block.Lamp(lights, "Lamp_East", new Vector3(40f, 6f, -8f), new Color(0.95f, 0.55f, 0.3f), 14f, 1.1f);

            Transform spawns = new GameObject("M03_Spawns").transform;
            spawns.SetParent(yard, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-36f, 2.5f, 20f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(40f, 3.4f, -4f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(-8f, 3.7f, 72f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(9f, 3.7f, 74f));

            Transform items = new GameObject("M03_Pickups").transform;
            items.SetParent(yard, false);
            Block.Pickup(items, "Pickup_Health_Bowl", new Vector3(0f, -1.35f, 1f), kit.Health, material, HealthTint);
            Block.Pickup(items, "Pickup_Armor_Temple", new Vector3(0f, 4.05f, 74f), kit.Armor, material, ArmorTint);
            Block.Pickup(items, "Pickup_Quad_Bridge", new Vector3(0f, 7.65f, 70f), kit.Quad, material, QuadTint);
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-36f, 2.85f, 8f), kit.Berserk, material, BerserkTint);
            Block.Pickup(items, "Pickup_Rockets_East", new Vector3(40f, 3.75f, 0f), kit.Rockets, material, RocketTint);
        }

        public static void Void(Transform yard, Material material, MapAuthor.PickupKit kit)
        {
            Color hull = new Color(0.09f, 0.1f, 0.12f);
            Color deck = new Color(0.16f, 0.17f, 0.2f);
            Color trim = new Color(0.25f, 0.78f, 0.86f);
            Color amber = new Color(0.75f, 0.42f, 0.16f);

            Block.Slab(yard, "PB_Core", 0f, 0f, 6f, 30f, 30f, 0.5f, material, deck);
            Block.Slab(yard, "PB_NorthIsle", 0f, 40f, 6f, 18f, 16f, 0.45f, material, hull);
            Block.Slab(yard, "PB_BridgeNorth", 0f, 24f, 6f, 4.6f, 20f, 0.35f, material, trim);
            Block.Slab(yard, "PB_SouthIsle", 6f, -38f, 6f, 16f, 14f, 0.45f, material, hull);
            Block.Slab(yard, "PB_BridgeSouth", 3f, -23f, 6f, 4.6f, 18f, 0.35f, material, trim);
            Block.Slab(yard, "PB_EastLanding", 23f, 2f, 6f, 18f, 5f, 0.4f, material, deck);
            Block.Slab(yard, "PB_EastIsle", 42f, 2f, 10.2f, 16f, 18f, 0.45f, material, hull);
            Flight(yard, "PB_EastStairs", new Vector3(30f, 6f, 2f), Vector3.right, 14, 0.45f, 4f, material, deck);
            Block.Slab(yard, "PB_WestIsle", -40f, -2f, 3f, 16f, 16f, 0.45f, material, hull);
            Block.Slab(yard, "PB_WestLanding", -31f, -2f, 3f, 10f, 4.6f, 0.4f, material, deck);
            Block.Slab(yard, "PB_WestHigh", -20f, -2f, 6f, 14f, 4.6f, 0.4f, material, trim);
            Flight(yard, "PB_WestStairs", new Vector3(-30f, 3f, -2f), Vector3.right, 10, 0.5f, 3.6f, material, deck);
            Block.Slab(yard, "PB_Satellite", 28f, 34f, 10.2f, 12f, 12f, 0.4f, material, hull);
            Block.Slab(yard, "PB_SatelliteBridge", 15f, 36f, 6f, 14f, 4.2f, 0.35f, material, trim);
            Flight(yard, "PB_SatelliteStairs", new Vector3(20f, 6f, 36f), Vector3.right, 14, 0.4f, 3.5f, material, deck);
            Block.Slab(yard, "PB_Crown", 0f, 42f, 10.5f, 8f, 8f, 0.4f, material, trim);
            Flight(yard, "PB_CrownStairs", new Vector3(-2f, 6f, 36f), Vector3.forward, 15, 0.4f, 3f, material, deck);
            Block.WallZ(yard, "PB_CrownBlind", 0f, 39.2f, 7f, 2.6f, 10.5f, 0.35f, material, trim);
            Block.Slab(yard, "PB_WestTongue", -40f, 8f, 3f, 8f, 6f, 0.35f, material, hull);
            Block.Slab(yard, "PB_LowSpur", -40f, 13f, 1.5f, 12f, 12f, 0.4f, material, amber);
            Flight(yard, "PB_SpurStairs", new Vector3(-40f, 1.5f, 9f), Vector3.back, 5, 0.5f, 3f, material, deck);

            Block.WallX(yard, "PB_RailNorthA", -2.15f, 24f, 18f, 1f, 6f, 0.12f, material, trim);
            Block.WallX(yard, "PB_RailNorthB", 2.15f, 24f, 18f, 1f, 6f, 0.12f, material, trim);
            Block.WallX(yard, "PB_RailSouthA", 0.8f, -23f, 14f, 1f, 6f, 0.12f, material, trim);
            Block.WallX(yard, "PB_RailSouthB", 5.2f, -23f, 14f, 1f, 6f, 0.12f, material, trim);
            Block.WallX(yard, "PB_EastScreen", 38f, 4f, 6f, 2.4f, 10.2f, 0.35f, material, trim);

            Block.ColumnOn(yard, "PB_MastA", 10f, 10f, 5f, 6f, material, trim);
            Block.ColumnOn(yard, "PB_MastB", -10f, 10f, 5f, 6f, material, trim);
            Block.ColumnOn(yard, "PB_MastC", 10f, -10f, 4.5f, 6f, material, amber);
            Block.ColumnOn(yard, "PB_MastD", -10f, -8f, 4.5f, 6f, material, trim);
            Block.Beam(yard, "PB_TrussA", -6f, 0f, 11f, 0.35f, 22f, material, trim);
            Block.Beam(yard, "PB_TrussB", 6f, 0f, 11f, 0.35f, 22f, material, trim);
            Block.Beam(yard, "PB_TrussCross", 0f, 0f, 11f, 14f, 0.35f, material, amber);
            Block.CoverOn(yard, "PB_CoreCoverA", -7f, 6f, 6f, material, hull);
            Block.CoverOn(yard, "PB_CoreCoverB", 8f, -7f, 6f, material, hull);
            Block.Pillar(yard, "PB_NorthMast", 0f, 44f, 6f, 6f, 0.4f, material, trim, hull);

            Transform lights = new GameObject("M04_Lights").transform;
            lights.SetParent(yard, false);
            Block.Lamp(lights, "Lamp_Core", new Vector3(0f, 9f, 0f), new Color(0.4f, 0.8f, 0.95f), 18f, 1.8f);
            Block.Lamp(lights, "Lamp_East", new Vector3(42f, 13f, 2f), new Color(1f, 0.55f, 0.25f), 14f, 1.5f);
            Block.Lamp(lights, "Lamp_North", new Vector3(0f, 9f, 40f), new Color(0.35f, 0.7f, 0.9f), 14f, 1.3f);
            Block.Lamp(lights, "Lamp_West", new Vector3(-40f, 6f, -2f), new Color(0.3f, 0.65f, 0.8f), 12f, 1.1f);

            Transform spawns = new GameObject("M04_Spawns").transform;
            spawns.SetParent(yard, false);
            Block.Marker(spawns, "Spawn_Human_A", new Vector3(-5f, 6.1f, -6f));
            Block.Marker(spawns, "Spawn_Human_B", new Vector3(6f, 6.1f, 5f));
            Block.Marker(spawns, "Spawn_Monster_A", new Vector3(0f, 10.6f, 42f));
            Block.Marker(spawns, "Spawn_Monster_B", new Vector3(42f, 10.3f, 4f));

            Transform items = new GameObject("M04_Pickups").transform;
            items.SetParent(yard, false);
            Block.Pickup(items, "Pickup_Health_Core", new Vector3(-7f, 6.5f, 2f), kit.Health, material, HealthTint);
            Block.Pickup(items, "Pickup_Armor_High", new Vector3(0f, 11f, 42f), kit.Armor, material, ArmorTint);
            Block.Pickup(items, "Pickup_Quad_East", new Vector3(44f, 10.7f, 2f), kit.Quad, material, QuadTint);
            Block.Pickup(items, "Pickup_Berserk_West", new Vector3(-40f, 3.5f, -2f), kit.Berserk, material, BerserkTint);
            Block.Pickup(items, "Pickup_Rockets_South", new Vector3(6f, 6.5f, -38f), kit.Rockets, material, RocketTint);
        }

        static void GiantTower(Transform parent, string name, Vector3 origin, bool doorFacesSouth, bool openEast, bool openWest, Material material, Color stone, Color dark)
        {
            float x = origin.x;
            float z = origin.z;
            const float edge = 12f;
            const float wallHeight = 16f;
            SideWall(parent, name + "_W", true, x - edge, z, openWest, wallHeight, material, stone);
            SideWall(parent, name + "_E", true, x + edge, z, openEast, wallHeight, material, stone);
            SideWall(parent, name + "_N", false, x, z + edge, doorFacesSouth == false, wallHeight, material, stone);
            SideWall(parent, name + "_S", false, x, z - edge, doorFacesSouth, wallHeight, material, stone);
            Block.Pillar(parent, name + "_C1", x - 10.4f, z - 10.4f, 0f, 22f, 1.35f, material, dark, stone);
            Block.Pillar(parent, name + "_C2", x + 10.4f, z - 10.4f, 0f, 22f, 1.35f, material, dark, stone);
            Block.Pillar(parent, name + "_C3", x - 10.4f, z + 10.4f, 0f, 22f, 1.35f, material, dark, stone);
            Block.Pillar(parent, name + "_C4", x + 10.4f, z + 10.4f, 0f, 22f, 1.35f, material, dark, stone);
            Block.Slab(parent, name + "_Mid", x, z, 6f, 23f, 23f, 0.4f, material, dark);
            Block.Slab(parent, name + "_Top", x, z, 12f, 23f, 23f, 0.4f, material, dark);
            Block.Slab(parent, name + "_Crown", x, z, 16f, 26f, 26f, 0.45f, material, stone);
            Flight(parent, name + "_StairsA", new Vector3(x - 5f, 0f, z - 7f), Vector3.forward, 20, 0.42f, 3.2f, material, stone);
            Flight(parent, name + "_StairsB", new Vector3(x + 5f, 6f, z + 6f), Vector3.back, 20, 0.42f, 3.2f, material, stone);
            float buttressZ = doorFacesSouth ? edge + 1.2f : -(edge + 1.2f);
            Vector3 outward = doorFacesSouth ? Vector3.forward : Vector3.back;
            Block.Buttress(parent, name + "_Buttress", origin + new Vector3(0f, 0f, buttressZ), outward, 12f, material, stone);
        }

        static void WatchTower(Transform parent, string name, Vector3 origin, bool doorFacesSouth, Material material, Color stone, Color dark)
        {
            float x = origin.x;
            float z = origin.z;
            const float edge = 8f;
            const float height = 9f;
            Block.WallX(parent, name + "_W", x - edge, z, 14f, height, 0f, 1.2f, material, stone);
            Block.WallX(parent, name + "_E", x + edge, z, 14f, height, 0f, 1.2f, material, stone);
            if (doorFacesSouth)
            {
                Block.WallZ(parent, name + "_NL", x - 5f, z + edge, 4f, height, 0f, 1.2f, material, stone);
                Block.WallZ(parent, name + "_NR", x + 5f, z + edge, 4f, height, 0f, 1.2f, material, stone);
                Block.WallZ(parent, name + "_S", x, z - edge, 14f, height, 0f, 1.2f, material, stone);
                Block.Arch(parent, name + "_Arch", new Vector3(x, 0f, z - 12f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            }
            else
            {
                Block.WallZ(parent, name + "_SL", x - 5f, z - edge, 4f, height, 0f, 1.2f, material, stone);
                Block.WallZ(parent, name + "_SR", x + 5f, z - edge, 4f, height, 0f, 1.2f, material, stone);
                Block.WallZ(parent, name + "_N", x, z + edge, 14f, height, 0f, 1.2f, material, stone);
                Block.Arch(parent, name + "_Arch", new Vector3(x, 0f, z + 12f), Vector3.forward, 7f, 2f, material, stone, 5.5f);
            }

            Block.Pillar(parent, name + "_P1", x - 6.4f, z - 6.4f, 0f, 11f, 0.7f, material, dark, stone);
            Block.Pillar(parent, name + "_P2", x + 6.4f, z + 6.4f, 0f, 11f, 0.7f, material, dark, stone);
            Block.Slab(parent, name + "_Deck", x, z, 3.6f, 14f, 14f, 0.35f, material, dark);
            Block.Slab(parent, name + "_Crown", x, z, 7.5f, 16f, 16f, 0.4f, material, stone);
            Flight(parent, name + "_Stairs", new Vector3(x - 2f, 0f, z - 4f), Vector3.forward, 12, 0.4f, 2.6f, material, stone);
        }

        static void RuinTower(Transform parent, string name, Vector3 origin, Material material, Color stone, Color moss)
        {
            float x = origin.x;
            float z = origin.z;
            Block.WallX(parent, name + "_W", x - 6f, z, 12f, 6.5f, 0f, 1.1f, material, stone);
            Block.WallZ(parent, name + "_N", x + 1f, z + 6f, 8f, 4.2f, 0f, 1.1f, material, moss);
            Block.WallX(parent, name + "_E", x + 6f, z - 2f, 7f, 3.1f, 0f, 1f, material, stone);
            Block.Pillar(parent, name + "_Tall", x - 4.5f, z - 4.5f, 0f, 8f, 0.65f, material, stone, moss);
            Block.Pillar(parent, name + "_Stub", x + 4.2f, z + 3.5f, 0f, 3.4f, 0.55f, material, moss, stone);
            Block.Slab(parent, name + "_StubFloor", x - 2f, z, 2.4f, 8f, 5f, 0.3f, material, DarkStone(stone));
            Block.Cylinder(parent, name + "_Fallen", 0.45f, 6f, new Vector3(x + 2f, 0.5f, z - 3f), Quaternion.Euler(0f, 25f, 82f), material, stone);
            Block.Cover(parent, name + "_Rubble", x + 1.5f, z + 1.2f, material, moss);
            Block.Arch(parent, name + "_Arch", new Vector3(x, 0f, z - 10f), Vector3.forward, 6f, 1.8f, material, stone, 4.2f);
        }

        static Color DarkStone(Color stone)
        {
            return Color.Lerp(stone, Color.black, 0.45f);
        }

        static void SideWall(Transform parent, string name, bool alongX, float x, float z, bool open, float height, Material material, Color color)
        {
            const float length = 22f;
            const float thickness = 1.6f;
            if (open == false)
            {
                if (alongX)
                {
                    Block.WallX(parent, name, x, z, length, height, 0f, thickness, material, color);
                }
                else
                {
                    Block.WallZ(parent, name, x, z, length, height, 0f, thickness, material, color);
                }

                return;
            }

            if (alongX)
            {
                Block.WallX(parent, name + "Low", x, z, length, 10.6f, 0f, thickness, material, color);
                Block.WallX(parent, name + "MidL", x, z - 7.5f, 7f, 3.8f, 10.6f, thickness, material, color);
                Block.WallX(parent, name + "MidR", x, z + 7.5f, 7f, 3.8f, 10.6f, thickness, material, color);
                Block.WallX(parent, name + "High", x, z, length, 1.6f, 14.4f, thickness, material, color);
                return;
            }

            Block.WallZ(parent, name + "L", x - 7.5f, z, 7f, height, 0f, thickness, material, color);
            Block.WallZ(parent, name + "R", x + 7.5f, z, 7f, height, 0f, thickness, material, color);
        }

        static void Flight(Transform parent, string name, Vector3 start, Vector3 direction, int steps, float run, float width, Material material, Color color)
        {
            Block.Stairs(parent, name, start, direction, steps, 0.3f, run, width, material, color);
        }
    }
}
