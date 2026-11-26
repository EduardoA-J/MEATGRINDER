using Meatgrinder.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Meatgrinder.Presentation
{
    [RequireComponent(typeof(LocalGameSession))]
    public sealed class SessionMenu : MonoBehaviour
    {
        LocalGameSession session;
        Canvas canvas;
        EventSystem menuEvents;
        RectTransform main, maps, options, credits, pause, results, setup, optionsReturn;
        RectTransform activePage, departingPage, pageBeforeLoading;
        CanvasGroup activeGroup, departingGroup;
        LocalSessionPhase? displayedPhase, phaseBeforeLoading;
        MapDefinition selectedMap;
        bool configuring, selectingInitial;
        UnityEngine.UI.Text setupText;
        UnityEngine.UI.Text resultText, errorText;
        UnityEngine.UI.Slider volume, sensitivity, fov;
        UnityEngine.UI.Button motionButton, flashesButton;
        AudioSource source;
        [SerializeField] private AudioClip navigationSound;
        [SerializeField] private Texture2D menuBackground;
        public Texture2D MenuBackground => menuBackground;
        ScreenBackground background;
        UnityEngine.UI.Image scrim;
        static readonly Vector2 PagePosition = new Vector2(68, -310);
        const float PageTransitionSeconds = .19f;
        float pageProgress, departingAlpha, nextFocusSound;
        void Awake()
        {
            session = GetComponent<LocalGameSession>();
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.ignoreListenerPause = true;
            Build();
        }
        void OnEnable() { if (session != null) session.Changed += Refresh; }
        void Start() => Refresh();
        void OnDisable() { if (session != null) session.Changed -= Refresh; }
        void Update()
        {
            if (activePage == null || pageProgress >= 1) return;
            pageProgress = session.ReduceMotion ? 1 : Mathf.MoveTowards(pageProgress, 1, Time.unscaledDeltaTime / PageTransitionSeconds);
            float eased = 1 - (1 - pageProgress) * (1 - pageProgress);
            activeGroup.alpha = eased;
            activePage.anchoredPosition = PagePosition + Vector2.right * (16 * (1 - eased));
            if (departingPage == null) return;
            departingGroup.alpha = departingAlpha * (1 - eased);
            departingPage.anchoredPosition = PagePosition - Vector2.right * (10 * eased);
            if (pageProgress >= 1) ClearDepartingPage();
        }
        void Build()
        {
            canvas = GameUi.Canvas(transform, "SessionScreens", 100);
            canvas.GetComponent<UnityEngine.UI.CanvasScaler>().screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            var events = new GameObject("MenuEventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.transform.SetParent(transform, false);
            menuEvents = events.GetComponent<EventSystem>();
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            background = ScreenBackground.Create(canvas.transform, menuBackground);
            scrim = GameUi.Image(GameUi.Fill(canvas.transform, "Scrim"), new Color(0, 0, 0, .65f));
            GameUi.Image(GameUi.Rect(canvas.transform, "Rule", new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -70), new Vector2(1470, 7)), GameUi.Accent);
            var brand = GameUi.Text(canvas.transform, "Brand", "MEATGRINDER", new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(64, -83), new Vector2(1460, 150), 100, GameUi.Paper, role: UiFontRole.Display);
            brand.resizeTextForBestFit = true;
            brand.resizeTextMinSize = 64;
            brand.resizeTextMaxSize = 100;
            GameUi.Text(canvas.transform, "Tagline", "STEEL. BLOOD. MOMENTUM.", new Vector2(0, 1), new Vector2(0, 1), new Vector2(68, -218), new Vector2(900, 30), 18, GameUi.Paper, role: UiFontRole.System);
            var footer = GameUi.Rect(canvas.transform, "FooterPlate", Vector2.zero, Vector2.zero, new Vector2(48, 18), new Vector2(1488, 62));
            GameUi.Panel(footer);
            GameUi.Text(footer, "Footer", "WASD  MOVER     ESPACIO  SALTAR     SHIFT  SPRINT     CTRL  AGACHARSE     V  MELEE     R  RECARGAR     1 / 2  ARMAS", Vector2.zero, Vector2.zero, new Vector2(16, 10), new Vector2(1450, 40), 16, GameUi.Muted, role: UiFontRole.System);
            main = Page("Main"); maps = Page("MapSelection"); options = Page("Options"); credits = Page("Credits"); pause = Page("Pause"); results = Page("Results"); setup = Page("LocalMatchSetup");
            Add(main, "Play", "JUGAR", 0, () => { configuring = false; Show(maps); });
            Add(main, "Create", "CREAR PARTIDA LOCAL", 1, () => { configuring = true; Show(maps); });
            Add(main, "Options", "OPCIONES", 2, OpenOptions);
            Add(main, "Credits", "CRÉDITOS", 3, () => Show(credits));
            Add(main, "Quit", "SALIR", 4, session.Quit);
            Add(main, "Online", "MULTIJUGADOR", 0, session.OpenOnline, x: 580);
            for (int i = 0; session.Maps != null && i < session.Maps.Length; i++)
            {
                var map = session.Maps[i];
                if (map == null) continue;
                Add(maps, map.MapId, map.MapId + "   " + map.DisplayName, i, () => SelectMap(map), 760);
                GameUi.Text(maps, map.MapId + "Description", map.Description, new Vector2(0, 1), new Vector2(0, 1), new Vector2(800, -i * 70), new Vector2(520, 60), 19, GameUi.Paper);
            }
            Add(maps, "Back", "VOLVER", 5, Back);
            setupText = GameUi.Text(setup, "Configuration", "", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(1300, 190), 27, GameUi.Paper, role: UiFontRole.Industrial);
            Add(setup, "CreateConfirmed", "CREAR PARTIDA", 3, () => session.StartMap(selectedMap));
            Add(setup, "SetupOptions", "AJUSTES DE JUEGO", 4, OpenOptions);
            Add(setup, "ChangeMap", "CAMBIAR MAPA", 5, () => Show(maps));
            errorText = GameUi.Text(canvas.transform, "LoadError", "", Vector2.zero, Vector2.zero, new Vector2(68, 74), new Vector2(1400, 42), 21, GameUi.Accent);
            Add(pause, "Resume", "CONTINUAR", 0, session.Resume);
            Add(pause, "Options", "OPCIONES", 1, OpenOptions);
            Add(pause, "Restart", "REINICIAR MAPA", 2, session.Restart);
            Add(pause, "Finish", "FINALIZAR SESIÓN", 3, session.Finish);
            Add(pause, "MainMenu", "MENÚ PRINCIPAL", 4, session.ReturnToMenu);
            resultText = GameUi.Text(results, "SessionResults", "", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(900, 290), 26, GameUi.Paper, role: UiFontRole.System);
            Add(results, "Replay", "VOLVER A JUGAR", 4, session.Restart);
            Add(results, "MainMenu", "MENÚ PRINCIPAL", 5, session.ReturnToMenu);
            GameUi.Text(credits, "CreditsText", "MEATGRINDER\nDirección y diseño: David\n\nImplementación y recursos procedurales propios.\nModelos aportados en el proyecto.\n\nReferencias de diseño: Quake, DOOM, ULTRAKILL y Left 4 Dead.\nConsulta de SourceEngineMovementUnity, Salvation’s Descent y doom.\nNo se han incorporado sus scripts ni sus recursos comerciales.", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(1300, 330), 24, GameUi.Paper);
            Add(credits, "Back", "VOLVER", 5, Back);
            volume = Slider("VOLUMEN", 0, 0, 1, session.Volume);
            sensitivity = Slider("SENSIBILIDAD", 1, .2f, 3, session.Sensitivity);
            fov = Slider("CAMPO DE VISIÓN", 2, 70, 120, session.FieldOfView);
            motionButton = Add(options, "Motion", "", 3, () => SaveOptions(!session.ReduceMotion, session.ReduceFlashes));
            flashesButton = Add(options, "Flashes", "", 4, () => SaveOptions(session.ReduceMotion, !session.ReduceFlashes));
            Add(options, "Fullscreen", "PANTALLA COMPLETA", 3, () => Screen.fullScreen = !Screen.fullScreen, x: 580);
            Add(options, "Resolution", "1366 × 768 / 1920 × 1080", 4, () => { bool low = Screen.width > 1400; Screen.SetResolution(low ? 1366 : 1920, low ? 768 : 1080, Screen.fullScreenMode); }, x: 580);
            Add(options, "Back", "GUARDAR Y VOLVER", 5, () => { PlayerPrefs.Save(); Back(); });
            volume.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
            sensitivity.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
            fov.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
        }
        RectTransform Page(string name)
        {
            var page = GameUi.Rect(canvas.transform, name, new Vector2(0, 1), new Vector2(0, 1), PagePosition, new Vector2(1450, 480));
            var plate = GameUi.Rect(page, "PagePlate", new Vector2(0, 1), new Vector2(0, 1), new Vector2(-20, 20), new Vector2(1488, 450));
            GameUi.Panel(plate).color = GameUi.Ink;
            page.gameObject.AddComponent<CanvasGroup>();
            page.gameObject.SetActive(false);
            return page;
        }
        UnityEngine.UI.Button Add(Transform page, string name, string label, int row, UnityEngine.Events.UnityAction action, float width = 500, float x = 0)
        {
            return GameUi.Button(page, name, label, new Vector2(x, -row * 70), new Vector2(width, 56),
                () => { PlayNavigation(.45f); action(); }, () => session.ReduceMotion, PlayFocus);
        }
        UnityEngine.UI.Slider Slider(string label, int row, float min, float max, float value)
        {
            GameUi.Text(options, label, label, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -row * 70), new Vector2(300, 50), 21, GameUi.Paper, role: UiFontRole.System);
            var rect = GameUi.Rect(options, label + "Slider", new Vector2(0, 1), new Vector2(0, 1), new Vector2(340, -row * 70 - 12), new Vector2(600, 28));
            GameUi.Panel(rect, false).raycastTarget = true;
            var track = GameUi.Fill(rect, "Track"); track.offsetMin = new Vector2(4, 6); track.offsetMax = new Vector2(-4, -6);
            GameUi.Image(track, GameUi.Ink);
            var fill = GameUi.Image(GameUi.Fill(track, "Fill"), GameUi.Accent);
            var handleArea = GameUi.Fill(rect, "HandleArea"); handleArea.offsetMin = new Vector2(12, 0); handleArea.offsetMax = new Vector2(-12, 0);
            var handle = GameUi.Panel(GameUi.Rect(handleArea, "Handle", Vector2.zero, new Vector2(.5f, .5f), Vector2.zero, new Vector2(22, 38)), false);
            handle.color = Color.white;
            var slider = rect.gameObject.AddComponent<UnityEngine.UI.Slider>(); slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
            slider.fillRect = fill.rectTransform;
            var colors = slider.colors;
            colors.normalColor = GameUi.Muted; colors.highlightedColor = GameUi.Paper; colors.selectedColor = GameUi.Accent;
            colors.pressedColor = GameUi.Pressed; colors.disabledColor = GameUi.Metal; colors.colorMultiplier = 1;
            slider.colors = colors;
            slider.minValue = min; slider.maxValue = max; slider.value = value;
            var readout = GameUi.Text(options, label + "Value", value.ToString("0.00"), new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(980, -row * 70), new Vector2(220, 50), 24, GameUi.Paper, role: UiFontRole.System);
            slider.onValueChanged.AddListener(current => readout.text = current.ToString("0.00"));
            return slider;
        }
        void SaveOptions(bool motion, bool flashes)
        {
            session.SetOptions(volume.value, sensitivity.value, fov.value, motion, flashes);
            UpdateOptionLabels();
        }
        void UpdateOptionLabels()
        {
            motionButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "REDUCIR MOVIMIENTO: " + (session.ReduceMotion ? "SÍ" : "NO");
            flashesButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "REDUCIR DESTELLOS: " + (session.ReduceFlashes ? "SÍ" : "NO");
        }
        void SelectMap(MapDefinition map)
        {
            if (!configuring) { session.StartMap(map); return; }
            selectedMap = map;
            setupText.text = map.MapId + "  /  " + map.DisplayName + "\nMODO DISPONIBLE: PRÁCTICA LOCAL\nInicio solo con melee · Recursos y puertas con llaves\n<size=19>Versus Campaign, Continuous y Ranked: no disponibles.</size>";
            Show(setup);
        }
        void OpenOptions()
        {
            optionsReturn = activePage;
            UpdateOptionLabels(); Show(options);
        }
        void Back() => Show(activePage == options && optionsReturn != null ? optionsReturn : session.Phase == LocalSessionPhase.Paused ? pause : main);
        void PlayNavigation(float volume)
        {
            if (navigationSound != null) source.PlayOneShot(navigationSound, volume);
        }
        void PlayFocus()
        {
            if (selectingInitial || Time.unscaledTime < nextFocusSound) return;
            nextFocusSound = Time.unscaledTime + .08f;
            PlayNavigation(.16f);
        }
        void Refresh()
        {
            if (session.Online)
            {
                Hide();
                menuEvents.gameObject.SetActive(true);
                displayedPhase = null;
                return;
            }
            errorText.text = session.LoadError;
            var phase = session.Phase;
            background.gameObject.SetActive(phase == LocalSessionPhase.Menu);
            scrim.enabled = phase != LocalSessionPhase.Menu;
            if (phase == displayedPhase) return;
            if (phase == LocalSessionPhase.Loading)
            {
                pageBeforeLoading = activePage;
                phaseBeforeLoading = displayedPhase;
            }
            displayedPhase = phase;
            if (phase == LocalSessionPhase.Playing || phase == LocalSessionPhase.Loading)
            {
                Hide();
                return;
            }
            resultText.text = "SESIÓN LOCAL FINALIZADA\n\nTiempo   " + System.TimeSpan.FromSeconds(session.Elapsed).ToString(@"mm\:ss") +
                "     Disparos   " + session.Shots + "\nBlancos destruidos   " + session.TargetsDestroyed + "\nMuertes   " + session.Deaths + "     Recogidas   " + session.Pickups;
            var page = !string.IsNullOrEmpty(session.LoadError) && phaseBeforeLoading == phase && pageBeforeLoading != null
                ? pageBeforeLoading
                : phase == LocalSessionPhase.Results ? results : phase == LocalSessionPhase.Paused ? pause : main;
            pageBeforeLoading = null;
            phaseBeforeLoading = null;
            Show(page);
        }
        void Show(RectTransform page)
        {
            if (activePage == page) return;
            ClearDepartingPage();
            departingPage = activePage;
            departingGroup = activeGroup;
            if (departingPage != null)
            {
                departingAlpha = departingGroup.alpha;
                departingGroup.interactable = false;
                departingGroup.blocksRaycasts = false;
            }
            activePage = page;
            activeGroup = page.GetComponent<CanvasGroup>();
            page.SetAsLastSibling();
            page.gameObject.SetActive(true);
            activeGroup.interactable = true;
            activeGroup.blocksRaycasts = true;
            bool animate = !session.ReduceMotion && !session.ReduceFlashes;
            pageProgress = animate ? 0 : 1;
            activeGroup.alpha = pageProgress;
            page.anchoredPosition = PagePosition + Vector2.right * (animate ? 16 : 0);
            if (!animate) ClearDepartingPage();
            canvas.enabled = true;
            menuEvents.gameObject.SetActive(true);
            var first = page.GetComponentInChildren<UnityEngine.UI.Selectable>();
            selectingInitial = true;
            menuEvents.SetSelectedGameObject(first != null ? first.gameObject : null);
            selectingInitial = false;
        }
        void Hide()
        {
            menuEvents.SetSelectedGameObject(null);
            menuEvents.gameObject.SetActive(false);
            canvas.enabled = false;
            if (activePage != null) activePage.gameObject.SetActive(false);
            activePage = null;
            activeGroup = null;
            ClearDepartingPage();
        }
        void ClearDepartingPage()
        {
            if (departingPage != null) departingPage.gameObject.SetActive(false);
            departingPage = null;
            departingGroup = null;
        }
    }
}
