using Meatgrinder.Combat;
using Meatgrinder.Movement;
using UnityEngine;

namespace Meatgrinder.Players
{
    [RequireComponent(typeof(PlayerMovement))]
    public sealed class PlayerImpulseReceiver : MonoBehaviour, IImpulseReceiver
    {
        public void AddImpulse(Vector3 impulse) => GetComponent<PlayerMovement>().AddImpulse(impulse);
    }
}
