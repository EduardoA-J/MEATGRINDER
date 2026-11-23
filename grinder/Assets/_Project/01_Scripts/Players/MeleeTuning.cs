using UnityEngine;

namespace Meatgrinder.Players
{
    [CreateAssetMenu(fileName = "MeleeTuning", menuName = "MEATGRINDER/Melee Tuning")]
    public sealed class MeleeTuning : ScriptableObject
    {
        [SerializeField, Min(0f)] private float damage = 20f;
        [SerializeField, Min(0f)] private float range = 2.2f;
        [SerializeField, Min(0f)] private float radius = 0.45f;
        [SerializeField, Min(0f)] private float cooldown = 0.55f;
        [SerializeField, Min(0f)] private float windup = 0.12f; // PENDIENTE: animation timing for playtesting.
        [SerializeField, Min(0f)] private float impactWindow = 0.1f;
        public float Damage => damage;
        public float Range => range;
        public float Radius => radius;
        public float Cooldown => cooldown;
        public float Windup => Mathf.Min(windup, cooldown);
        public float ImpactWindow => Mathf.Min(impactWindow, Mathf.Max(0f, cooldown - Windup));
    }
}
