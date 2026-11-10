using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Pickups
{
    [CreateAssetMenu(fileName = "PickupDefinition", menuName = "MEATGRINDER/Pickup Definition")]
    public sealed class PickupDefinition : ScriptableObject
    {
        [SerializeField] private string pickupId = "pickup";
        [SerializeField] private string displayName = "Pickup";
        [SerializeField] private PickupType type = PickupType.Health;
        [SerializeField] private float amount = 25f; // PENDIENTE - Salud o armadura a dar
        [SerializeField] private float respawnTime = 30f; // PENDIENTE - D-11
        [SerializeField] private bool canPickupWhenFull = false;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private GameObject pickupVfx;
        
        // Para pickups de power-up
        [SerializeField] private PowerUpType powerUpType;
        [SerializeField] private float duration = 30f; // PENDIENTE - duración del efecto
        [SerializeField] private float effectValue = 2f; // PENDIENTE - multiplicador o valor del efecto
        
        // Para pickups de armas
        [SerializeField] private WeaponDefinition weapon;
        [SerializeField] private int ammoAmount = 0; // Si es 0, da munición estándar del arma

        public string PickupId => pickupId;
        public string DisplayName => displayName;
        public PickupType Type => type;
        public float Amount => amount;
        public float RespawnTime => respawnTime > 0f ? respawnTime : 0f;
        public bool CanPickupWhenFull => canPickupWhenFull;
        public GameObject VisualPrefab => visualPrefab;
        public AudioClip PickupSound => pickupSound;
        public GameObject PickupVfx => pickupVfx;
        public PowerUpType PowerUpType => powerUpType;
        public float Duration => duration > 0f ? duration : 0f;
        public float EffectValue => effectValue;
        public WeaponDefinition Weapon => weapon;
        public int AmmoAmount => ammoAmount;

        internal void Configure(
            PickupType pickupType,
            float pickupAmount,
            PowerUpType effectType,
            float effectDuration,
            float value,
            WeaponDefinition grantedWeapon,
            int ammo)
        {
            type = pickupType;
            amount = pickupAmount;
            powerUpType = effectType;
            duration = effectDuration;
            effectValue = value;
            weapon = grantedWeapon;
            ammoAmount = ammo;
        }
    }
}
