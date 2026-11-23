using UnityEngine;

namespace Meatgrinder.Weapons
{
    public sealed class RocketProjectile : MonoBehaviour
    {
        ProjectileLaunch launch;
        float age;
        bool active;
        bool exploded;
        public event System.Action<Vector3> Exploded;

        public void Launch(ProjectileLaunch shot)
        {
            launch = shot;
            age = 0f;
            exploded = false;
            active = shot != null && shot.Weapon != null;
            transform.position = shot != null ? shot.Origin : transform.position;
            Collider body = GetComponent<Collider>();
            if (body != null)
            {
                body.enabled = false;
            }
        }

        void FixedUpdate()
        {
            Step(Time.fixedDeltaTime);
        }

        internal void Step(float deltaTime)
        {
            if (active == false || exploded || launch == null || launch.Weapon == null || deltaTime <= 0f)
            {
                return;
            }

            age += deltaTime;
            if (age >= launch.Weapon.ProjectileLifetime)
            {
                Retire();
                return;
            }

            Physics.SyncTransforms();
            Vector3 direction = launch.Direction.sqrMagnitude > 0.0001f
                ? launch.Direction.normalized
                : Vector3.forward;
            float distance = launch.Weapon.ProjectileSpeed * deltaTime;
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, launch.Weapon.ProjectileRadius,
                direction, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                if (IsOwner(hit.collider)) continue;
                transform.position = hit.point;
                launch.Origin = hit.point + hit.normal * 0.05f;
                Explode();
                return;
            }

            transform.position += direction * distance;
        }

        void Explode()
        {
            if (exploded)
            {
                return;
            }

            exploded = true;
            int damaged = ExplosionQuery.Apply(launch);
            Exploded?.Invoke(launch.Origin);
            if (launch.Presenter != null)
            {
                launch.Presenter.Present(launch.Origin, damaged > 0);
            }

            Retire();
        }

        bool IsOwner(Collider collider)
        {
            return launch.Owner != null && collider.transform.IsChildOf(launch.Owner);
        }

        void Retire()
        {
            active = false;
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
    }
}
