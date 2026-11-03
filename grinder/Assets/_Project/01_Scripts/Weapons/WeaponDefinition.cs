using UnityEngine;

namespace Meatgrinder.Weapons
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "MEATGRINDER/Weapon Definition")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [SerializeField] private string weaponId = "weapon";
        [SerializeField] private string displayName = "Weapon";
        [SerializeField] private WeaponDelivery delivery = WeaponDelivery.Hitscan;
        [SerializeField] private float damage = 10f; // PENDIENTE
        [SerializeField] private int pelletCount = 1; // PENDIENTE
        [SerializeField] private float spreadDegrees; // PENDIENTE
        [SerializeField] private float maxRange = 40f; // PENDIENTE
        [SerializeField] private float falloffStart = 8f; // PENDIENTE
        [SerializeField] private float falloffEnd = 28f; // PENDIENTE
        [SerializeField] private float minimumDamageScale = 0.3f; // PENDIENTE
        [SerializeField] private float fireInterval = 0.6f; // PENDIENTE
        [SerializeField] private int magazineSize = 6; // PENDIENTE
        [SerializeField] private int startingLoaded = 6; // PENDIENTE
        [SerializeField] private int startingReserve = 24; // PENDIENTE
        [SerializeField] private int ammoPerShot = 1; // PENDIENTE
        [SerializeField] private float reloadDuration = 1.4f; // PENDIENTE
        [SerializeField] private float projectileSpeed = 24f; // PENDIENTE
        [SerializeField] private float projectileRadius = 0.18f; // PENDIENTE
        [SerializeField] private float projectileLifetime = 4f; // PENDIENTE
        [SerializeField] private float blastRadius = 4f; // PENDIENTE
        [SerializeField] private float blastDamage = 80f; // PENDIENTE
        [SerializeField] private float blastEdgeScale = 0.2f; // PENDIENTE
        [SerializeField] private float recoilPitch; // PENDIENTE
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private AudioClip shotSound;
        [SerializeField] private GameObject impactVfx;

        public string WeaponId => weaponId;
        public string DisplayName => displayName;
        public WeaponDelivery Delivery => delivery;
        public float Damage => damage;
        public int PelletCount => pelletCount > 0 ? pelletCount : 1;
        public float SpreadDegrees => spreadDegrees;
        public float MaxRange => maxRange;
        public float FireInterval => fireInterval < 0f ? 0f : fireInterval;
        public int MagazineSize => magazineSize > 0 ? magazineSize : 1;
        public int StartingLoaded => startingLoaded;
        public int StartingReserve => startingReserve;
        public int AmmoPerShot => ammoPerShot > 0 ? ammoPerShot : 1;
        public float ReloadDuration => reloadDuration < 0f ? 0f : reloadDuration;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileRadius => projectileRadius > 0f ? projectileRadius : 0.1f;
        public float ProjectileLifetime => projectileLifetime;
        public float BlastRadius => blastRadius;
        public float BlastDamage => blastDamage;
        public float BlastEdgeScale => blastEdgeScale;
        public float RecoilPitch => recoilPitch < 0f ? 0f : recoilPitch;
        public GameObject ProjectilePrefab => projectilePrefab;
        public AudioClip ShotSound => shotSound;
        public GameObject ImpactVfx => impactVfx;

        public float DamageAt(float distance)
        {
            if (falloffEnd <= falloffStart || distance <= falloffStart)
            {
                return damage;
            }

            if (distance >= falloffEnd)
            {
                return damage * minimumDamageScale;
            }

            float blend = Mathf.InverseLerp(falloffStart, falloffEnd, distance);
            return Mathf.Lerp(damage, damage * minimumDamageScale, blend);
        }

        public float BlastAmount(float distance)
        {
            if (blastRadius <= 0.01f)
            {
                return blastDamage;
            }

            float blend = Mathf.Clamp01(distance / blastRadius);
            return Mathf.Lerp(blastDamage, blastDamage * blastEdgeScale, blend);
        }

        internal void SetFireInterval(float value)
        {
            fireInterval = value;
        }

        internal void SetAmmo(int loaded, int reserve, int magazine, int perShot)
        {
            startingLoaded = loaded;
            startingReserve = reserve;
            magazineSize = magazine;
            ammoPerShot = perShot;
        }

        internal void SetSpread(float degrees, int pellets)
        {
            spreadDegrees = degrees;
            pelletCount = pellets;
        }

        internal void SetFalloff(float start, float end, float scale, float perPellet)
        {
            falloffStart = start;
            falloffEnd = end;
            minimumDamageScale = scale;
            damage = perPellet;
        }

        internal void SetBlast(float radius, float blast, float edge)
        {
            blastRadius = radius;
            blastDamage = blast;
            blastEdgeScale = edge;
        }

        internal void SetDelivery(WeaponDelivery value)
        {
            delivery = value;
        }

        internal void SetSpeed(float value)
        {
            projectileSpeed = value;
        }

        internal void SetReload(float seconds)
        {
            reloadDuration = seconds;
        }

        internal void SetRecoil(float degrees)
        {
            recoilPitch = degrees;
        }

        internal void SetMaxRange(float value)
        {
            maxRange = value;
        }

        internal void SetIdentity(string id, string name)
        {
            weaponId = id;
            displayName = name;
        }
    }
}
