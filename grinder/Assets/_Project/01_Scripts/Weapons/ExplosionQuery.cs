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

                launch.Resolver.AttackerPowerUps = launch.Owner != null
                    ? launch.Owner.GetComponent<IDamageModifiers>()
                    : launch.Resolver.AttackerPowerUps;
                launch.Resolver.TargetPowerUps = health.GetComponent<IDamageModifiers>();
                bool hit = launch.Resolver.Apply(new DamageInfo(
                    launch.AttackerId,
                    launch.AttackerTeam,
                    health,
                    launch.Weapon.BlastAmount(distance) * (health.StableId == launch.AttackerId ? launch.Weapon.SelfDamageScale : 1f),
                    DamageType.Explosion,
                    launch.Origin,
                    launch.ConfirmationBase,
                    launch.HasAuthority,
                    !launch.AllowSelfDamage,
                    launch.Weapon.WeaponId));
                if (launch.HasAuthority && launch.AllowSelfDamage && health.StableId == launch.AttackerId && health.IsAlive)
                {
                    float impulse = launch.Weapon.BlastImpulse * Mathf.Clamp01(1f - distance / launch.Weapon.BlastRadius);
                    Vector3 radial = new Vector3(delta.x, 0f, delta.z);
                    Vector3 direction = (Vector3.up + (radial.sqrMagnitude > 0.0001f ? radial.normalized * 0.35f : Vector3.zero)).normalized;
                    health.GetComponent<IImpulseReceiver>()?.AddImpulse(direction * impulse);
                }
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
