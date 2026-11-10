using UnityEngine;

namespace Meatgrinder.Movement
{
    // Local integrator for ground friction, ground acceleration, a short air wish and timed hops.
    // Inspected reference: rogerli2020/SourceEngineMovementUnity. That repository has no license,
    // so none of its files were copied.
    // D-18 stays open: Gravity here is design data. PhysX gravity is not applied.
    // Fusion later: call Tick from FixedUpdateNetwork with a replicated MovementIntent.
    // Do not read devices inside Tick. Reconcile MovementState.Velocity and the pose.
    public static class MovementSimulator
    {
        const float SpeedEpsilon = 0.0001f;

        public static void Tick(
            ref MovementState state,
            in MovementIntent intent,
            MovementTuning tuning,
            Vector3 forward,
            Vector3 right,
            float deltaTime,
            bool collisionGrounded,
            float speedScale = 1f)
        {
            if (tuning == null || deltaTime <= 0f)
            {
                return;
            }

            if (state.GroundLockRemaining > 0f)
            {
                state.GroundLockRemaining -= deltaTime;
                collisionGrounded = false;
            }

            bool wasGrounded = state.IsGrounded;
            state.IsGrounded = collisionGrounded;
            if (state.IsGrounded && wasGrounded == false)
            {
                state.LandingAge = 0f;
            }

            forward.y = 0f;
            right.y = 0f;
            if (forward.sqrMagnitude > SpeedEpsilon)
            {
                forward.Normalize();
            }

            if (right.sqrMagnitude > SpeedEpsilon)
            {
                right.Normalize();
            }

            Vector3 wishDirection = (forward * intent.Move.y) + (right * intent.Move.x);
            if (wishDirection.sqrMagnitude > 1f)
            {
                wishDirection.Normalize();
            }

            float scale = speedScale > 0f ? speedScale : 1f;
            if (state.IsGrounded)
            {
                TickGround(ref state, intent, tuning, wishDirection, deltaTime, scale);
            }
            else
            {
                TickAir(ref state, tuning, wishDirection, deltaTime, scale);
            }

            Sanitize(ref state.Velocity);
        }

        public static float HorizontalSpeed(Vector3 velocity)
        {
            velocity.y = 0f;
            return velocity.magnitude;
        }

        static void TickGround(
            ref MovementState state,
            in MovementIntent intent,
            MovementTuning tuning,
            Vector3 wishDirection,
            float deltaTime,
            float speedScale)
        {
            bool hop = intent.Jump && state.LandingAge <= tuning.BunnyHopWindow;
            if (hop == false)
            {
                ApplyFriction(ref state.Velocity, tuning, deltaTime);
            }

            Accelerate(
                ref state.Velocity,
                wishDirection,
                tuning.GroundSpeed * speedScale,
                tuning.GroundAcceleration,
                tuning.MaxSpeed * speedScale,
                deltaTime,
                0f);

            if (intent.Jump)
            {
                if (hop)
                {
                    ApplyRetention(ref state.Velocity, tuning);
                }

                state.Velocity.y = tuning.JumpVelocity;
                state.IsGrounded = false;
                state.GroundLockRemaining = tuning.JumpGroundLock;
                state.LandingAge = tuning.BunnyHopWindow + deltaTime;
                return;
            }

            state.Velocity.y = -tuning.GroundStick;
            state.LandingAge += deltaTime;
        }

        static void TickAir(
            ref MovementState state,
            MovementTuning tuning,
            Vector3 wishDirection,
            float deltaTime,
            float speedScale)
        {
            state.Velocity.y -= tuning.Gravity * deltaTime;
            Accelerate(
                ref state.Velocity,
                wishDirection,
                tuning.AirWishSpeed * speedScale,
                tuning.AirAcceleration,
                tuning.MaxSpeed * speedScale,
                deltaTime,
                tuning.AirControl);
        }

        static void ApplyFriction(ref Vector3 velocity, MovementTuning tuning, float deltaTime)
        {
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            float speed = horizontal.magnitude;
            if (speed <= SpeedEpsilon)
            {
                velocity.x = 0f;
                velocity.z = 0f;
                return;
            }

            float control = Mathf.Max(speed, tuning.StopSpeed);
            float drop = control * tuning.Friction * deltaTime;
            float newSpeed = Mathf.Max(0f, speed - drop);
            if (newSpeed < 0.05f)
            {
                newSpeed = 0f;
            }

            horizontal *= newSpeed / speed;
            velocity.x = horizontal.x;
            velocity.z = horizontal.z;
        }

        static void ApplyRetention(ref Vector3 velocity, MovementTuning tuning)
        {
            float speed = HorizontalSpeed(velocity);
            if (speed <= tuning.GroundSpeed)
            {
                return;
            }

            float scale = Mathf.Clamp01(tuning.BunnyHopSpeedRetention);
            velocity.x *= scale;
            velocity.z *= scale;
        }

        static void Accelerate(
            ref Vector3 velocity,
            Vector3 wishDirection,
            float wishSpeed,
            float acceleration,
            float maxSpeed,
            float deltaTime,
            float airControl)
        {
            Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
            if (wishDirection.sqrMagnitude > SpeedEpsilon && wishSpeed > 0f && acceleration > 0f)
            {
                Vector3 wish = wishDirection.normalized;
                float current = Vector3.Dot(horizontal, wish);
                float add = wishSpeed - current;
                if (add > 0f)
                {
                    float accel = Mathf.Min(acceleration * deltaTime * wishSpeed, add);
                    horizontal += wish * accel;
                }
            }

            horizontal = Vector3.ClampMagnitude(horizontal, maxSpeed);
            if (airControl > 0f && wishDirection.sqrMagnitude > SpeedEpsilon && horizontal.sqrMagnitude > SpeedEpsilon)
            {
                float speed = horizontal.magnitude;
                Vector3 currentDirection = horizontal / speed;
                float blend = 1f - Mathf.Exp(-airControl * deltaTime);
                Vector3 redirected = Vector3.Slerp(currentDirection, wishDirection.normalized, blend);
                if (redirected.sqrMagnitude > SpeedEpsilon)
                {
                    horizontal = redirected.normalized * speed;
                }

                horizontal = Vector3.ClampMagnitude(horizontal, maxSpeed);
            }

            velocity.x = horizontal.x;
            velocity.z = horizontal.z;
        }

        static void Sanitize(ref Vector3 velocity)
        {
            if (float.IsNaN(velocity.x) || float.IsInfinity(velocity.x))
            {
                velocity.x = 0f;
            }

            if (float.IsNaN(velocity.y) || float.IsInfinity(velocity.y))
            {
                velocity.y = 0f;
            }

            if (float.IsNaN(velocity.z) || float.IsInfinity(velocity.z))
            {
                velocity.z = 0f;
            }
        }
    }
}
