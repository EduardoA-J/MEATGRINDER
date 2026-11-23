using Meatgrinder.Pickups;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class PickupPresentation : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        [SerializeField] private Renderer halo;
        [SerializeField] private AudioClip sound;
        PickupController pickup;
        Vector3 rest;
        Vector3 haloScale;
        void Awake()
        {
            pickup = GetComponent<PickupController>();
            if (visual != null) rest = visual.localPosition;
            if (halo != null) haloScale = halo.transform.localScale;
        }
        void OnEnable() { if (pickup != null) pickup.PickedUp += Collected; }
        void OnDisable() { if (pickup != null) pickup.PickedUp -= Collected; }
        void Update()
        {
            if (visual != null) visual.localPosition = rest + Vector3.up * (Mathf.Sin(Time.time * 2.4f) * .12f);
            if (halo != null)
            {
                halo.enabled = pickup != null && pickup.IsAvailable;
                halo.transform.localScale = haloScale * (1 + Mathf.Sin(Time.time * 3) * .06f);
            }
        }
        void Collected(PickupDefinition definition, GameObject collector)
        {
            var audio = collector.GetComponent<CombatAudio>();
            audio?.Pickup(definition, sound);
        }
    }
}
