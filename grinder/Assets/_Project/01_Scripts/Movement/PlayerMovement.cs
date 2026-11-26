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
        ISpeedScale speedScale;
        bool jumpWasHeld;
        float jumpBuffer;

        public bool IsCrouching { get; private set; }
        public float BodyHeight => tuning == null ? 1.8f : IsCrouching ? tuning.CrouchHeight : tuning.Height;
        public Vector3 Velocity => state.Velocity;

        public void ResetMotion()
        {
            state = MovementState.Standing();
            collisionGrounded = false;
            jumpWasHeld = false;
            jumpBuffer = 0f;
            IsCrouching = false;
            ApplyBody();
        }

        public void AddImpulse(Vector3 impulse)
        {
            state.Velocity += impulse;
            if (impulse.y > 0f)
            {
                state.IsGrounded = false;
                collisionGrounded = false;
            }
        }
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

            float deltaTime = Time.fixedDeltaTime;
            IPlanarInput input = scriptedInput ?? (planarInputSource as IPlanarInput);
            IStanceInput stance = input as IStanceInput;
            if (stance != null && stance.CrouchHeld) IsCrouching = true;
            else if (IsCrouching && CanStand()) IsCrouching = false;
            ApplyBody();
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
            if (speedScale == null)
            {
                speedScale = GetComponent<ISpeedScale>();
            }

            float scale = 1f;
            if (speedScale != null)
            {
                float sampled = speedScale.GetSpeedMultiplier();
                scale = sampled > 0f ? sampled : 1f;
            }

            float desiredSpeed = IsCrouching ? tuning.CrouchSpeed : stance != null && stance.SprintHeld ? tuning.SprintSpeed : tuning.GroundSpeed;
            scale *= desiredSpeed / Mathf.Max(0.01f, tuning.GroundSpeed);
            MovementSimulator.Tick(
                ref state,
                intent,
                tuning,
                transform.forward,
                transform.right,
                deltaTime,
                collisionGrounded,
                scale);

            CollisionFlags flags = controller.Move(state.Velocity * deltaTime);
            if ((flags & CollisionFlags.Above) != 0 && state.Velocity.y > 0f)
            {
                state.Velocity.y = 0f;
            }

            SnapToWalkableGround();
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

        public void SetTuning(MovementTuning value)
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

            controller.height = BodyHeight;
            controller.radius = tuning.Radius;
            controller.center = new Vector3(0f, BodyHeight * 0.5f, 0f);
            controller.stepOffset = Mathf.Min(tuning.StepOffset, BodyHeight - 0.01f);
            controller.slopeLimit = tuning.SlopeLimit;
            controller.skinWidth = tuning.SkinWidth;
            controller.minMoveDistance = 0f;
        }

        bool CanStand()
        {
            float radius = Mathf.Max(0.01f, tuning.Radius - tuning.SkinWidth);
            Vector3 bottom = transform.position + Vector3.up * (radius + tuning.SkinWidth);
            Vector3 top = transform.position + Vector3.up * (tuning.Height - radius);
            foreach (Collider found in Physics.OverlapCapsule(bottom, top, radius, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                if (!found.transform.IsChildOf(transform)) return false;
            return true;
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

        void SnapToWalkableGround()
        {
            if (controller == null || tuning == null || state.Velocity.y > 0.1f) return;
            Vector3 origin = transform.position + Vector3.up * 0.55f;
            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1.1f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return;
            if (hit.collider.transform.IsChildOf(transform) || hit.normal.y < 0.7f) return;
            float delta = hit.point.y - transform.position.y;
            if (delta < -0.2f || delta > 0.25f) return;
            if (delta > 0f)
            {
                controller.enabled = false;
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                controller.enabled = true;
            }
        }
    }
}
