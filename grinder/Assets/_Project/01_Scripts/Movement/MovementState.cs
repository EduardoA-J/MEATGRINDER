using UnityEngine;

namespace Meatgrinder.Movement
{
    public struct MovementState
    {
        public Vector3 Velocity;
        public bool IsGrounded;
        public float LandingAge;
        public float GroundLockRemaining;

        public static MovementState Standing()
        {
            return new MovementState
            {
                LandingAge = 1000f
            };
        }
    }
}
