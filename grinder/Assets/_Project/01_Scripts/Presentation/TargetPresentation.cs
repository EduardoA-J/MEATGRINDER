using Meatgrinder.Combat;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class TargetPresentation : MonoBehaviour
    {
        [SerializeField] private Transform modelSocket;
        HealthComponent health;
        float deathProgress;
        void Awake() => health = GetComponent<HealthComponent>();
        void OnEnable() { if (health != null) health.Died += Died; }
        void OnDisable() { if (health != null) health.Died -= Died; }
        void Died() { foreach (var body in GetComponents<Collider>()) body.enabled = false; }
        void Update()
        {
            if (modelSocket == null || health == null || health.IsAlive) return;
            deathProgress = Mathf.MoveTowards(deathProgress, 1, Time.deltaTime * 2.5f);
            modelSocket.localRotation = Quaternion.Euler(0, 0, deathProgress * 85);
            modelSocket.localPosition = Vector3.down * (deathProgress * .2f);
        }
    }
}
