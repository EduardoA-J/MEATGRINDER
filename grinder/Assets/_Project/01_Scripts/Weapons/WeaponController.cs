using UnityEngine;

namespace Meatgrinder.Weapons
{
    public sealed class WeaponController : MonoBehaviour
    {
        struct Slot
        {
            public int Loaded;
            public int Reserve;
            public float Cooldown;
            public float ReloadRemaining;
        }

        [SerializeField] private WeaponDefinition[] loadout = System.Array.Empty<WeaponDefinition>();
        [SerializeField] private int activeIndex;

        Slot[] slots = System.Array.Empty<Slot>();
        bool equipped = true;
        bool ready;
        bool loggedShot;
        float drawRemaining;
        ulong nextConfirmation = 1;

        public event System.Action StateChanged;

        public bool IsEquipped => equipped;
        public bool IsReloading => ready && slots.Length > 0 && slots[activeIndex].ReloadRemaining > 0f;
        public bool IsDrawing => drawRemaining > 0f;
        public float DrawRemaining => drawRemaining;
        public float ReloadRemaining => ready && slots.Length > 0 ? slots[activeIndex].ReloadRemaining : 0f;
        public int ActiveIndex => activeIndex;
        public int Loaded => ready && slots.Length > 0 ? slots[activeIndex].Loaded : 0;
        public int Reserve => ready && slots.Length > 0 ? slots[activeIndex].Reserve : 0;
        public WeaponDefinition ActiveDefinition => ready && slots.Length > 0 ? loadout[activeIndex] : null;
        public int SlotCount => ready ? slots.Length : 0;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (ready)
            {
                return;
            }

            int count = loadout != null ? loadout.Length : 0;
            slots = new Slot[count];
            for (int i = 0; i < count; i++)
            {
                WeaponDefinition weapon = loadout[i];
                if (weapon == null)
                {
                    continue;
                }

                slots[i].Loaded = Mathf.Clamp(weapon.StartingLoaded, 0, weapon.MagazineSize);
                slots[i].Reserve = Mathf.Max(0, weapon.StartingReserve);
            }

            if (count > 0)
            {
                activeIndex = Mathf.Clamp(activeIndex, 0, count - 1);
            }

            ready = true;
            BeginDraw();
            StateChanged?.Invoke();
        }

        public void Tick(float deltaTime)
        {
            Initialize();
            if (deltaTime <= 0f)
            {
                return;
            }

            bool finishedDraw = false;
            if (drawRemaining > 0f)
            {
                drawRemaining = Mathf.Max(0f, drawRemaining - deltaTime);
                finishedDraw = drawRemaining <= 0f;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                Slot slot = slots[i];
                if (slot.Cooldown > 0f)
                {
                    slot.Cooldown = Mathf.Max(0f, slot.Cooldown - deltaTime);
                }

                bool finishedReload = false;
                if (slot.ReloadRemaining > 0f)
                {
                    slot.ReloadRemaining -= deltaTime;
                    if (slot.ReloadRemaining <= 0f)
                    {
                        slot.ReloadRemaining = 0f;
                        FinishReload(i, ref slot);
                        finishedReload = true;
                    }
                }

                slots[i] = slot;
                if (finishedReload && i == activeIndex)
                {
                    StateChanged?.Invoke();
                }
            }

            if (finishedDraw)
            {
                StateChanged?.Invoke();
            }
        }

        public bool CanFire()
        {
            return Inspect() == ShotRejectReason.None;
        }

        public void Equip()
        {
            equipped = true;
        }

        public void Unequip()
        {
            equipped = false;
        }

        public bool Select(int index)
        {
            Initialize();
            if (equipped == false || index < 0 || index >= slots.Length || index == activeIndex)
            {
                return false;
            }

            activeIndex = index;
            BeginDraw();
            StateChanged?.Invoke();
            return true;
        }

        public bool TryBeginReload()
        {
            Initialize();
            if (equipped == false || slots.Length == 0 || drawRemaining > 0f)
            {
                return false;
            }

            WeaponDefinition weapon = loadout[activeIndex];
            Slot slot = slots[activeIndex];
            if (weapon == null || slot.ReloadRemaining > 0f || slot.Loaded >= weapon.MagazineSize || slot.Reserve <= 0)
            {
                return false;
            }

            slot.ReloadRemaining = weapon.ReloadDuration;
            if (slot.ReloadRemaining <= 0f)
            {
                FinishReload(activeIndex, ref slot);
            }

            slots[activeIndex] = slot;
            StateChanged?.Invoke();
            return true;
        }

