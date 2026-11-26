using Meatgrinder.Combat;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class CombatAudio : MonoBehaviour
    {
        [SerializeField] private PresentationTheme theme;
        AudioSource source;
        AudioSource deathSource;
        bool deathPlayed;
        HealthComponent health;
        PlayerImpactFeedback feedback;
        PlayerMelee melee;
        WeaponController weapons;
        PlayerLife life;
        bool reloading;
        void Awake()
        {
            source = gameObject.AddComponent<AudioSource>(); source.playOnAwake = false;
            deathSource = gameObject.AddComponent<AudioSource>(); deathSource.playOnAwake = false;
            health = GetComponent<HealthComponent>(); feedback = GetComponent<PlayerImpactFeedback>();
            melee = GetComponent<PlayerMelee>(); weapons = GetComponent<WeaponController>(); life = GetComponent<PlayerLife>();
        }
        void OnEnable()
        {
            if (health != null) { health.Damaged += Damaged; health.Died += Died; }
            if (feedback != null) { feedback.Recoiled += Shot; feedback.Presented += Hit; }
            if (melee != null) melee.Swung += Swing;
            if (weapons != null) weapons.StateChanged += Changed;
            if (life != null) life.Respawned += Respawn;
        }
        void OnDisable()
        {
            if (health != null) { health.Damaged -= Damaged; health.Died -= Died; }
            if (feedback != null) { feedback.Recoiled -= Shot; feedback.Presented -= Hit; }
            if (melee != null) melee.Swung -= Swing;
            if (weapons != null) weapons.StateChanged -= Changed;
            if (life != null) life.Respawned -= Respawn;
        }
        void Play(int index, float volume = .7f) { if (theme != null && theme.Sound(index) != null) source.PlayOneShot(theme.Sound(index), volume); }
        void Shot(float value)
        {
            var clip = weapons != null ? weapons.ActiveDefinition?.ShotSound : null;
            if (clip != null) source.PlayOneShot(clip, .85f);
        }
        void Swing() => Play(3);
        void Damaged(float value) { if (!source.isPlaying) Play(4, .45f); }
        void Died()
        {
            if (deathPlayed) return;
            deathPlayed = true;
            if (theme == null || theme.DeathSound == null) return;
            deathSource.clip = theme.DeathSound;
            deathSource.Play();
        }
        void Respawn() { deathPlayed = false; Play(6); }
        void Hit(bool target) { if (target && !source.isPlaying) Play(7, .25f); }
        void Changed() { bool next = weapons.IsReloading; if (next && !reloading) Play(2); reloading = next; }
        public void Pickup(PickupDefinition definition, AudioClip clip)
        {
            if (clip != null) source.PlayOneShot(clip, .6f); else Play(6);
            GetComponent<CombatHud>()?.ShowPickup(definition.DisplayName);
        }
    }
}
