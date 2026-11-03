using UnityEngine;

namespace Meatgrinder.Weapons
{
    // Instantiate is local. Host migration replaces this body with Runner.Spawn of a NetworkObject
    // and runs RocketProjectile.Step from FixedUpdateNetwork. This object is not replicated.
    public sealed class LocalProjectileSpawner : MonoBehaviour, IProjectileSpawner
    {
        [SerializeField] private GameObject fallbackPrefab;

        public RocketProjectile Spawn(ProjectileLaunch launch)
        {
            if (launch == null || launch.Weapon == null)
            {
                return null;
            }

            GameObject prefab = launch.Weapon.ProjectilePrefab != null
                ? launch.Weapon.ProjectilePrefab
                : fallbackPrefab;
            Quaternion rotation = Quaternion.LookRotation(
                launch.Direction.sqrMagnitude > 0.0001f ? launch.Direction.normalized : Vector3.forward);
            GameObject body = prefab != null
                ? Instantiate(prefab, launch.Origin, rotation)
                : CreateFallback(launch, rotation);
            body.name = "Rocket";
            RocketProjectile projectile = body.GetComponent<RocketProjectile>();
            if (projectile == null)
            {
                projectile = body.AddComponent<RocketProjectile>();
            }

            projectile.Launch(launch);
            return projectile;
        }

        static GameObject CreateFallback(ProjectileLaunch launch, Quaternion rotation)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.transform.SetPositionAndRotation(launch.Origin, rotation);
            body.transform.localScale = Vector3.one * (launch.Weapon.ProjectileRadius * 2f);
            return body;
        }
    }
}
