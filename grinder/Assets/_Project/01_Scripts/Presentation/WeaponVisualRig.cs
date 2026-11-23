using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Movement;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class WeaponVisualRig : MonoBehaviour
    {
        [SerializeField] private Transform shotgun;
        [SerializeField] private Transform rocket;
        [SerializeField] private Transform melee;
        [SerializeField] private Transform muzzleSocket;
        WeaponController weapons;
        PlayerMelee strikes;
        PlayerMovement movement;
        PlayerInputReader input;
        HealthComponent health;
        LocalGameSession session;
        Vector3 sway;
        public Transform MuzzleSocket => muzzleSocket;
        void Start()
        {
            weapons = GetComponentInParent<WeaponController>(); strikes = GetComponentInParent<PlayerMelee>();
            movement = GetComponentInParent<PlayerMovement>(); input = GetComponentInParent<PlayerInputReader>();
            health = GetComponentInParent<HealthComponent>(); session = FindFirstObjectByType<LocalGameSession>();
        }
        void LateUpdate()
        {
            if (weapons == null) return;
            string id = weapons.ActiveDefinition != null ? weapons.ActiveDefinition.WeaponId : "melee";
            bool meleeActive = id == "melee" || (strikes != null && strikes.IsSwinging);
            bool alive = health == null || health.IsAlive;
            if (shotgun != null) shotgun.gameObject.SetActive(alive && !meleeActive && id == "shotgun");
            if (rocket != null) rocket.gameObject.SetActive(alive && !meleeActive && id == "rocket_launcher");
            if (melee != null)
            {
                melee.gameObject.SetActive(alive && meleeActive);
                float t = strikes != null ? strikes.SwingProgress : 0f;
                float slash = t < .23f ? -t / .23f : t < .5f ? Mathf.Lerp(-1, 1, (t - .23f) / .27f) : Mathf.Lerp(1, 0, (t - .5f) / .5f);
                melee.localPosition = new Vector3(.28f - slash * .16f, -.28f + Mathf.Sin(t * Mathf.PI) * .12f, .48f + Mathf.Max(0, slash) * .18f);
                melee.localRotation = Quaternion.Euler(12 - slash * 55, -10 - slash * 30, -15 - slash * 55);
            }
            float motion = session != null && session.ReduceMotion ? 0f : 1f;
            Vector2 delta = input != null ? input.LookDelta : Vector2.zero;
            sway = Vector3.Lerp(sway, new Vector3(-delta.x, -delta.y, 0) * .0005f * motion, 1 - Mathf.Exp(-12 * Time.deltaTime));
            float speed = movement != null ? new Vector2(movement.Velocity.x, movement.Velocity.z).magnitude : 0;
            float bob = Mathf.Min(speed, 12) * .0009f * motion;
            transform.localPosition = sway + new Vector3(Mathf.Sin(Time.time * 9) * bob, Mathf.Cos(Time.time * 18) * bob + Mathf.Sin(Time.time * 2) * .002f * motion, 0);
        }
    }
}
