using Meatgrinder.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Meatgrinder.Players
{
    [DefaultExecutionOrder(-50)]
    public sealed class PlayerInputReader : MonoBehaviour, IPlanarInput
    {
        [SerializeField] private InputActionAsset actions;

        InputActionMap playerMap;
        InputAction moveAction;
        InputAction lookAction;
        InputAction jumpAction;
        InputAction attackAction;
        InputAction reloadAction;
        InputAction slot1Action;
        InputAction slot2Action;
        Vector2 lookDelta;
        bool loggedSample;
        bool attackLatched;
        bool reloadLatched;
        int slotLatched;

        public Vector2 Move => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        public bool JumpHeld => jumpAction != null && jumpAction.IsPressed();
        public Vector2 LookDelta => lookDelta;

        public bool ConsumeAttack()
        {
            bool value = attackLatched;
            attackLatched = false;
            return value;
        }

        public bool ConsumeReload()
        {
            bool value = reloadLatched;
            reloadLatched = false;
            return value;
        }

        public int ConsumeWeaponSlot()
        {
            int value = slotLatched;
            slotLatched = 0;
            return value;
        }

        void OnEnable()
        {
            Bind();
            playerMap?.Enable();
        }

        void OnDisable()
        {
            playerMap?.Disable();
        }

        void Update()
        {
            Poll();
            Latch();
        }

        void Latch()
        {
            if (attackAction != null && attackAction.WasPressedThisFrame())
            {
                attackLatched = true;
            }

            if (reloadAction != null && reloadAction.WasPressedThisFrame())
            {
                reloadLatched = true;
            }

            if (slot1Action != null && slot1Action.WasPressedThisFrame())
            {
                slotLatched = 1;
            }

            if (slot2Action != null && slot2Action.WasPressedThisFrame())
            {
                slotLatched = 2;
            }
        }

        internal void AssignAsset(InputActionAsset asset)
        {
            actions = asset;
            Bind();
            playerMap?.Enable();
        }

        internal void Poll()
        {
            lookDelta = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
            if (loggedSample || Move.sqrMagnitude <= 0.01f)
            {
                return;
            }

            Debug.Log("[Movement] Move " + Move + " Look " + lookDelta);
            loggedSample = true;
        }

        void Bind()
        {
            if (actions == null)
            {
                return;
            }

            playerMap = actions.FindActionMap("Player", true);
            moveAction = playerMap.FindAction("Move", true);
            lookAction = playerMap.FindAction("Look", true);
            jumpAction = playerMap.FindAction("Jump", true);
            attackAction = playerMap.FindAction("Attack", true);
            reloadAction = playerMap.FindAction("Reload");
            slot1Action = playerMap.FindAction("WeaponSlot1");
            slot2Action = playerMap.FindAction("WeaponSlot2");
        }
    }
}
