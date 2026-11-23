using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Pickups;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Meatgrinder.Tests
{
    public sealed class ProductAssetsEditModeTests
    {
        [Test]
        public void ProductPrefabHasTuningIconsAmmoAndProjectileAssets()
        {
            const string root = "Assets/_Project/";
            var player = AssetDatabase.LoadAssetAtPath<GameObject>(root + "03_Prefabs/Players/LocalPlayer.prefab");
            Assert.NotNull(player);
            foreach (var component in new Component[] { player.GetComponent<PlayerLife>(), player.GetComponent<PlayerMelee>(), player.GetComponent<PlayerVoidFall>() })
            {
                Assert.NotNull(component);
                Assert.NotNull(new SerializedObject(component).FindProperty("tuning").objectReferenceValue, component.GetType().Name);
            }
            var theme = AssetDatabase.LoadAssetAtPath<PresentationTheme>(root + "02_ScriptableObjects/Presentation/ProductTheme.asset");
            for (int i = 0; i < 6; i++) Assert.NotNull(theme.Icon((PowerUpType)i), "Power icon " + i);
            var rocket = AssetDatabase.LoadAssetAtPath<WeaponDefinition>(root + "02_ScriptableObjects/Weapons/RocketLauncher.asset");
            Assert.NotNull(rocket.ProjectilePrefab); Assert.NotNull(rocket.ShotSound);
            Assert.NotNull(player.GetComponentInChildren<WeaponVisualRig>(true));
            Assert.NotNull(new SerializedObject(player.GetComponent<PlayerCombat>()).FindProperty("projectileMuzzle").objectReferenceValue);
        }
    }
}
