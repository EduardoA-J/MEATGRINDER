using Meatgrinder.Combat;
using UnityEngine;
namespace Meatgrinder.Core
{
    public sealed class KeyGate : MonoBehaviour
    {
        [SerializeField] private MapRouteState state;
        [SerializeField] private KeyDefinition requiredKey;
        [SerializeField] private Transform door;
        [SerializeField] private bool advancesMap;
        [SerializeField] private MapDefinition nextMap;
        [SerializeField, Min(.01f)] private float openingDuration = 1.2f; // PENDIENTE: presentation timing.
        Vector3 closed;
        float opening;
        bool transitioned;
        public bool IsOpen => opening >= 1;
        public bool AdvancesMap => advancesMap;
        public event System.Action<HealthComponent> OnlineExitReached;
        public bool Replicated { get; set; }
        public bool Unlocked => state != null && state.Has(requiredKey);
        public bool RemoteUnlocked { get; set; }
        void Awake() { if (door != null) closed = door.localPosition; }
        void Update()
        {
            if (!(Replicated ? RemoteUnlocked : Unlocked)) return;
            opening = Mathf.MoveTowards(opening, 1, Time.deltaTime / openingDuration);
            if (door != null) door.localPosition = closed + Vector3.up * (Mathf.SmoothStep(0, 1, opening) * 4.5f);
        }
        void OnTriggerEnter(Collider other) => TryAdvance(other);
        void OnTriggerStay(Collider other) => TryAdvance(other);
        void TryAdvance(Collider other)
        {
            if (!advancesMap || !IsOpen || transitioned) return;
            var health = other.GetComponentInParent<HealthComponent>();
            if (health == null || !health.IsAlive) return;
            var authority = health.GetComponent<LocalCombatAuthority>();
            if (authority != null && !authority.IsAuthoritative) return;
            var session = FindFirstObjectByType<LocalGameSession>();
            if (session != null && session.Online)
            {
                OnlineExitReached?.Invoke(health);
                return;
            }
            if (session == null || session.Phase != LocalSessionPhase.Playing) return;
            transitioned = true;
            if (nextMap != null) { if (!session.StartMap(nextMap)) transitioned = false; }
            else session.Finish();
        }
    }
}
