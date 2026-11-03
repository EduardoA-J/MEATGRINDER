using UnityEngine;

namespace Meatgrinder.Weapons
{
    public static class HitscanSpread
    {
        public static Vector3 Direction(Vector3 aim, int pellet, float spreadDegrees, int seed)
        {
            Vector3 forward = aim.sqrMagnitude > 0.0001f ? aim.normalized : Vector3.forward;
            if (pellet <= 0 || spreadDegrees <= 0f)
            {
                return forward;
            }

            uint hash = Mix((uint)seed, (uint)pellet);
            float yaw = Unit(hash) * spreadDegrees;
            float pitch = Unit(hash >> 8) * spreadDegrees;
            Quaternion basis = Quaternion.LookRotation(forward, Vector3.up);
            return (basis * Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward).normalized;
        }

        static float Unit(uint value)
        {
            return ((value & 255) / 255f) * 2f - 1f;
        }

        static uint Mix(uint seed, uint pellet)
        {
            uint value = (seed * 747796405u) + (pellet * 2891336453u) + 277803737u;
            value ^= value >> 16;
            return value;
        }
    }
}
