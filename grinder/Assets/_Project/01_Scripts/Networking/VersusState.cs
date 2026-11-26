using Fusion;
using Meatgrinder.Combat;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Networking
{
    public enum VersusMode { Campaign, Continuous, Private }
    public enum VersusPhase { Lobby, Loading, Playing, Results }
    public enum VersusButton { Jump, Sprint, Crouch, Attack, Reload, Melee, Shotgun, Rocket }

    public struct VersusInput : INetworkInput
    {
        public Vector2 Move;
        public Vector2 Aim;
        public NetworkButtons Buttons;
    }

    public sealed class VersusState : NetworkBehaviour
    {
        [Networked] public NetworkBool IsMatch { get; set; }
        [Networked] public NetworkBool IsProjectile { get; set; }
        [Networked] public VersusMode Mode { get; set; }
        [Networked] public VersusPhase Phase { get; set; }
        [Networked] public int Map { get; set; }
        [Networked] public int Half { get; set; }
        [Networked] public int ScoreA { get; set; }
        [Networked] public int ScoreB { get; set; }
        [Networked] public int ForfeitWinner { get; set; }
        [Networked] public TickTimer Deadline { get; set; }
        [Networked, Capacity(128)] public NetworkArray<NetworkBool> Pickups => default;
        [Networked, Capacity(32)] public NetworkArray<NetworkBool> Gates => default;
        [Networked, Capacity(32)] public NetworkArray<NetworkBool> Keys => default;
        [Networked] public int Team { get; set; }
        [Networked] public NetworkBool Monster { get; set; }
        [Networked] public MonsterKind Archetype { get; set; }
        [Networked, Capacity(8)] public NetworkDictionary<PlayerRef, MonsterKind> MonsterChoices => default;
        [Networked] public float Swing { get; set; }
        [Networked] public int Kills { get; set; }
        [Networked] public int Deaths { get; set; }
        [Networked] public Vector3 Position { get; set; }
        [Networked] public Vector2 Aim { get; set; }
        [Networked] public float Health { get; set; }
        [Networked] public float Armor { get; set; }
        [Networked] public float EyeHeight { get; set; }
        [Networked] public int Loaded { get; set; }
        [Networked] public int Reserve { get; set; }

        public GameObject Body { get; private set; }
        public HealthComponent Vitality { get; private set; }
        PlayerInputReader input;
        PlayerLook look;
        WeaponController weapons;
        PlayerMelee melee;
        GameObject silhouette;
        MonsterKind renderedKind;
        RocketProjectile projectile;
        bool placed;

        public override void Spawned()
        {
            if (IsMatch)
            {
                Runner.MakeDontDestroyOnLoad(gameObject);
                return;
            }
            if (IsProjectile && HasStateAuthority) return;
            if (HasInputAuthority) return;
            UpdateVisual();
        }

        void UpdateVisual()
        {
            if (silhouette != null && renderedKind == Archetype) return;
            if (silhouette != null) { silhouette.SetActive(false); Destroy(silhouette); }
            renderedKind = Archetype;
            var definition = MonsterDefinition.Get(renderedKind);
            if (definition != null && definition.VisualPrefab != null)
            {
                silhouette = Instantiate(definition.VisualPrefab, transform, false);
                // Replacement models are presentation only.
                foreach (var hitbox in silhouette.GetComponentsInChildren<Collider>(true)) hitbox.enabled = false;
                return;
            }
            silhouette = GameObject.CreatePrimitive(IsProjectile ? PrimitiveType.Sphere : PrimitiveType.Capsule);
            silhouette.transform.SetParent(transform, false);
            silhouette.transform.localPosition = IsProjectile ? Vector3.zero : Vector3.up * .9f;
            silhouette.transform.localScale = IsProjectile ? Vector3.one * .22f : new Vector3(.72f, .9f, .72f);
            var collider = silhouette.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
            var renderer = silhouette.GetComponent<Renderer>();
            var color = new MaterialPropertyBlock();
            color.SetColor("_BaseColor", IsProjectile ? new Color(1f, .4f, .12f) :
                Monster ? new Color(.75f, .14f, .10f) : new Color(.28f, .55f, .75f));
            renderer.SetPropertyBlock(color);
        }

        public void BindBody(GameObject body)
        {
            Body = body;
            input = body.GetComponent<PlayerInputReader>();
            look = body.GetComponent<PlayerLook>();
            Vitality = body.GetComponent<HealthComponent>();
            weapons = body.GetComponent<WeaponController>();
            melee = body.GetComponent<PlayerMelee>();
            Vitality.Died += CountDeath;
            Position = body.transform.position;
            Aim = new Vector2(look.Yaw, look.Pitch);
            Health = Vitality.Current;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPC_ChooseMonster(MonsterKind choice, RpcInfo info = default)
        {
            if (!IsMatch || !info.Source.IsRealPlayer || MonsterDefinition.Get(choice) == null) return;
            foreach (var player in Runner.ActivePlayers)
            {
                if (player != info.Source) continue;
                MonsterChoices.Set(player, choice);
                break;
            }
        }

        void CountDeath() => Deaths++;

        public void BindProjectile(RocketProjectile value) => projectile = value;

        public override void FixedUpdateNetwork()
        {
            if (IsProjectile)
            {
                if (!HasStateAuthority) return;
                if (projectile == null) Runner.Despawn(Object);
                else Position = projectile.transform.position;
                return;
            }
            if (IsMatch || !HasStateAuthority || Body == null) return;
            if (GetInput<VersusInput>(out var command) && Vitality.IsAlive &&
                float.IsFinite(command.Aim.x) && float.IsFinite(command.Aim.y) &&
                float.IsFinite(command.Move.x) && float.IsFinite(command.Move.y))
            {
                var b = command.Buttons;
                look.SetExternalAim(Mathf.Repeat(command.Aim.x, 360), command.Aim.y);
                input.SetExternalInput(Vector2.ClampMagnitude(command.Move, 1), b.IsSet(VersusButton.Jump),
                    b.IsSet(VersusButton.Sprint), b.IsSet(VersusButton.Crouch), !Monster && b.IsSet(VersusButton.Attack),
                    !Monster && b.IsSet(VersusButton.Reload), b.IsSet(VersusButton.Melee) || (Monster && b.IsSet(VersusButton.Attack)),
                    Monster ? 0 : b.IsSet(VersusButton.Shotgun) ? 1 : b.IsSet(VersusButton.Rocket) ? 2 : 0);
            }
            else input.SetExternalInput(Vector2.zero, false, false, false, false, false, false, 0);

            Position = Body.transform.position;
            Aim = new Vector2(look.Yaw, look.Pitch);
            EyeHeight = look.View.transform.localPosition.y;
            Health = Vitality.Current;
            Armor = Vitality.Armor;
            Swing = melee.IsSwinging ? melee.SwingProgress : 0;
            Loaded = weapons.Loaded;
            Reserve = weapons.Reserve;
        }

        public override void Render()
        {
            if (IsMatch || HasInputAuthority || (IsProjectile && HasStateAuthority)) return;
            UpdateVisual();
            bool snap = !placed || (transform.position - Position).sqrMagnitude > 16;
            transform.SetPositionAndRotation(snap ? Position : Vector3.Lerp(transform.position, Position, 20 * Time.deltaTime),
                Quaternion.Euler(0, Aim.x, 0));
            placed = true;
            silhouette.SetActive(IsProjectile || Health > 0);
            if (Monster) silhouette.transform.localRotation = Quaternion.Euler(Mathf.Sin(Swing * Mathf.PI) * 14, 0, 0);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Body != null)
            {
                Body.SetActive(false);
                Destroy(Body);
            }
        }
    }
}