        public ShotTicket TryBeginShot()
        {
            ShotRejectReason reason = Inspect();
            if (reason != ShotRejectReason.None)
            {
                if (reason == ShotRejectReason.Empty)
                {
                    TryBeginReload();
                }

                return ShotTicket.Rejected(reason);
            }

            WeaponDefinition weapon = loadout[activeIndex];
            Slot slot = slots[activeIndex];
            slot.Loaded -= weapon.AmmoPerShot;
            slot.Cooldown = weapon.FireInterval;
            slots[activeIndex] = slot;
            ulong confirmation = nextConfirmation;
            nextConfirmation += 32;
            if (loggedShot == false)
            {
                Debug.Log("[Combat] Shot " + weapon.WeaponId + " loaded " + slot.Loaded);
                loggedShot = true;
            }

            StateChanged?.Invoke();
            return ShotTicket.Ok(weapon, confirmation);
        }

        public int TryGrant(WeaponDefinition weapon, int bonusAmmo)
        {
            Initialize();
            if (weapon == null)
            {
                return -1;
            }

            int grant = bonusAmmo > 0 ? bonusAmmo : Mathf.Max(0, weapon.StartingReserve);
            int existing = IndexOf(weapon);
            if (existing >= 0)
            {
                Slot slot = slots[existing];
                slot.Reserve += grant;
                slots[existing] = slot;
                StateChanged?.Invoke();
                return existing;
            }

            int count = loadout.Length;
            WeaponDefinition[] nextLoadout = new WeaponDefinition[count + 1];
            Slot[] nextSlots = new Slot[count + 1];
            for (int i = 0; i < count; i++)
            {
                nextLoadout[i] = loadout[i];
                nextSlots[i] = slots[i];
            }

            nextLoadout[count] = weapon;
            nextSlots[count].Loaded = Mathf.Clamp(weapon.StartingLoaded, 0, weapon.MagazineSize);
            nextSlots[count].Reserve = grant;
            loadout = nextLoadout;
            slots = nextSlots;
            StateChanged?.Invoke();
            return count;
        }

        internal void LoadForTests(WeaponDefinition[] weapons)
        {
            loadout = weapons;
            activeIndex = 0;
            equipped = true;
            ready = false;
            nextConfirmation = 1;
            Initialize();
        }

        ShotRejectReason Inspect()
        {
            Initialize();
            if (equipped == false || slots.Length == 0 || loadout[activeIndex] == null)
            {
                return ShotRejectReason.Unequipped;
            }

            if (drawRemaining > 0f)
            {
                return ShotRejectReason.Drawing;
            }

            Slot slot = slots[activeIndex];
            if (slot.ReloadRemaining > 0f)
            {
                return ShotRejectReason.Reloading;
            }

            if (slot.Cooldown > 0f)
            {
                return ShotRejectReason.CoolingDown;
            }

            if (slot.Loaded < loadout[activeIndex].AmmoPerShot)
            {
                return ShotRejectReason.Empty;
            }

            return ShotRejectReason.None;
        }

        void FinishReload(int index, ref Slot slot)
        {
            WeaponDefinition weapon = loadout[index];
            if (weapon == null)
            {
                return;
            }

            int room = weapon.MagazineSize - slot.Loaded;
            int moved = Mathf.Min(Mathf.Max(0, room), slot.Reserve);
            slot.Loaded += moved;
            slot.Reserve -= moved;
        }

        int IndexOf(WeaponDefinition weapon)
        {
            for (int i = 0; i < loadout.Length; i++)
            {
                WeaponDefinition candidate = loadout[i];
                if (candidate == null)
                {
                    continue;
                }

                if (candidate == weapon || candidate.WeaponId == weapon.WeaponId)
                {
                    return i;
                }
            }

            return -1;
        }

        void BeginDraw()
        {
            WeaponDefinition weapon = slots.Length > 0 ? loadout[activeIndex] : null;
            drawRemaining = weapon != null ? weapon.DrawDuration : 0f;
        }
    }
}
