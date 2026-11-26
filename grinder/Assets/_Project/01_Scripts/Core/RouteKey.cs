using Meatgrinder.Combat;
using UnityEngine;
namespace Meatgrinder.Core
{
    public sealed class RouteKey : MonoBehaviour
    {
        [SerializeField] private KeyDefinition definition;
        [SerializeField] private MapRouteState state;
        [SerializeField] private Transform visual;
        public KeyDefinition Definition => definition;
        public bool Replica { get; set; }
        void Update() { if (visual != null) visual.Rotate(Vector3.up, 60 * Time.deltaTime); }
        void OnTriggerEnter(Collider other)
        {
            var health = other.GetComponentInParent<HealthComponent>();
            if (Replica || health == null || !health.IsAlive || !health.AllowWeapons) return;
            var authority = health.GetComponent<LocalCombatAuthority>();
            if (authority != null && !authority.IsAuthoritative) return;
            if (state != null && state.Unlock(definition)) gameObject.SetActive(false);
        }
    }
}
