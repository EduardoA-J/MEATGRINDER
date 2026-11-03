using Meatgrinder.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Meatgrinder.Players
{
    public sealed class PlayerLook : MonoBehaviour
    {
        [SerializeField] private MovementTuning tuning;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Camera view;

        [SerializeField] private float recoilRecovery = 40f; // PENDIENTE

        float pitch;
        float yaw;
        float recoilOffset;

        public float Pitch => pitch;
        public float Yaw => yaw;
        public Camera View => view;

        void Awake()
        {
            if (view == null)
            {
                view = GetComponentInChildren<Camera>();
            }

            yaw = transform.eulerAngles.y;
            LockCursor();
            ApplyPose();
        }

        void Update()
        {
            if (tuning == null)
            {
                return;
            }

            if (Application.isBatchMode == false)
            {
                UpdateCursor();
            }

            if (recoilOffset > 0f)
            {
                recoilOffset = Mathf.MoveTowards(recoilOffset, 0f, recoilRecovery * Time.deltaTime);
            }

            Vector2 delta = input != null ? input.LookDelta : Vector2.zero;
            ApplyLook(delta);
        }

        public void AddRecoil(float degrees)
        {
            if (degrees <= 0f)
            {
                return;
            }

            recoilOffset = Mathf.Min(recoilOffset + degrees, 12f);
        }

        internal void SetView(Camera value)
        {
            view = value;
            ApplyPose();
        }

        internal void SetTuning(MovementTuning value)
        {
            tuning = value;
            ApplyPose();
        }

        internal void ApplyLook(Vector2 delta)
        {
            if (tuning == null)
            {
                return;
            }

            yaw += delta.x * tuning.LookSensitivity;
            pitch = Mathf.Clamp(
                pitch - (delta.y * tuning.LookSensitivity),
                tuning.MinPitch,
                tuning.MaxPitch);
            ApplyPose();
        }

        void ApplyPose()
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            if (view == null || tuning == null)
            {
                return;
            }

            float eye = Mathf.Max(0.2f, tuning.Height - 0.15f);
            view.transform.localPosition = new Vector3(0f, eye, 0f);
            view.transform.localRotation = Quaternion.Euler(pitch - recoilOffset, 0f, 0f);
            view.fieldOfView = tuning.FieldOfView;
            view.nearClipPlane = 0.05f;
        }

        void LockCursor()
        {
            if (Application.isPlaying == false || Application.isBatchMode)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void UpdateCursor()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            Mouse mouse = Mouse.current;
            if (mouse != null
                && mouse.leftButton.wasPressedThisFrame
                && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }
    }
}
