using UnityEngine;

namespace Meatgrinder.Movement
{
    // Local body. FixedUpdate keeps the same result when the render rate changes.
    // Fusion later: move this call into FixedUpdateNetwork and feed a predicted command.
    // Sync Velocity, pose and the grounded bit. Presentation (camera) stays on the client.
    // DashCooldown is intentionally unread until D-06.
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private MovementTuning tuning;
        [SerializeField] private MonoBehaviour planarInputSource;

        CharacterController controller;
        IPlanarInput scriptedInput;
        MovementState state;
        bool collisionGrounded;
        bool jumpWasHeld;
        float jumpBuffer;

        public Vector3 Velocity => state.Velocity;
        public float HorizontalSpeed => MovementSimulator.HorizontalSpeed(state.Velocity);
        public bool IsGrounded => state.IsGrounded;
        public bool JumpBuffered => jumpBuffer > 0f;
        public MovementTuning Tuning => tuning;

        // A press fills the window once. Holding the key does not fill it again.
        internal static void SampleJump(
            ref bool wasHeld,
            ref float buffer,
            bool held,
            float window,
            float deltaTime,
            out bool jump)
        {
            if (held && wasHeld == false)
            {
                buffer = window > 0f ? window : 0f;
            }

            wasHeld = held;
            jump = buffer > 0f;
            if (buffer > 0f)
            {
                buffer = Mathf.Max(0f, buffer - Mathf.Max(0f, deltaTime));
            }
        }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            state = MovementState.Standing();
            ResolveInput();
            ApplyBody();
        }

        void FixedUpdate()
        {
            if (tuning == null || controller == null)
            {
                return;
            }

            ApplyBody();
            float deltaTime = Time.fixedDeltaTime;
            IPlanarInput input = scriptedInput ?? (planarInputSource as IPlanarInput);
            MovementIntent intent = default;
            if (input != null)
            {
                intent.Move = Vector2.ClampMagnitude(input.Move, 1f);
                SampleJump(
                    ref jumpWasHeld,
                    ref jumpBuffer,
                    input.JumpHeld,
                    tuning.BunnyHopWindow,
                    deltaTime,
                    out bool jump);
                intent.Jump = jump;
            }
            MovementSimulator.Tick(
                ref state,
                intent,
                tuning,
                transform.forward,
                transform.right,
                deltaTime,
                collisionGrounded);

            CollisionFlags flags = controller.Move(state.Velocity * deltaTime);
            if ((flags & CollisionFlags.Above) != 0 && state.Velocity.y > 0f)
            {
                state.Velocity.y = 0f;
            }

            collisionGrounded = (flags & CollisionFlags.Below) != 0 || ProbeGround();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (tuning == null || controller == null || hit.normal.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < tuning.SlopeLimit)
            {
                return;
            }

            float stepTop = transform.position.y + controller.stepOffset;
            if (hit.point.y <= stepTop)
            {
                return;
            }

            Vector3 horizontal = Vector3.ProjectOnPlane(
                new Vector3(state.Velocity.x, 0f, state.Velocity.z),
                hit.normal);
            state.Velocity.x = horizontal.x;
            state.Velocity.z = horizontal.z;
        }

        internal void SetPlanarInput(IPlanarInput input)
        {
            scriptedInput = input;
        }

        internal void SetTuning(MovementTuning value)
        {
            tuning = value;
            ApplyBody();
        }

        void ResolveInput()
        {
            if (planarInputSource != null)
            {
                return;
            }

            MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is IPlanarInput && behaviours[i] != this)
                {
                    planarInputSource = behaviours[i];
                    return;
                }
            }
        }

        void ApplyBody()
        {
            if (tuning == null || controller == null)
            {
                return;
            }

            controller.height = tuning.Height;
            controller.radius = tuning.Radius;
            controller.center = new Vector3(0f, tuning.Height * 0.5f, 0f);
            controller.stepOffset = Mathf.Min(tuning.StepOffset, tuning.Height);
            controller.slopeLimit = tuning.SlopeLimit;
            controller.skinWidth = tuning.SkinWidth;
            controller.minMoveDistance = 0f;
        }

        bool ProbeGround()
        {
            float radius = Mathf.Max(0.05f, controller.radius * 0.45f);
            Vector3 origin = transform.position + (Vector3.up * (radius + tuning.SkinWidth));
            float distance = tuning.GroundProbeDistance + tuning.SkinWidth;
            return Physics.SphereCast(
                origin,
                radius,
                Vector3.down,
                out _,
                distance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);
        }
    }
}
