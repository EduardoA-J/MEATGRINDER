using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class LocalSpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] points;
        [SerializeField] private float voidHeight = -15f;
        public float VoidHeight => voidHeight;
        public Transform[] Points => points;

        internal void SetPoints(Transform[] value) => points = value;

        public Transform Choose(Vector3 deathPosition, Transform previous, Transform body, float height, float radius)
        {
            Transform best = null;
            float bestDistance = -1f;
            if (points == null) return null;
            foreach (Transform point in points)
            {
                if (point == null || point == previous) continue;
                float distance = (point.position - deathPosition).sqrMagnitude;
                if (distance < 1f || distance <= bestDistance) continue;
                if (!Physics.Raycast(point.position + Vector3.up * 0.1f, Vector3.down, 0.6f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) continue;
                bool occupied = false;
                foreach (var hit in Physics.OverlapCapsule(point.position + Vector3.up * radius,
                    point.position + Vector3.up * (height - radius), radius * 0.9f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (!hit.transform.IsChildOf(body)) { occupied = true; break; }
                }
                if (occupied) continue;
                best = point;
                bestDistance = distance;
            }
            return best;
        }
    }
}
