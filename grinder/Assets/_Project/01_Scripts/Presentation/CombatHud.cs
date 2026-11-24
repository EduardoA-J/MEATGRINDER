using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class CombatHud : MonoBehaviour
    {
        [SerializeField] private WeaponController weapons;
        [SerializeField] private HealthComponent health;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private PowerUpManager powerUps;
        [SerializeField] private PresentationTheme theme;
        [SerializeField] private float crosshairScale = 1;
        UnityEngine.UI.Text healthText, armorText, weaponText, ammoText, statusText, deathText, pickupText;
        UnityEngine.UI.Image healthFill, armorFill, damageOverlay, fallOverlay, reloadFill;
        RectTransform crosshair, deathPanel;
        UnityEngine.UI.Image[] hitMarks;
        readonly GameObject[] cards = new GameObject[6];
        readonly UnityEngine.UI.Text[] timers = new UnityEngine.UI.Text[6];
        readonly UnityEngine.UI.Image[] powerBars = new UnityEngine.UI.Image[6];
        readonly float[] powerDurations = new float[6];
        PlayerLife life;
        PlayerVoidFall fall;
        LocalGameSession session;
        MapRouteState routes;
        Canvas canvas;
        float damageFlash, hitTime, pickupTime, refreshTime;
        public float CrosshairScale { get => crosshairScale; set { crosshairScale = Mathf.Max(.2f, value); if (crosshair != null) crosshair.localScale = Vector3.one * crosshairScale; } }
        public string HealthReadout => healthText != null ? healthText.text : "";
        public string AmmoReadout => ammoText != null ? ammoText.text : "";
        void Awake()
        {
            if (weapons == null) weapons = GetComponent<WeaponController>();
            if (health == null) health = GetComponent<HealthComponent>();
            if (feedback == null) feedback = GetComponent<PlayerImpactFeedback>();
            if (powerUps == null) powerUps = GetComponent<PowerUpManager>();
            life = GetComponent<PlayerLife>(); fall = GetComponent<PlayerVoidFall>();
            Build();
        }
        void Start()
        {
            session = FindFirstObjectByType<LocalGameSession>();
            routes = FindFirstObjectByType<MapRouteState>();
            if (routes != null) routes.Unlocked += OnKey;
            RefreshAll();
        }
        void OnDestroy() { if (routes != null) routes.Unlocked -= OnKey; }
        void OnKey(KeyDefinition key) => ShowPickup(key.DisplayName + " / puertas abiertas");
        void OnEnable()
        {
            if (weapons != null) weapons.StateChanged += RefreshWeapon;
            if (health != null) { health.Damaged += OnDamaged; health.HealthChanged += OnHealthChanged; health.ArmorChanged += OnHealthChanged; health.Died += EffectsChanged; }
            if (feedback != null) feedback.Presented += OnHit;
            if (powerUps != null) { powerUps.PowerUpActivated += Activated; powerUps.PowerUpExpired += Expired; }
            RefreshAll();
        }
        void OnDisable()
        {
            if (weapons != null) weapons.StateChanged -= RefreshWeapon;
            if (health != null) { health.Damaged -= OnDamaged; health.HealthChanged -= OnHealthChanged; health.ArmorChanged -= OnHealthChanged; health.Died -= EffectsChanged; }
            if (feedback != null) feedback.Presented -= OnHit;
            if (powerUps != null) { powerUps.PowerUpActivated -= Activated; powerUps.PowerUpExpired -= Expired; }
        }
        void Update()
        {
            if (session != null) canvas.enabled = session.Phase == LocalSessionPhase.Playing;
            damageFlash = Mathf.MoveTowards(damageFlash, 0, Time.deltaTime * 2);
            float low = health != null && health.IsAlive && health.Current <= 25 ? .08f + .025f * Mathf.Sin(Time.time * 3) : 0;
            Alpha(damageOverlay, session != null && session.ReduceFlashes ? low : Mathf.Max(low, damageFlash));
            float falling = fall != null && fall.IsFalling ? Mathf.Lerp(.12f, .92f, fall.Progress) : 0;
            Alpha(fallOverlay, falling);
            bool dead = life != null && life.IsWaitingToRespawn;
            deathPanel.gameObject.SetActive(dead); crosshair.gameObject.SetActive(!dead);
            if (dead) deathText.text = "HAS MUERTO\n<size=22>" + life.DeathCause + "</size>\n<size=26>REAPARICIÓN EN " + life.RespawnRemaining.ToString("0.0") + " s</size>";
            foreach (var mark in hitMarks) mark.enabled = Time.time < hitTime;
            pickupText.enabled = Time.time < pickupTime;
            float reload = weapons != null && weapons.IsReloading && weapons.ActiveDefinition != null ? 1 - weapons.ReloadRemaining / Mathf.Max(.01f, weapons.ActiveDefinition.ReloadDuration) : 0;
            reloadFill.rectTransform.localScale = new Vector3(reload, 1, 1);
            if (Time.unscaledTime < refreshTime) return;
            refreshTime = Time.unscaledTime + .1f;
            statusText.text = falling > 0 ? "VACÍO  /  DAÑO PROGRESIVO" : health != null && health.IsSpawnProtected ? "PROTECCIÓN DE APARICIÓN" : "";
            int visible = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                bool active = powerUps != null && powerUps.HasEffect((PowerUpType)i);
                cards[i].SetActive(active);
                if (!active) continue;
                ((RectTransform)cards[i].transform).anchoredPosition = new Vector2(-28, -100 - visible++ * 72);
                float remaining = powerUps.GetTimeRemaining((PowerUpType)i);
                timers[i].text = PresentationTheme.PowerName((PowerUpType)i) + "\n" + remaining.ToString("0.0") + " s";
                powerBars[i].rectTransform.localScale = new Vector3(Mathf.Clamp01(remaining / Mathf.Max(.01f, powerDurations[i])), 1, 1);
            }
        }
        public void RefreshAll() { RefreshHealth(); RefreshWeapon(); }
        void OnHealthChanged(float value) => RefreshHealth();
        void OnDamaged(float value) { damageFlash = .25f; }
        void EffectsChanged() { refreshTime = 0; }
        void Expired(PowerUpType type) => EffectsChanged();
        void Activated(PowerUpType type, float duration) { powerDurations[(int)type] = duration; EffectsChanged(); }
        void OnHit(bool damaged)
        {
            hitTime = Time.time + .12f;
            foreach (var mark in hitMarks) mark.color = damaged ? GameUi.Accent : new Color(.7f, .7f, .7f, .5f);
        }
        public void ShowPickup(string name) { pickupText.text = "+ " + name.ToUpperInvariant(); pickupTime = Time.time + 2; }
        void RefreshHealth()
        {
            if (healthText == null || health == null) return;
            healthText.text = Mathf.CeilToInt(health.Current) + " <size=19>/ " + health.Max.ToString("0") + "</size>";
            armorText.text = Mathf.CeilToInt(health.Armor) + " <size=19>/ " + health.MaxArmor.ToString("0") + "</size>";
            healthFill.rectTransform.localScale = new Vector3(Mathf.Clamp01(health.Current / health.Max), 1, 1);
            armorFill.rectTransform.localScale = new Vector3(health.MaxArmor > 0 ? health.Armor / health.MaxArmor : 0, 1, 1);
        }
        void RefreshWeapon()
        {
            if (ammoText == null) return;
            var weapon = weapons != null ? weapons.ActiveDefinition : null;
            weaponText.text = weapon != null ? weapon.DisplayName.ToUpperInvariant() : "MELEE / CUCHILLA";
            ammoText.text = weapon != null ? weapons.Loaded + " <size=26>/ " + weapons.Reserve + "</size>" : "—";
            if (weapons != null && weapons.IsReloading) weaponText.text += "   ·   RECARGANDO";
        }
        void Build()
        {
            canvas = GameUi.Canvas(transform, "CombatHud", 20);
            damageOverlay = GameUi.Image(GameUi.Fill(canvas.transform, "DamageOverlay"), new Color(.5f, .01f, .005f, 0));
            fallOverlay = GameUi.Image(GameUi.Fill(canvas.transform, "VoidDarkness"), new Color(0, 0, 0, 0));
            var stats = GameUi.Rect(canvas.transform, "VitalPanel", Vector2.zero, Vector2.zero, new Vector2(28, 28), new Vector2(420, 110));
            GameUi.Image(stats, GameUi.Ink);
            Label(stats, "HealthLabel", "+  SALUD", new Vector2(18, 72), new Vector2(180, 26), 17, GameUi.Accent);
            Label(stats, "ArmorLabel", "ARMADURA", new Vector2(226, 72), new Vector2(180, 26), 17, new Color(.4f, .7f, 1));
            healthText = Label(stats, "Health", "100", new Vector2(18, 22), new Vector2(180, 56), 42, GameUi.Paper, UiFontRole.System);
            armorText = Label(stats, "Armor", "0", new Vector2(226, 22), new Vector2(180, 56), 42, GameUi.Paper, UiFontRole.System);
            healthFill = Bar(stats, "HealthBar", new Vector2(18, 12), new Vector2(178, 5), GameUi.Accent);
            armorFill = Bar(stats, "ArmorBar", new Vector2(226, 12), new Vector2(178, 5), new Color(.4f, .7f, 1));
            var ammo = GameUi.Rect(canvas.transform, "AmmoPanel", new Vector2(1, 0), new Vector2(1, 0), new Vector2(-28, 28), new Vector2(410, 110));
            GameUi.Image(ammo, GameUi.Ink);
            weaponText = Label(ammo, "Weapon", "MELEE", new Vector2(18, 75), new Vector2(380, 26), 17, GameUi.Accent);
            ammoText = Label(ammo, "Ammo", "—", new Vector2(18, 20), new Vector2(380, 60), 44, GameUi.Paper, UiFontRole.System);
            reloadFill = Bar(ammo, "ReloadProgress", new Vector2(18, 12), new Vector2(374, 5), GameUi.Accent);
            crosshair = GameUi.Rect(canvas.transform, "Crosshair", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(60, 60));
            CrosshairScale = crosshairScale;
            hitMarks = new UnityEngine.UI.Image[4];
            for (int i = 0; i < 4; i++)
            {
                float angle = i * 90;
                var rect = GameUi.Rect(crosshair, "Aim" + i, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Quaternion.Euler(0, 0, angle) * new Vector3(10, 0), new Vector2(9, 2));
                rect.localRotation = Quaternion.Euler(0, 0, angle); GameUi.Image(rect, GameUi.Paper);
                var mark = GameUi.Rect(crosshair, "Hit" + i, new Vector2(.5f, .5f), new Vector2(.5f, .5f), Quaternion.Euler(0, 0, angle + 45) * new Vector3(20, 0), new Vector2(10, 2));
                mark.localRotation = Quaternion.Euler(0, 0, angle + 45); hitMarks[i] = GameUi.Image(mark, GameUi.Accent); hitMarks[i].enabled = false;
            }
            statusText = GameUi.Text(canvas.transform, "State", "", new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -28), new Vector2(700, 32), 21, GameUi.Paper, TextAnchor.MiddleCenter, UiFontRole.System);
            pickupText = GameUi.Text(canvas.transform, "PickupMessage", "", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, -95), new Vector2(600, 32), 22, GameUi.Accent, TextAnchor.MiddleCenter, UiFontRole.Industrial);
            AddOutline(statusText);
            AddOutline(pickupText);
            for (int i = 0; i < 6; i++)
            {
                var card = GameUi.Rect(canvas.transform, "Power" + i, Vector2.one, Vector2.one, new Vector2(-28, -100 - i * 72), new Vector2(230, 62));
                GameUi.Image(card, GameUi.Ink); cards[i] = card.gameObject;
                var icon = GameUi.Image(GameUi.Rect(card, "Icon", Vector2.zero, Vector2.zero, new Vector2(8, 10), new Vector2(42, 42)), PresentationTheme.PowerColor((PowerUpType)i));
                if (theme != null) icon.sprite = theme.Icon((PowerUpType)i);
                timers[i] = Label(card, "Timer", "", new Vector2(62, 8), new Vector2(164, 50), 17, GameUi.Paper, UiFontRole.System);
                powerBars[i] = Bar(card, "Duration", Vector2.zero, new Vector2(230, 3), PresentationTheme.PowerColor((PowerUpType)i));
                card.gameObject.SetActive(false);
            }
            deathPanel = GameUi.Rect(canvas.transform, "DeathPanel", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(720, 280));
            GameUi.Image(deathPanel, GameUi.Ink);
            deathText = GameUi.Text(deathPanel, "Death", "", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(680, 230), 48, GameUi.Accent, TextAnchor.MiddleCenter, UiFontRole.Industrial);
            deathPanel.gameObject.SetActive(false);
        }
        static UnityEngine.UI.Text Label(Transform parent, string name, string value, Vector2 position, Vector2 size, int fontSize, Color color, UiFontRole role = UiFontRole.Industrial)
            => GameUi.Text(parent, name, value, Vector2.zero, Vector2.zero, position, size, fontSize, color, role: role);
        static void AddOutline(UnityEngine.UI.Text text)
        {
            var outline = text.gameObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(0, 0, 0, .95f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }
        static UnityEngine.UI.Image Bar(Transform parent, string name, Vector2 pos, Vector2 size, Color color) => GameUi.Image(GameUi.Rect(parent, name, Vector2.zero, Vector2.zero, pos, size), color);
        static void Alpha(UnityEngine.UI.Image image, float value) { var c = image.color; c.a = value; image.color = c; }
    }
}
