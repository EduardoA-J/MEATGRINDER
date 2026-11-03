using System.Collections.Generic;
using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Weapons
{
    public static class ExplosionQuery
    {
        public static int Apply(ProjectileLaunch launch)
        {
            if (launch == null || launch.Weapon == null || launch.Resolver == null)
            {
                return 0;
            }

            Physics.SyncTransforms();
            Collider[] found = Physics.OverlapSphere(
                launch.Origin,
                launch.Weapon.BlastRadius,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            HashSet<int> seen = new HashSet<int>();
            int applied = 0;
            int slot = 0;
            for (int i = 0; i < found.Length; i++)
            {
                HealthComponent health = found[i].GetComponentInParent<HealthComponent>();
                if (health == null || seen.Add(health.StableId) == false)
                {
                    continue;
                }

                Vector3 point = health.transform.position + (Vector3.up * 0.5f);
                Vector3 delta = point - launch.Origin;
                float distance = delta.magnitude;
                if (distance > launch.Weapon.BlastRadius)
                {
                    continue;
                }

                if (distance > 0.05f && Blocked(launch.Origin, delta, distance, health, launch.AttackerId))
                {
                    continue;
                }

                bool hit = launch.Resolver.Apply(new DamageInfo(
                    launch.AttackerId,
                    launch.AttackerTeam,
                    health,
                    launch.Weapon.BlastAmount(distance),
                    DamageType.Explosion,
                    launch.Origin,
                    launch.ConfirmationBase + (ulong)slot,
                    launch.HasAuthority,
                    true,
                    launch.Weapon.WeaponId));
                slot++;
                if (hit)
                {
                    applied++;
                }
            }

            return applied;
        }

        static bool Blocked(Vector3 origin, Vector3 delta, float distance, HealthComponent health, int attackerId)
        {
            RaycastHit[] hits = Physics.RaycastAll(
                origin,
                delta.normalized,
                Mathf.Max(0.01f, distance - 0.05f),
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                HealthComponent hitHealth = hits[i].collider.GetComponentInParent<HealthComponent>();
                if (hitHealth == health)
                {
                    return false;
                }

                if (hitHealth != null && hitHealth.StableId == attackerId)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
