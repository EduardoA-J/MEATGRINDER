using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Weapons
{
    public readonly struct HitscanHit
    {
        public HitscanHit(Vector3 point, float distance, float damage, IDamageable target, int pellet)
        {
            Point = point;
            Distance = distance;
            Damage = damage;
            Target = target;
            Pellet = pellet;
        }

        public Vector3 Point { get; }
        public float Distance { get; }
        public float Damage { get; }
        public IDamageable Target { get; }
        public int Pellet { get; }
    }
}
