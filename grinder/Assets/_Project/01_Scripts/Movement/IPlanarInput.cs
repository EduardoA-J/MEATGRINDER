using UnityEngine;

namespace Meatgrinder.Movement
{
    public interface IPlanarInput
    {
        Vector2 Move { get; }
        bool JumpHeld { get; }
        Vector2 LookDelta { get; }
    }
}
