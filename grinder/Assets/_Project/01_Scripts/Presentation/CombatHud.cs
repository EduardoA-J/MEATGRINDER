using Meatgrinder.Combat;
using Meatgrinder.Players;
using Meatgrinder.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace Meatgrinder.Presentation
{
    public sealed class CombatHud : MonoBehaviour
    {
        [SerializeField] private WeaponController weapons;
        [SerializeField] private HealthComponent health;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private Meatgrinder.Pickups.PowerUpManager powerUps;
        [SerializeField] private float lineLength = 12f;
        [SerializeField] private float lineThickness = 2f;
        [SerializeField] private float gap = 5f;
        [SerializeField] private float crosshairScale = 1f;
        [SerializeField] private Color lineColor = new Color(0.95f, 0.95f, 0.92f, 1f);
        [SerializeField] private Color outlineColor = new Color(0f, 0f, 0f, 0.8f);
        [SerializeField] private Color hitColor = new Color(1f, 0.82f, 0.25f, 1f);
        [SerializeField] private Color worldHitColor = new Color(0.8f, 0.8f, 0.8f, 0.55f);

        Text healthText;
        Text armorText;
        Text weaponText;
        Text ammoText;
        Text powerUpText;
        Image healthFill;
        Image armorFill;
        Image vignette;
        Image[] hitMarks;
        float vignetteUntil;
        float hitUntil;

        public float CrosshairScale
        {
            get => crosshairScale;
            set => crosshairScale = Mathf.Max(0.2f, value);
        }

        public string HealthReadout => healthText != null ? healthText.text : string.Empty;
        public string AmmoReadout => ammoText != null ? ammoText.text : string.Empty;

        void Awake()
        {
            if (weapons == null)
            {
                weapons = GetComponent<WeaponController>();
            }

            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            if (feedback == null)
            {
                feedback = GetComponent<PlayerImpactFeedback>();
            }

            if (powerUps == null)
            {
                powerUps = GetComponent<Meatgrinder.Pickups.PowerUpManager>();
            }

            Build();
        }

        void OnEnable()
        {
            if (weapons != null)
            {
                weapons.StateChanged += RefreshWeapon;
            }

            if (health != null)
            {
                health.Damaged += OnDamaged;
                health.Died += OnDied;
                health.ArmorChanged += OnArmorChanged;
            }

            if (feedback != null)
            {
                feedback.Presented += OnPresented;
            }

            if (powerUps != null)
            {
                powerUps.PowerUpActivated += OnPowerUpActivated;
                powerUps.PowerUpExpired += OnPowerUpExpired;
            }

            RefreshAll();
        }

        void OnDisable()
        {
            if (weapons != null)
            {
                weapons.StateChanged -= RefreshWeapon;
            }

            if (health != null)
            {
                health.Damaged -= OnDamaged;
                health.Died -= OnDied;
                health.ArmorChanged -= OnArmorChanged;
            }

            if (feedback != null)
            {
                feedback.Presented -= OnPresented;
            }

            if (powerUps != null)
            {
                powerUps.PowerUpActivated -= OnPowerUpActivated;
                powerUps.PowerUpExpired -= OnPowerUpExpired;
            }
        }

        void Update()
        {
            if (vignette != null && vignette.color.a > 0f && Time.time >= vignetteUntil)
            {
                SetAlpha(vignette, 0f);
            }

            if (hitMarks != null && Time.time >= hitUntil)
            {
                SetHitVisible(false);
            }

            if (powerUps == null)
            {
                powerUps = GetComponent<Meatgrinder.Pickups.PowerUpManager>();
                if (powerUps != null)
                {
                    powerUps.PowerUpActivated += OnPowerUpActivated;
                    powerUps.PowerUpExpired += OnPowerUpExpired;
                }
            }

            if (powerUps != null && powerUpText != null)
            {
                RefreshPowerUps();
            }
        }

        void OnArmorChanged(float newArmor)
        {
            RefreshArmor();
        }

        void OnPowerUpActivated(Meatgrinder.Pickups.PowerUpType type, float duration)
        {
            RefreshPowerUps();
        }

        void OnPowerUpExpired(Meatgrinder.Pickups.PowerUpType type)
        {
            RefreshPowerUps();
        }

        public void RefreshAll()
        {
            RefreshHealth();
            RefreshArmor();
            RefreshWeapon();
            RefreshPowerUps();
        }

        void OnDamaged(float amount)
        {
            RefreshHealth();
            if (vignette == null || amount <= 0f)
            {
                return;
            }

            SetAlpha(vignette, 0.38f);
            vignetteUntil = Time.time + 0.18f;
        }

        void OnDied()
        {
            RefreshHealth();
        }

        void OnPresented(bool damagedTarget)
        {
            hitUntil = Time.time + 0.12f;
            Color color = damagedTarget ? hitColor : worldHitColor;
            if (hitMarks == null)
            {
                return;
            }

            for (int i = 0; i < hitMarks.Length; i++)
            {
                hitMarks[i].color = color;
                hitMarks[i].enabled = true;
            }
        }

        void RefreshHealth()
        {
            if (healthText == null)
            {
                return;
            }

            if (health == null)
            {
                healthText.text = "salud —";
                return;
            }

            healthText.text = health.IsAlive
                ? "salud  " + health.Current.ToString("0") + " / " + health.Max.ToString("0")
                : "muerto";
            if (healthFill != null)
            {
                healthFill.fillAmount = health.Max > 0f ? health.Current / health.Max : 0f;
            }
        }

        void RefreshWeapon()
        {
            if (ammoText == null)
            {
                return;
            }

            WeaponDefinition weapon = weapons != null ? weapons.ActiveDefinition : null;
            if (weaponText != null)
            {
                weaponText.text = weapon != null ? weapon.DisplayName : "—";
            }

            if (weapons == null || weapon == null)
            {
                ammoText.text = "—";
                return;
            }

            string state = weapons.IsDrawing ? "   saca" : weapons.IsReloading ? "   recarga" : string.Empty;
            ammoText.text = weapons.Loaded + "  /  " + weapons.Reserve + state;
        }

        void RefreshArmor()
        {
            if (armorText == null)
            {
                return;
            }

            if (health == null)
            {
                armorText.text = "armadura —";
                return;
            }

            armorText.text = "armadura  " + health.Armor.ToString("0") + " / " + health.MaxArmor.ToString("0");
            if (armorFill != null)
            {
                armorFill.fillAmount = health.MaxArmor > 0f ? health.Armor / health.MaxArmor : 0f;
            }
        }

        void RefreshPowerUps()
        {
            if (powerUpText == null || powerUps == null)
            {
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var types = System.Enum.GetValues(typeof(Meatgrinder.Pickups.PowerUpType));
            foreach (Meatgrinder.Pickups.PowerUpType type in types)
            {
                if (powerUps.HasEffect(type))
                {
                    float remaining = powerUps.GetTimeRemaining(type);
                    if (sb.Length > 0)
                    {
                        sb.Append("  |  ");
                    }
                    sb.Append(type.ToString()).Append(" ").Append(remaining.ToString("0.0")).Append("s");
                }
            }

            powerUpText.text = sb.Length > 0 ? sb.ToString() : "";
        }

        void SetHitVisible(bool visible)
        {
            if (hitMarks == null)
            {
                return;
            }

            for (int i = 0; i < hitMarks.Length; i++)
            {
                hitMarks[i].enabled = visible;
            }
        }

        static void SetAlpha(Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        void Build()
        {
            if (transform.Find("CombatHud") != null)
            {
                return;
            }

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            GameObject root = new GameObject("CombatHud", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            vignette = Panel(root.transform, "DamageVignette", new Color(0.55f, 0.05f, 0.04f, 0f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            vignette.rectTransform.offsetMin = Vector2.zero;
            vignette.rectTransform.offsetMax = Vector2.zero;

            BuildCrosshair(root.transform);
            BuildCorner(root.transform, font, true);
            BuildCorner(root.transform, font, false);
            BuildPowerUpPanel(root.transform, font);
        }

        void BuildCrosshair(Transform parent)
        {
            RectTransform pivot = NewRect(parent, "Crosshair", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            pivot.sizeDelta = new Vector2(64f, 64f);
            float length = lineLength * crosshairScale;
            float thick = lineThickness * crosshairScale;
            float space = gap * crosshairScale;
            Bar(pivot, "Up", new Vector2(0f, space + (length * 0.5f)), new Vector2(thick, length));
            Bar(pivot, "Down", new Vector2(0f, -(space + (length * 0.5f))), new Vector2(thick, length));
            Bar(pivot, "Left", new Vector2(-(space + (length * 0.5f)), 0f), new Vector2(length, thick));
            Bar(pivot, "Right", new Vector2(space + (length * 0.5f), 0f), new Vector2(length, thick));
            Image dot = ImageOn(NewRect(pivot, "Dot", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero), lineColor);
            dot.rectTransform.sizeDelta = new Vector2(thick, thick);
            hitMarks = new Image[4];
            hitMarks[0] = Slash(pivot, "HitA", 45f);
            hitMarks[1] = Slash(pivot, "HitB", -45f);
            hitMarks[2] = Slash(pivot, "HitC", 135f);
            hitMarks[3] = Slash(pivot, "HitD", -135f);
            SetHitVisible(false);
        }

        void BuildCorner(Transform parent, Font font, bool healthCorner)
        {
            Vector2 anchor = healthCorner ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
            RectTransform panel = NewRect(parent, healthCorner ? "HealthPanel" : "AmmoPanel", anchor, anchor, healthCorner ? new Vector2(28f, 28f) : new Vector2(-28f, 28f));
            panel.sizeDelta = new Vector2(320f, healthCorner ? 120f : 92f); // Más alto para armadura
            Image background = panel.gameObject.AddComponent<Image>();
            background.sprite = White();
            background.color = new Color(0.04f, 0.04f, 0.05f, 0.62f);
            background.raycastTarget = false;
            if (healthCorner)
            {
                healthText = Label(panel, font, "HealthText", new Vector2(16f, 74f), 26, TextAnchor.MiddleLeft);
                RectTransform healthBar = NewRect(panel, "HealthBar", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(16f, 48f));
                healthBar.sizeDelta = new Vector2(288f, 14f);
                Image healthBack = healthBar.gameObject.AddComponent<Image>();
                healthBack.sprite = White();
                healthBack.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                healthBack.raycastTarget = false;
                healthFill = ImageOn(NewRect(healthBar, "Fill", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0f)), new Color(0.72f, 0.16f, 0.12f, 1f));
                healthFill.rectTransform.pivot = new Vector2(0f, 0.5f);
                healthFill.rectTransform.sizeDelta = new Vector2(288f, 14f);
                healthFill.type = Image.Type.Filled;
                healthFill.fillMethod = Image.FillMethod.Horizontal;
                healthFill.fillAmount = 1f;

                // Armadura
                armorText = Label(panel, font, "ArmorText", new Vector2(16f, 26f), 22, TextAnchor.MiddleLeft);
                RectTransform armorBar = NewRect(panel, "ArmorBar", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(16f, 8f));
                armorBar.sizeDelta = new Vector2(288f, 10f);
                Image armorBack = armorBar.gameObject.AddComponent<Image>();
                armorBack.sprite = White();
                armorBack.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                armorBack.raycastTarget = false;
                armorFill = ImageOn(NewRect(armorBar, "Fill", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0f)), new Color(0.25f, 0.5f, 0.75f, 1f));
                armorFill.rectTransform.pivot = new Vector2(0f, 0.5f);
                armorFill.rectTransform.sizeDelta = new Vector2(288f, 10f);
                armorFill.type = Image.Type.Filled;
                armorFill.fillMethod = Image.FillMethod.Horizontal;
                armorFill.fillAmount = 0f;
            }
            else
            {
                weaponText = Label(panel, font, "WeaponText", new Vector2(16f, 50f), 22, TextAnchor.MiddleLeft);
                ammoText = Label(panel, font, "AmmoText", new Vector2(16f, 16f), 28, TextAnchor.MiddleLeft);
            }
        }

        void BuildPowerUpPanel(Transform parent, Font font)
        {
            // Panel superior central para power-ups activos
            RectTransform panel = NewRect(parent, "PowerUpPanel", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -28f));
            panel.sizeDelta = new Vector2(600f, 32f);
            Image background = panel.gameObject.AddComponent<Image>();
            background.sprite = White();
            background.color = new Color(0.04f, 0.04f, 0.05f, 0.62f);
            background.raycastTarget = false;
            
            powerUpText = Label(panel, font, "PowerUpText", new Vector2(8f, 16f), 18, TextAnchor.MiddleCenter);
            powerUpText.rectTransform.sizeDelta = new Vector2(584f, 32f);
        }

        void Bar(RectTransform parent, string name, Vector2 position, Vector2 size)
        {
            Image outline = ImageOn(NewRect(parent, name + "Outline", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position), outlineColor);
            outline.rectTransform.sizeDelta = size + new Vector2(2f, 2f);
            Image line = ImageOn(NewRect(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position), lineColor);
            line.rectTransform.sizeDelta = size;
        }

        Image Slash(RectTransform parent, string name, float angle)
        {
            RectTransform rect = NewRect(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
            rect.sizeDelta = new Vector2(10f, 2f);
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
            rect.anchoredPosition = Quaternion.Euler(0f, 0f, angle) * new Vector3(18f, 0f, 0f);
            return ImageOn(rect, hitColor);
        }

        Text Label(RectTransform parent, Font font, string name, Vector2 position, int size, TextAnchor anchor)
        {
            RectTransform rect = NewRect(parent, name, new Vector2(0f, 0f), new Vector2(0f, 0f), position);
            rect.sizeDelta = new Vector2(290f, 32f);
            Text text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = anchor;
            text.color = new Color(0.94f, 0.93f, 0.9f, 1f);
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        static Image Panel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position)
        {
            RectTransform rect = NewRect(parent, name, anchorMin, anchorMax, position);
            rect.pivot = pivot;
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = White();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        static Image ImageOn(RectTransform rect, Color color)
        {
            Image image = rect.gameObject.AddComponent<Image>();
            image.sprite = White();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        static RectTransform NewRect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position)
        {
            GameObject item = new GameObject(name, typeof(RectTransform));
            item.transform.SetParent(parent, false);
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            return rect;
        }

        static Sprite white;

        static Sprite White()
        {
            if (white != null)
            {
                return white;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            white = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return white;
        }
    }
}
