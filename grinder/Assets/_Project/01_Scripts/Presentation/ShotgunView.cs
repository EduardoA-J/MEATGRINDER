using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    // Local camera child. It never moves the hitscan origin. Do not put a NetworkObject on this view.
    public sealed class ShotgunView : MonoBehaviour
    {
        [SerializeField] private Transform model;
        [SerializeField] private Renderer muzzleFlash;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private Vector3 restPosition = new Vector3(0.24f, -0.22f, 0.46f);
        [SerializeField] private Vector3 restEuler = new Vector3(4f, -8f, 3f);
        [SerializeField] private float kickBack = 0.07f;
        [SerializeField] private float kickPitch = 9f;
        [SerializeField] private float recover = 16f;
        [SerializeField] private Vector3 reloadOffset = new Vector3(0.04f, -0.1f, -0.06f);
        [SerializeField] private Vector3 reloadEuler = new Vector3(26f, 14f, 16f);
        [SerializeField] private float flashSeconds = 0.05f;

        Vector3 kick;
        float kickAngle;
        float flashUntil;
        float pump;
        Transform pumpTransform;
        Vector3 pumpRest;
        bool pumpReady;

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
            pump = Mathf.Lerp(pump, 0f, blend);
            bool reloading = weapons != null && weapons.IsReloading;
            float draw = DrawBlend();
            if (model != null)
            {
                Vector3 lowered = new Vector3(0.06f, -0.26f, -0.1f) * draw;
                model.localPosition = restPosition + kick + lowered + (reloading ? reloadOffset : Vector3.zero);
                Vector3 euler = restEuler + new Vector3(-kickAngle, 0f, 0f);
                euler += new Vector3(42f, 16f, -8f) * draw;
                if (reloading)
                {
                    euler += reloadEuler;
                }

                model.localRotation = Quaternion.Euler(euler);
                ApplyPump();
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
            pumpReady = false;
        }

        public void PlayShotPose()
        {
            kick = new Vector3(0f, 0.012f, -kickBack);
            kickAngle = kickPitch;
            pump = 1f;
            flashUntil = Time.time + flashSeconds;
            if (muzzleFlash != null)
            {
                muzzleFlash.enabled = true;
            }
        }

        void OnRecoil(float pitch)
        {
            PlayShotPose();
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

        void ApplyPump()
        {
            if (pumpReady == false)
            {
                pumpTransform = model.Find("Pump");
                if (pumpTransform != null)
                {
                    pumpRest = pumpTransform.localPosition;
                }

                pumpReady = true;
            }

            if (pumpTransform == null)
            {
                return;
            }

            pumpTransform.localPosition = pumpRest + new Vector3(0f, 0f, -0.06f * pump);
        }
    }
}
