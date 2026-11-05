using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    // Viewmodel del lanzacohetes. No mueve el origen del rayo.
    public sealed class RocketView : MonoBehaviour
    {
        [SerializeField] private Transform model;
        [SerializeField] private Renderer muzzleFlash;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private Vector3 restPosition = new Vector3(0.3f, -0.25f, 0.5f);
        [SerializeField] private Vector3 restEuler = new Vector3(2f, -5f, 1f);
        [SerializeField] private float kickBack = 0.12f;
        [SerializeField] private float kickPitch = 4f;
        [SerializeField] private float recover = 12f;
        [SerializeField] private Vector3 reloadOffset = new Vector3(0.05f, -0.15f, -0.1f);
        [SerializeField] private Vector3 reloadEuler = new Vector3(20f, 10f, 12f);
        [SerializeField] private float flashSeconds = 0.08f;

        Vector3 kick;
        float kickAngle;
        float flashUntil;

        void OnEnable()
        {
            if (feedback != null)
            {
                feedback.Recoiled += OnRecoil;
            }
        }

        void OnDisable()
        {
            if (feedback != null)
            {
                feedback.Recoiled -= OnRecoil;
            }
        }

        void LateUpdate()
        {
            ApplyVisual(Time.deltaTime);
        }

        public void ApplyVisual(float deltaTime)
        {
            float blend = 1f - Mathf.Exp(-recover * Mathf.Max(0f, deltaTime));
            kick = Vector3.Lerp(kick, Vector3.zero, blend);
            kickAngle = Mathf.Lerp(kickAngle, 0f, blend);
            bool reloading = weapons != null && weapons.IsReloading;
            float draw = DrawBlend();
            if (model != null)
            {
                Vector3 lowered = new Vector3(0.08f, -0.3f, -0.15f) * draw;
                model.localPosition = restPosition + kick + lowered + (reloading ? reloadOffset : Vector3.zero);
                Vector3 euler = restEuler + new Vector3(-kickAngle, 0f, 0f);
                euler += new Vector3(45f, 18f, -10f) * draw;
                if (reloading)
                {
                    euler += reloadEuler;
                }

                model.localRotation = Quaternion.Euler(euler);
            }

            if (muzzleFlash != null && muzzleFlash.enabled && Time.time >= flashUntil)
            {
                muzzleFlash.enabled = false;
            }
        }

        public void Bind(Transform viewModel, Renderer flash, PlayerImpactFeedback impact, WeaponController weaponController)
        {
            model = viewModel;
            muzzleFlash = flash;
            feedback = impact;
            weapons = weaponController;
        }

        public void PlayShotPose()
        {
            kick = new Vector3(0f, 0.018f, -kickBack);
            kickAngle = kickPitch;
            flashUntil = Time.time + flashSeconds;
            if (muzzleFlash != null)
            {
                muzzleFlash.enabled = true;
            }
        }

        void OnRecoil(float pitch)
        {
            // Solo aplicar si el arma activa es el lanzacohetes
            if (weapons != null && weapons.ActiveDefinition != null && weapons.ActiveDefinition.WeaponId == "rocket_launcher")
            {
                PlayShotPose();
            }
        }

        float DrawBlend()
        {
            if (weapons == null || weapons.IsDrawing == false)
            {
                return 0f;
            }

            WeaponDefinition weapon = weapons.ActiveDefinition;
            float duration = weapon != null ? weapon.DrawDuration : 0f;
            if (duration <= 0.0001f)
            {
                return 1f;
            }

            return Mathf.Clamp01(weapons.DrawRemaining / duration);
        }
    }
}
