using UnityEngine;

namespace Meatgrinder.Combat
{
    public sealed class DamageResolverHost : MonoBehaviour
    {
        public DamageResolver Resolver { get; } = new DamageResolver();
    }
}
