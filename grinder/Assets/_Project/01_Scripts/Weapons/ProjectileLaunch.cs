using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Weapons
{
    public sealed class ProjectileLaunch
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public WeaponDefinition Weapon;
        public DamageResolver Resolver;
        public IImpactPresenter Presenter;
        public Transform Owner;
        public int AttackerId;
        public int AttackerTeam;
        public ulong ConfirmationBase;
        public bool HasAuthority = true;
    }
}
