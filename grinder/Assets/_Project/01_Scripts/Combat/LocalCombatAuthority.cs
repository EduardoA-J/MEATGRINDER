using UnityEngine;

namespace Meatgrinder.Combat
{
    // Local scenes have no Fusion runner. A remote client must leave this false and send the shot as intent.
    public sealed class LocalCombatAuthority : MonoBehaviour, ICombatAuthority
    {
        [SerializeField] private bool authoritative = true;

        public bool IsAuthoritative => authoritative;

        internal void SetAuthoritative(bool value)
        {
            authoritative = value;
        }
    }
}
