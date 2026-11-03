using System.Collections;
using System.Text;
using Meatgrinder.Movement;
using Meatgrinder.Players;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class MovementPlayModeTests
    {
        [UnityTest]
        public IEnumerator WasdSpaceAndMouseReachTheReader()
        {
            InputSettings.BackgroundBehavior previousBehavior = InputSystem.settings.backgroundBehavior;
            InputSettings.UpdateMode previousMode = InputSystem.settings.updateMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
            Keyboard keyboard = null;
            Mouse mouse = null;
            GameObject host = null;
            InputActionAsset asset = null;
            try
            {
            for (int i = InputSystem.devices.Count - 1; i >= 0; i--)
            {
                InputDevice device = InputSystem.devices[i];
                if (device is Keyboard || device is Mouse)
                {
                    InputSystem.RemoveDevice(device);
                }
            }

            keyboard = InputSystem.AddDevice<Keyboard>();
            mouse = InputSystem.AddDevice<Mouse>();
            keyboard.MakeCurrent();
            mouse.MakeCurrent();
            using (StateEvent.From(keyboard, out InputEventPtr keyEvent, Allocator.Temp))
            {
                keyboard.wKey.WriteValueIntoEvent(1f, keyEvent);
                keyboard.spaceKey.WriteValueIntoEvent(1f, keyEvent);
                InputSystem.QueueEvent(keyEvent);
            }

            InputSystem.Update();
            asset = CreateActions();
            foreach (InputAction created in asset.FindActionMap("Player", true).actions)
            {
                created.wantsInitialStateCheck = true;
            }

            host = new GameObject("input-host");
            PlayerInputReader reader = host.AddComponent<PlayerInputReader>();
            reader.AssignAsset(asset);
            using (StateEvent.From(mouse, out InputEventPtr mouseEvent, Allocator.Temp))
            {
                mouse.delta.WriteValueIntoEvent(new Vector2(4f, -2f), mouseEvent);
                InputSystem.QueueEvent(mouseEvent);
            }

            InputSystem.Update();
            reader.Poll();
            string sample = Describe(asset, keyboard, reader);
            Debug.Log("[Movement] " + sample);
            Assert.IsTrue(keyboard.wKey.isPressed, sample);
            Assert.IsTrue(keyboard.spaceKey.isPressed, sample);
            Assert.That(sample, Does.Contain("/Keyboard/w:True"));
            InputAction look = asset.FindAction("Player/Look", true);
            bool lookBound = false;
            for (int i = 0; i < look.controls.Count; i++)
            {
                if (look.controls[i].path.Contains("delta"))
                {
                    lookBound = true;
                }
            }

            Assert.IsTrue(lookBound, sample);
            }
            finally
            {
                InputSystem.settings.backgroundBehavior = previousBehavior;
                InputSystem.settings.updateMode = previousMode;
                if (host != null)
                {
                    Object.Destroy(host);
                }

                if (asset != null)
                {
                    Object.Destroy(asset);
                }

                if (keyboard != null)
                {
                    InputSystem.RemoveDevice(keyboard);
                }

                if (mouse != null)
                {
                    InputSystem.RemoveDevice(mouse);
                }
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator CapsuleStaysOnTheFloorForTenPasses()
        {
            MovementRig rig = CreateRig(new Vector3(80f, 0f, 80f));
            rig.Input.Move = new Vector2(0f, 1f);
            for (int pass = 0; pass < 10; pass++)
            {
                rig.Player.transform.position = rig.Origin + new Vector3(0f, 0.05f, -6f);
                for (int frame = 0; frame < 20; frame++)
                {
                    yield return new WaitForFixedUpdate();
                }

                float feet = rig.Player.transform.position.y;
                Assert.Greater(feet, -0.05f, "Pass " + pass + " fell through the floor.");
                Assert.Less(feet, 0.35f, "Pass " + pass + " left the floor.");
            }

            rig.Destroy();
        }

        [UnityTest]
        public IEnumerator BodyDoesNotPassThroughAWall()
        {
            MovementRig rig = CreateRig(new Vector3(80f, 0f, 120f));
            CreateBox("Wall", rig.Origin + new Vector3(0f, 1.5f, 2.2f), new Vector3(4f, 3f, 1f));
            rig.Input.Move = new Vector2(0f, 1f);
            for (int frame = 0; frame < 180; frame++)
            {
                yield return new WaitForFixedUpdate();
            }

            float face = rig.Origin.z + 2.2f - 0.5f;
            Assert.Less(rig.Player.transform.position.z, face - 0.2f);
            rig.Destroy();
        }

        [UnityTest]
        public IEnumerator LowStepIsClimbedAndTheCameraStaysInsideTheBody()
        {
            MovementRig rig = CreateRig(new Vector3(80f, 0f, 160f));
            CreateBox("Step", rig.Origin + new Vector3(0f, 0.15f, 4f), new Vector3(6f, 0.3f, 8f));
            CreateBox("StepBackstop", rig.Origin + new Vector3(0f, 1.5f, 8.5f), new Vector3(6f, 3f, 1f));
            rig.Input.Move = new Vector2(0f, 1f);
            float peak = rig.Player.transform.position.y;
            for (int frame = 0; frame < 160; frame++)
            {
                yield return new WaitForFixedUpdate();
                peak = Mathf.Max(peak, rig.Player.transform.position.y);
            }

            Assert.Greater(peak, 0.2f, "Peak feet height was " + peak);
            Camera view = rig.Player.GetComponentInChildren<Camera>();
            Assert.NotNull(view);
            Assert.AreEqual(0f, view.transform.localPosition.z, 0.001f);
            Assert.Less(view.transform.localPosition.y, rig.Tuning.Height);
            rig.Destroy();
        }

        [UnityTest]
        public IEnumerator RampRaisesTheBody()
        {
            MovementRig rig = CreateRig(new Vector3(80f, 0f, 200f));
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ramp";
            const float angle = 18f;
            ramp.transform.localScale = new Vector3(4f, 0.5f, 12f);
            ramp.transform.rotation = Quaternion.Euler(-angle, 0f, 0f);
            Vector3 lip = ramp.transform.rotation * new Vector3(0f, 0.25f, -6f);
            Vector3 lipWorld = rig.Origin + new Vector3(0f, 0.05f, 2.5f);
            ramp.transform.position = lipWorld - lip;
            rig.Input.Move = new Vector2(0f, 1f);
            float start = rig.Player.transform.position.y;
            float peak = start;
            for (int frame = 0; frame < 180; frame++)
            {
                yield return new WaitForFixedUpdate();
                peak = Mathf.Max(peak, rig.Player.transform.position.y);
            }

            Assert.Greater(peak, start + 0.35f, "Peak feet height was " + peak + " from " + start);
            rig.Destroy();
            Object.Destroy(ramp);
        }

        sealed class ScriptedInput : IPlanarInput
        {
            public Vector2 Move { get; set; }
            public bool JumpHeld { get; set; }
            public Vector2 LookDelta { get; set; }
        }

        sealed class MovementRig
        {
            public GameObject Player;
            public MovementTuning Tuning;
            public ScriptedInput Input;
            public Vector3 Origin;
            public GameObject Floor;

            public void Destroy()
            {
                if (Player != null)
                {
                    Object.Destroy(Player);
                }

                if (Floor != null)
                {
                    Object.Destroy(Floor);
                }

                if (Tuning != null)
                {
                    Object.Destroy(Tuning);
                }
            }
        }

        static string Describe(InputActionAsset asset, Keyboard keyboard, PlayerInputReader reader)
        {
            InputAction move = asset.FindAction("Player/Move", true);
            StringBuilder text = new StringBuilder();
            text.Append("move=");
            text.Append(reader.Move);
            text.Append(" jump=");
            text.Append(reader.JumpHeld);
            text.Append(" look=");
            text.Append(reader.LookDelta);
            text.Append(" w=");
            text.Append(keyboard.wKey.isPressed);
            text.Append(" controls=");
            for (int i = 0; i < move.controls.Count; i++)
            {
                InputControl control = move.controls[i];
                text.Append(control.path);
                text.Append(':');
                text.Append(control.IsPressed());
                text.Append(';');
            }

            return text.ToString();
        }

        static InputActionAsset CreateActions()
        {
            InputActionAsset asset = ScriptableObject.CreateInstance<InputActionAsset>();
            InputActionMap map = asset.AddActionMap("Player");
            InputAction move = map.AddAction("Move", InputActionType.Value);
            move.expectedControlType = "Vector2";
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            InputAction look = map.AddAction("Look", InputActionType.Value);
            look.expectedControlType = "Vector2";
            look.AddBinding("<Pointer>/delta");
            map.AddAction("Jump", InputActionType.Button).AddBinding("<Keyboard>/space");
            map.AddAction("Attack", InputActionType.Button).AddBinding("<Mouse>/leftButton");
            return asset;
        }

        static MovementRig CreateRig(Vector3 origin)
        {
            MovementRig rig = new MovementRig();
            rig.Origin = origin;
            rig.Tuning = ScriptableObject.CreateInstance<MovementTuning>();
            rig.Input = new ScriptedInput();
            rig.Floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rig.Floor.transform.position = origin + new Vector3(0f, -0.5f, 20f);
            rig.Floor.transform.localScale = new Vector3(24f, 1f, 64f);
            rig.Player = new GameObject("MovementRig");
            rig.Player.transform.position = origin + new Vector3(0f, 0.05f, 0f);
            rig.Player.AddComponent<CharacterController>();
            GameObject view = new GameObject("View");
            view.transform.SetParent(rig.Player.transform, false);
            view.AddComponent<Camera>();
            PlayerMovement movement = rig.Player.AddComponent<PlayerMovement>();
            movement.SetTuning(rig.Tuning);
            movement.SetPlanarInput(rig.Input);
            return rig;
        }

        static void CreateBox(string name, Vector3 position, Vector3 scale)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = position;
            box.transform.localScale = scale;
        }
    }
}
