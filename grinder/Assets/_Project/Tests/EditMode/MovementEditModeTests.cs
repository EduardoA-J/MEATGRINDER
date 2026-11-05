using Meatgrinder.Movement;
using Meatgrinder.Players;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Meatgrinder.Tests
{
    public sealed class MovementEditModeTests
    {
        const float Tick = 0.02f;

        [Test]
        public void TuningExposesTheDesignFields()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            Assert.Greater(tuning.GroundSpeed, 0f);
            Assert.Greater(tuning.GroundAcceleration, 0f);
            Assert.Greater(tuning.AirAcceleration, 0f);
            Assert.Greater(tuning.JumpVelocity, 0f);
            Assert.Greater(tuning.Gravity, 0f);
            Assert.Greater(tuning.Friction, 0f);
            Assert.Greater(tuning.MaxSpeed, tuning.GroundSpeed);
            Assert.Greater(tuning.AirWishSpeed, 0f);
            Assert.Less(tuning.AirWishSpeed, tuning.GroundSpeed);
            Assert.AreEqual(0f, tuning.DashCooldown);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void HoldingForwardApproachesGroundSpeed()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            float speed = Run(ref state, tuning, new Vector2(0f, 1f), false, true, 150);
            Assert.Greater(speed, tuning.GroundSpeed * 0.7f);
            Assert.LessOrEqual(speed, tuning.GroundSpeed + 0.05f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void HigherGroundAccelerationIsFasterBeforeTheCap()
        {
            MovementTuning slowTuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementTuning fastTuning = ScriptableObject.CreateInstance<MovementTuning>();
            slowTuning.SetGroundAcceleration(4f);
            fastTuning.SetGroundAcceleration(40f);
            MovementState slow = MovementState.Standing();
            MovementState fast = MovementState.Standing();
            slow.IsGrounded = true;
            fast.IsGrounded = true;
            float slowSpeed = Run(ref slow, slowTuning, new Vector2(0f, 1f), false, true, 12);
            float fastSpeed = Run(ref fast, fastTuning, new Vector2(0f, 1f), false, true, 12);
            Assert.Greater(fastSpeed, slowSpeed + 0.5f);
            Object.DestroyImmediate(slowTuning);
            Object.DestroyImmediate(fastTuning);
        }

        [Test]
        public void ReleasingDirectionAppliesFriction()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            state.Velocity = new Vector3(0f, 0f, 10f);
            float speed = Run(ref state, tuning, Vector2.zero, false, true, 20);
            Assert.Less(speed, 1f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void HigherFrictionStopsSooner()
        {
            MovementTuning light = ScriptableObject.CreateInstance<MovementTuning>();
            MovementTuning heavy = ScriptableObject.CreateInstance<MovementTuning>();
            light.SetFriction(2f);
            heavy.SetFriction(12f);
            MovementState lightState = MovementState.Standing();
            MovementState heavyState = MovementState.Standing();
            lightState.IsGrounded = true;
            heavyState.IsGrounded = true;
            lightState.Velocity = new Vector3(0f, 0f, 10f);
            heavyState.Velocity = new Vector3(0f, 0f, 10f);
            float lightSpeed = Run(ref lightState, light, Vector2.zero, false, true, 8);
            float heavySpeed = Run(ref heavyState, heavy, Vector2.zero, false, true, 8);
            Assert.Less(heavySpeed, lightSpeed);
            Object.DestroyImmediate(light);
            Object.DestroyImmediate(heavy);
        }

        [Test]
        public void JumpSetsVerticalSpeedAndBlocksASecondJump()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            MovementIntent jump = new MovementIntent { Move = Vector2.zero, Jump = true };
            MovementSimulator.Tick(ref state, jump, tuning, Vector3.forward, Vector3.right, Tick, true);
            Assert.AreEqual(tuning.JumpVelocity, state.Velocity.y, 0.001f);
            Assert.IsFalse(state.IsGrounded);
            MovementSimulator.Tick(ref state, jump, tuning, Vector3.forward, Vector3.right, Tick, true);
            Assert.Less(state.Velocity.y, tuning.JumpVelocity - 0.05f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void GravityUsesTheAssetValue()
        {
            MovementTuning light = ScriptableObject.CreateInstance<MovementTuning>();
            MovementTuning heavy = ScriptableObject.CreateInstance<MovementTuning>();
            heavy.SetGravity(light.Gravity * 2f);
            MovementState lightState = default;
            MovementState heavyState = default;
            MovementIntent intent = default;
            MovementSimulator.Tick(ref lightState, intent, light, Vector3.forward, Vector3.right, Tick, false);
            MovementSimulator.Tick(ref heavyState, intent, heavy, Vector3.forward, Vector3.right, Tick, false);
            Assert.Less(heavyState.Velocity.y, lightState.Velocity.y);
            Object.DestroyImmediate(light);
            Object.DestroyImmediate(heavy);
        }

        [Test]
        public void AirborneHorizontalSpeedSurvivesWithoutInput()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = default;
            state.Velocity = new Vector3(0f, 4f, 12f);
            float before = MovementSimulator.HorizontalSpeed(state.Velocity);
            Run(ref state, tuning, Vector2.zero, false, false, 5);
            Assert.AreEqual(before, MovementSimulator.HorizontalSpeed(state.Velocity), 0.001f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void AirInputChangesHorizontalVelocity()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = default;
            state.Velocity = new Vector3(0f, 4f, 8f);
            Run(ref state, tuning, new Vector2(1f, 0f), false, false, 3);
            Assert.Greater(state.Velocity.x, 0.2f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void AirControlRedirectsWhenAccelerationIsZero()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            tuning.SetAirAcceleration(0f);
            tuning.SetAirControl(12f);
            MovementState state = default;
            state.Velocity = new Vector3(0f, 2f, 8f);
            Run(ref state, tuning, new Vector2(1f, 0f), false, false, 20);
            Assert.Greater(Mathf.Abs(state.Velocity.x), 0.5f);
            Assert.AreEqual(8f, MovementSimulator.HorizontalSpeed(state.Velocity), 0.35f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void StrafeJumpExceedsAStraightJumpInsideMaxSpeed()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            float straight = JumpThenFly(tuning, false);
            float strafe = JumpThenFly(tuning, true);
            Assert.Greater(strafe, straight + 0.5f);
            Assert.LessOrEqual(strafe, tuning.MaxSpeed + 0.001f);
            Assert.LessOrEqual(straight, tuning.GroundSpeed + 0.2f);
            Assert.Greater(strafe, tuning.GroundSpeed + 1f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void SpeedStaysUnderTheSafetyCap()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            tuning.SetAirWishSpeed(40f);
            tuning.SetAirAcceleration(400f);
            tuning.SetMaxSpeed(12f);
            MovementState state = default;
            float speed = Run(ref state, tuning, new Vector2(0f, 1f), false, false, 20);
            Assert.Greater(speed, 11f);
            Assert.LessOrEqual(speed, tuning.MaxSpeed + 0.001f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void HoldingSpaceDoesNotRefreshTheJump()
        {
            bool held = false;
            float buffer = 0f;
            PlayerMovement.SampleJump(ref held, ref buffer, true, 0.12f, 0.02f, out bool jump);
            Assert.IsTrue(jump);
            for (int i = 0; i < 8; i++)
            {
                PlayerMovement.SampleJump(ref held, ref buffer, true, 0.12f, 0.02f, out jump);
            }

            Assert.IsFalse(jump);
            PlayerMovement.SampleJump(ref held, ref buffer, false, 0.12f, 0.02f, out jump);
            Assert.IsFalse(jump);
            PlayerMovement.SampleJump(ref held, ref buffer, true, 0.12f, 0.02f, out jump);
            Assert.IsTrue(jump);
        }

        [Test]
        public void TimedHopKeepsSpeedAboveGroundSpeed()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            state.LandingAge = 0f;
            state.Velocity = new Vector3(0f, 0f, 16f);
            MovementIntent hop = new MovementIntent { Jump = true };
            MovementSimulator.Tick(ref state, hop, tuning, Vector3.forward, Vector3.right, Tick, true);
            float kept = MovementSimulator.HorizontalSpeed(state.Velocity);
            Assert.Greater(kept, tuning.GroundSpeed);
            Assert.AreEqual(16f, kept, 0.05f);

            tuning.SetBunnyHopSpeedRetention(0.5f);
            state = MovementState.Standing();
            state.IsGrounded = true;
            state.LandingAge = 0f;
            state.Velocity = new Vector3(0f, 0f, 16f);
            MovementSimulator.Tick(ref state, hop, tuning, Vector3.forward, Vector3.right, Tick, true);
            Assert.AreEqual(8f, MovementSimulator.HorizontalSpeed(state.Velocity), 0.05f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void ALongLandingWithoutJumpAppliesFriction()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            state.LandingAge = 10f;
            state.Velocity = new Vector3(0f, 0f, 16f);
            float speed = Run(ref state, tuning, Vector2.zero, false, true, 5);
            Assert.Less(speed, 15f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void TheSameTickCountIsRepeatable()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState first = MovementState.Standing();
            MovementState second = MovementState.Standing();
            first.IsGrounded = true;
            second.IsGrounded = true;
            float a = Run(ref first, tuning, new Vector2(0f, 1f), false, true, 40);
            float b = Run(ref second, tuning, new Vector2(0f, 1f), false, true, 40);
            Assert.AreEqual(a, b, 0.0001f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void HalfTicksStayCloseToTheFixedStep()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            MovementState coarse = MovementState.Standing();
            MovementState fine = MovementState.Standing();
            coarse.IsGrounded = true;
            fine.IsGrounded = true;
            float coarseSpeed = Run(ref coarse, tuning, new Vector2(0f, 1f), false, true, 50, 0.02f);
            float fineSpeed = Run(ref fine, tuning, new Vector2(0f, 1f), false, true, 100, 0.01f);
            Assert.AreEqual(coarseSpeed, fineSpeed, 1.25f);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void LookYawsTheBodyAndClampsPitch()
        {
            MovementTuning tuning = ScriptableObject.CreateInstance<MovementTuning>();
            GameObject body = new GameObject("look-body");
            GameObject cameraObject = new GameObject("look-camera");
            cameraObject.transform.SetParent(body.transform, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            PlayerLook look = body.AddComponent<PlayerLook>();
            look.SetTuning(tuning);
            float yaw = look.Yaw;
            float pitch = look.Pitch;
            look.ApplyLook(Vector2.zero);
            Assert.AreEqual(yaw, look.Yaw, 0.001f);
            Assert.AreEqual(pitch, look.Pitch, 0.001f);

            look.ApplyLook(new Vector2(100f, 0f));
            Assert.Greater(look.Yaw, yaw);
            Assert.AreEqual(0f, body.transform.eulerAngles.x, 0.1f);
            Assert.AreEqual(0f, camera.transform.localEulerAngles.y, 0.1f);

            look.ApplyLook(new Vector2(0f, 5000f));
            Assert.AreEqual(tuning.MinPitch, look.Pitch, 0.001f);
            Assert.LessOrEqual(camera.transform.localPosition.y, tuning.Height);
            Assert.AreEqual(0f, camera.transform.localPosition.z, 0.001f);
            Object.DestroyImmediate(body);
            Object.DestroyImmediate(tuning);
        }

        [Test]
        public void GameActionsBindWasdSpaceAndMouseWithoutCrouch()
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                "Assets/_Project/02_ScriptableObjects/Input/MeatgrinderActions.inputactions");
            Assert.NotNull(asset);
            InputActionMap map = asset.FindActionMap("Player", true);
            Assert.IsNull(map.FindAction("Crouch"));
            Assert.IsNull(map.FindAction("Sprint"));
            Assert.NotNull(map.FindAction("Attack", true));
            Assert.IsTrue(HasBinding(map.FindAction("Move", true), "<Keyboard>/w"));
            Assert.IsTrue(HasBinding(map.FindAction("Jump", true), "<Keyboard>/space"));
            Assert.IsTrue(HasBinding(map.FindAction("Look", true), "<Pointer>/delta"));
            Assert.IsTrue(HasBinding(map.FindAction("Attack", true), "<Mouse>/leftButton"));
            Debug.Log("[Movement] Actions Move, Look, Jump and Attack are bound. Crouch and Sprint are absent.");
        }

        static bool HasBinding(InputAction action, string path)
        {
            foreach (InputBinding binding in action.bindings)
            {
                if (binding.path == path || binding.effectivePath == path)
                {
                    return true;
                }
            }

            return false;
        }

        static float JumpThenFly(MovementTuning tuning, bool strafe)
        {
            MovementState state = MovementState.Standing();
            state.IsGrounded = true;
            Run(ref state, tuning, new Vector2(0f, 1f), false, true, 150);
            MovementIntent jump = new MovementIntent { Move = new Vector2(0f, 1f), Jump = true };
            MovementSimulator.Tick(ref state, jump, tuning, Vector3.forward, Vector3.right, Tick, true);
            float yaw = 0f;
            for (int i = 0; i < 40; i++)
            {
                Vector3 forward = Vector3.forward;
                Vector3 right = Vector3.right;
                Vector2 move = new Vector2(0f, 1f);
                if (strafe)
                {
                    yaw += 4f;
                    Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
                    forward = rotation * Vector3.forward;
                    right = rotation * Vector3.right;
                    move = new Vector2(1f, 0f);
                }

                MovementIntent intent = new MovementIntent { Move = move, Jump = false };
                MovementSimulator.Tick(ref state, intent, tuning, forward, right, Tick, false);
            }

            return MovementSimulator.HorizontalSpeed(state.Velocity);
        }

        static float Run(
            ref MovementState state,
            MovementTuning tuning,
            Vector2 move,
            bool jump,
            bool grounded,
            int ticks,
            float deltaTime = Tick)
        {
            MovementIntent intent = new MovementIntent { Move = move, Jump = jump };
            for (int i = 0; i < ticks; i++)
            {
                MovementSimulator.Tick(
                    ref state,
                    intent,
                    tuning,
                    Vector3.forward,
                    Vector3.right,
                    deltaTime,
                    grounded);
            }

            return MovementSimulator.HorizontalSpeed(state.Velocity);
        }
    }
}
