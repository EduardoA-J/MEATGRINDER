using Meatgrinder.Combat;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    public sealed class DeathLoot : MonoBehaviour
    {
        WeaponInventoryEntry[] inventory;
        float remaining;
        bool collected;

        public static DeathLoot Drop(Vector3 position, Vector3 velocity, WeaponInventoryEntry[] contents, float lifetime, GameObject prefab = null)
        {
            if (contents == null || contents.Length == 0) return null;
            GameObject bag = prefab != null ? Instantiate(prefab) : GameObject.CreatePrimitive(PrimitiveType.Cube);
            bag.name = "Dropped weapons and ammo";
            bag.transform.position = position + Vector3.up * 0.5f;
            if (prefab == null) bag.transform.localScale = new Vector3(0.65f, 0.4f, 0.45f);
            var rigidbody = bag.GetComponent<Rigidbody>() ?? bag.AddComponent<Rigidbody>();
            rigidbody.linearVelocity = velocity;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            var trigger = bag.GetComponent<SphereCollider>() ?? bag.AddComponent<SphereCollider>();
            trigger.radius = prefab != null ? 1f : 2f;
            trigger.isTrigger = true;
            var loot = bag.GetComponent<DeathLoot>() ?? bag.AddComponent<DeathLoot>();
            loot.inventory = contents;
            loot.remaining = lifetime;
            return loot;
        }

        void Update()
        {
            remaining -= Time.deltaTime;
            if (remaining <= 0f) Destroy(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            var health = other.GetComponentInParent<HealthComponent>();
            if (collected || health == null || !health.IsAlive || !health.AllowWeapons) return;
            var weapons = health.GetComponent<WeaponController>();
            if (weapons == null || !weapons.ReceiveInventory(inventory)) return;
            collected = true;
            Destroy(gameObject);
        }
    }
}
