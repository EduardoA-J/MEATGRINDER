using Meatgrinder.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Meatgrinder.Players
{
    [DefaultExecutionOrder(-50)]
    public sealed class PlayerInputReader : MonoBehaviour, IPlanarInput, IStanceInput
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
        InputAction meleeAction;
        InputAction sprintAction;
        InputAction crouchAction;
        Vector2 lookDelta;
        bool loggedSample;
        bool attackLatched;
        bool reloadLatched;
        bool meleeLatched;
        int slotLatched;

        public bool ExternalInput { get; set; }
        Vector2 externalMove;
        bool externalJump, externalSprint, externalCrouch;
        public Vector2 Move => ExternalInput ? externalMove : moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        public bool JumpHeld => ExternalInput ? externalJump : jumpAction != null && jumpAction.IsPressed();
        public Vector2 LookDelta => lookDelta;
        public bool SprintHeld => ExternalInput ? externalSprint : sprintAction != null && sprintAction.IsPressed();
        public bool CrouchHeld => ExternalInput ? externalCrouch : crouchAction != null && crouchAction.IsPressed();

        public void SetExternalInput(Vector2 move, bool jump, bool sprint, bool crouch, bool attack, bool reload, bool melee, int slot)
        {
            externalMove = move;
            externalJump = jump;
            externalSprint = sprint;
            externalCrouch = crouch;
            attackLatched |= attack;
            reloadLatched |= reload;
            meleeLatched |= melee;
            if (slot > 0) slotLatched = slot;
        }

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

        public bool ConsumeMelee()
        {
            bool value = meleeLatched;
            meleeLatched = false;
            return value;
        }

        void OnEnable()
        {
            if (ExternalInput) return;
            Bind();
            playerMap?.Enable();
        }

        void OnDisable()
        {
            playerMap?.Disable();
            attackLatched = reloadLatched = meleeLatched = false;
            slotLatched = 0;
            lookDelta = Vector2.zero;
        }

        void Update()
        {
            if (ExternalInput) return;
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

            if (meleeAction != null && meleeAction.WasPressedThisFrame())
            {
                meleeLatched = true;
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
            meleeAction = playerMap.FindAction("Melee", false);
            sprintAction = playerMap.FindAction("Sprint", false);
            crouchAction = playerMap.FindAction("Crouch", false);
        }
    }
}
