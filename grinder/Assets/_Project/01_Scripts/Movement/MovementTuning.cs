using UnityEngine;

namespace Meatgrinder.Movement
{
    [CreateAssetMenu(fileName = "MovementTuning", menuName = "MEATGRINDER/Movement Tuning")]
    public sealed class MovementTuning : ScriptableObject
    {
        [Header("Suelo")]
        [SerializeField] private float groundSpeed = 8f; // PENDIENTE
        [SerializeField] private float groundAcceleration = 10f; // PENDIENTE
        [SerializeField] private float friction = 6f; // PENDIENTE
        [SerializeField] private float stopSpeed = 2f; // PENDIENTE

        [Header("Aire")]
        [SerializeField] private float airAcceleration = 70f; // PENDIENTE
        [SerializeField] private float airWishSpeed = 2.8f; // PENDIENTE. Recorta el wish, no la velocidad ya ganada.
        [SerializeField] private float airControl; // PENDIENTE. El slerp solo redirige. Un valor alto cierra el ángulo del strafe.
        [SerializeField] private float maxSpeed = 32f; // PENDIENTE. Tope de seguridad, no es la carrera.

        [Header("Salto")]
        [SerializeField] private float jumpVelocity = 7.5f; // PENDIENTE
        [SerializeField] private float gravity = 22f; // PENDIENTE
        [SerializeField] private float bunnyHopWindow = 0.12f; // PENDIENTE
        [SerializeField] private float bunnyHopSpeedRetention = 1f; // PENDIENTE
        [SerializeField] private float jumpGroundLock = 0.1f; // PENDIENTE
        [SerializeField] private float groundStick = 3f; // PENDIENTE

        [Header("Cuerpo")]
        [SerializeField] private float height = 1.8f; // PENDIENTE
        [SerializeField] private float radius = 0.4f; // PENDIENTE
        [SerializeField] private float stepOffset = 0.4f; // PENDIENTE
        [SerializeField] private float slopeLimit = 46f; // PENDIENTE
        [SerializeField] private float skinWidth = 0.08f; // PENDIENTE
        [SerializeField] private float groundProbeDistance = 0.2f; // PENDIENTE

        [Header("Mirada")]
        [SerializeField] private float lookSensitivity = 0.08f; // PENDIENTE
        [SerializeField] private float minPitch = -89f; // PENDIENTE
        [SerializeField] private float maxPitch = 89f; // PENDIENTE
        [SerializeField] private float fieldOfView = 100f; // PENDIENTE

        [Header("Sin usar")]
        [SerializeField] private float dashCooldown; // PENDIENTE. D-06. El controlador no lo lee.

        public float GroundSpeed => groundSpeed;
        public float GroundAcceleration => groundAcceleration;
        public float Friction => friction;
        public float StopSpeed => stopSpeed;
        public float AirAcceleration => airAcceleration;
        public float AirWishSpeed => airWishSpeed > 0f ? airWishSpeed : groundSpeed;
        public float AirControl => airControl;
        public float MaxSpeed => maxSpeed;
        public float JumpVelocity => jumpVelocity;
        public float Gravity => gravity;
        public float BunnyHopWindow => bunnyHopWindow;
        public float BunnyHopSpeedRetention => bunnyHopSpeedRetention;
        public float JumpGroundLock => jumpGroundLock;
        public float GroundStick => groundStick;
        public float Height => height;
        public float Radius => radius;
        public float StepOffset => stepOffset;
        public float SlopeLimit => slopeLimit;
        public float SkinWidth => skinWidth;
        public float GroundProbeDistance => groundProbeDistance;
        public float LookSensitivity => lookSensitivity;
        public float MinPitch => minPitch;
        public float MaxPitch => maxPitch;
        public float FieldOfView => fieldOfView;

        public float DashCooldown => dashCooldown;

        internal void SetGroundAcceleration(float value)
        {
            groundAcceleration = value;
        }

        internal void SetFriction(float value)
        {
            friction = value;
        }

        internal void SetAirAcceleration(float value)
        {
            airAcceleration = value;
        }

        internal void SetAirWishSpeed(float value)
        {
            airWishSpeed = value;
        }

        internal void SetAirControl(float value)
        {
            airControl = value;
        }

        internal void SetMaxSpeed(float value)
        {
            maxSpeed = value;
        }

        internal void SetBunnyHopSpeedRetention(float value)
        {
            bunnyHopSpeedRetention = value;
        }

        internal void SetGravity(float value)
        {
            gravity = value;
        }
    }
}
