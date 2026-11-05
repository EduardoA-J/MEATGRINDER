using Meatgrinder.Movement;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class MovementProbeReadout : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        float sessionMax;
        float previousSpeed;
        float speedDelta;

        void Awake()
        {
            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }
        }

        void Update()
        {
            if (movement == null)
            {
                return;
            }

            float speed = movement.HorizontalSpeed;
            speedDelta = speed - previousSpeed;
            previousSpeed = speed;
            if (speed > sessionMax)
            {
                sessionMax = speed;
            }
        }

        void OnGUI()
        {
            if (movement == null)
            {
                return;
            }

            string grounded = movement.IsGrounded ? "suelo" : "aire";
            string jump = movement.JumpBuffered ? "salto" : "quieto";
            GUI.Label(
                new Rect(16f, 16f, 640f, 40f),
                "vel " + movement.HorizontalSpeed.ToString("0.00")
                + "   max " + sessionMax.ToString("0.00")
                + "   " + grounded
                + "\ndvel " + speedDelta.ToString("+0.00;-0.00;0.00")
                + "   " + jump);
        }
    }
}
