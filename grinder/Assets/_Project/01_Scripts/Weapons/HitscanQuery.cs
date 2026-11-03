using System.Collections.Generic;
using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Weapons
{
    public static class HitscanQuery
    {
        public static void Collect(
            WeaponDefinition weapon,
            Vector3 origin,
            Vector3 direction,
            int seed,
            Transform ignoreRoot,
            List<HitscanHit> results)
        {
            results.Clear();
            Physics.SyncTransforms();
            if (weapon == null)
            {
                return;
            }

            Vector3 aim = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.forward;
            for (int pellet = 0; pellet < weapon.PelletCount; pellet++)
            {
                Vector3 pelletDirection = HitscanSpread.Direction(aim, pellet, weapon.SpreadDegrees, seed);
                if (Physics.Raycast(
                        origin,
                        pelletDirection,
                        out RaycastHit hit,
                        weapon.MaxRange,
                        Physics.DefaultRaycastLayers,
                        QueryTriggerInteraction.Ignore) == false)
                {
                    continue;
                }

                if (ignoreRoot != null && hit.collider.transform.IsChildOf(ignoreRoot))
                {
                    continue;
                }

                IDamageable target = hit.collider.GetComponentInParent<HealthComponent>();
                results.Add(new HitscanHit(hit.point, hit.distance, weapon.DamageAt(hit.distance), target, pellet));
            }
        }
    }
}
