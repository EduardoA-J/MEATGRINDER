using UnityEngine;

namespace Meatgrinder.Weapons
{
    public interface IImpactPresenter
    {
        void Present(Vector3 point, bool damagedTarget);
    }
}
