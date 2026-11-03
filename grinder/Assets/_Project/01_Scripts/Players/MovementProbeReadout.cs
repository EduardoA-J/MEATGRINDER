using Meatgrinder.Movement;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class MovementProbeReadout : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        void Awake()
        {
            if (movement == null)
            {
                movement = GetComponent<PlayerMovement>();
            }
        }

        void OnGUI()
        {
            if (movement == null)
            {
                return;
            }

            string grounded = movement.IsGrounded ? "suelo" : "aire";
            GUI.Label(
                new Rect(16f, 16f, 480f, 24f),
                "vel " + movement.HorizontalSpeed.ToString("0.00") + "  " + grounded);
        }
    }
}
