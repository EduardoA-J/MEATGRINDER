using UnityEngine;

namespace Meatgrinder.Weapons
{
    public interface IProjectileSpawner
    {
        RocketProjectile Spawn(ProjectileLaunch launch);
    }
}
