using UnityEngine;

namespace Meatgrinder.Players
{
    [CreateAssetMenu(fileName = "LifeTuning", menuName = "MEATGRINDER/Life Tuning")]
    public sealed class LifeTuning : ScriptableObject
    {
        [SerializeField, Min(0f)] private float respawnDelay = 4f;
        [SerializeField, Min(0f)] private float spawnProtection = 1f;
        [SerializeField, Min(0f)] private float lootLifetime = 90f;
        [SerializeField, Min(0f)] private float voidGrace = 1f;
        [SerializeField, Min(0.01f)] private float voidHealthFractionPerSecond = 0.25f;
        [SerializeField, Range(0.1f, 5f)] private float maximumVoidDuration = 5f;
        [SerializeField] private GameObject lootPrefab;
        public float RespawnDelay => respawnDelay;
        public float SpawnProtection => spawnProtection;
        public float LootLifetime => lootLifetime;
        public float VoidGrace => voidGrace;
        public float VoidHealthFractionPerSecond => voidHealthFractionPerSecond;
        public float MaximumVoidDuration => Mathf.Clamp(maximumVoidDuration, 0.1f, 5f);
        public GameObject LootPrefab => lootPrefab;
    }
}
