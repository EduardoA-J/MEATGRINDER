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
        CanvasGroup group;
        RectTransform main, maps, options, credits, pause, results, loading, setup, optionsReturn;
        MapDefinition selectedMap;
        bool configuring;
        UnityEngine.UI.Text setupText;
        UnityEngine.UI.Text loadingText, resultText, errorText;
        UnityEngine.UI.Slider volume, sensitivity, fov;
        UnityEngine.UI.Button motionButton, flashesButton;
        AudioSource source;
        [SerializeField] private AudioClip navigationSound;
        float shown;
        void Awake()
        {
            session = GetComponent<LocalGameSession>();
            source = gameObject.AddComponent<AudioSource>(); source.ignoreListenerPause = true;
            Build();
        }
        void OnEnable() { if (session != null) session.Changed += Refresh; }
        void Start() => Refresh();
        void OnDisable() { if (session != null) session.Changed -= Refresh; }
        void Update()
        {
            bool visible = session.Phase != LocalSessionPhase.Playing;
            canvas.enabled = visible;
            if (!visible) { shown = 0; return; }
            shown = Mathf.MoveTowards(shown, 1, Time.unscaledDeltaTime * 5);
            group.alpha = shown;
            if (session.Phase == LocalSessionPhase.Loading) loadingText.text = "PREPARANDO ARENA   /   " + Mathf.RoundToInt(session.LoadingProgress * 100) + "%";
        }
        void Build()
        {
            canvas = GameUi.Canvas(transform, "SessionScreens", 100);
            canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            group = canvas.gameObject.AddComponent<CanvasGroup>();
            var events = new GameObject("MenuEventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.transform.SetParent(transform, false);
            events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
            GameUi.Image(GameUi.Fill(canvas.transform, "Scrim"), new Color(.02f, .025f, .03f, .88f));
            GameUi.Image(GameUi.Rect(canvas.transform, "Rule", new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -70), new Vector2(64, 6)), GameUi.Accent);
            GameUi.Text(canvas.transform, "Brand", "MEATGRINDER", new Vector2(0, 1), new Vector2(0, 1), new Vector2(64, -100), new Vector2(1400, 100), 74, GameUi.Paper);
            GameUi.Text(canvas.transform, "Tagline", "STEEL. BLOOD. MOMENTUM.", new Vector2(0, 1), new Vector2(0, 1), new Vector2(68, -206), new Vector2(900, 30), 20, GameUi.Accent);
            GameUi.Text(canvas.transform, "Footer", "WASD  MOVER     ESPACIO  SALTAR     SHIFT  SPRINT     CTRL  AGACHARSE     V  MELEE     R  RECARGAR     1 / 2  ARMAS", Vector2.zero, Vector2.zero, new Vector2(64, 30), new Vector2(1450, 40), 16, new Color(.65f, .66f, .64f));
            main = Page("Main"); maps = Page("MapSelection"); options = Page("Options"); credits = Page("Credits"); pause = Page("Pause"); results = Page("Results"); loading = Page("Loading"); setup = Page("LocalMatchSetup");
            Add(main, "Play", "01   JUGAR", 0, () => { configuring = false; Show(maps); });
            Add(main, "Create", "02   CREAR PARTIDA LOCAL", 1, () => { configuring = true; Show(maps); });
            Add(main, "Options", "03   OPCIONES", 2, OpenOptions);
            Add(main, "Credits", "04   CRÉDITOS", 3, () => Show(credits));
            Add(main, "Quit", "05   SALIR", 4, session.Quit);
            GameUi.Text(main, "Status", "ARENAS ABIERTAS\n\nExplora las rutas. Controla los recursos.\nEmpiezas con melee: busca tu arsenal.\n\nPráctica local disponible.\nVersus, ranked y sesiones de red: no disponibles.", new Vector2(0, 1), new Vector2(0, 1), new Vector2(580, 0), new Vector2(650, 330), 24, GameUi.Paper);
            for (int i = 0; i < session.Maps.Length; i++)
            {
                var map = session.Maps[i];
                if (map == null) continue;
                Add(maps, map.MapId, map.MapId + "   " + map.DisplayName, i, () => SelectMap(map), 760);
                GameUi.Text(maps, map.MapId + "Description", map.Description, new Vector2(0, 1), new Vector2(0, 1), new Vector2(800, -i * 70), new Vector2(520, 60), 19, GameUi.Paper);
            }
            Add(maps, "Back", "VOLVER", 5, Back);
            setupText = GameUi.Text(setup, "Configuration", "", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(1300, 190), 27, GameUi.Paper);
            Add(setup, "CreateConfirmed", "CREAR PARTIDA", 3, () => session.StartMap(selectedMap));
            Add(setup, "SetupOptions", "AJUSTES DE JUEGO", 4, OpenOptions);
            Add(setup, "ChangeMap", "CAMBIAR MAPA", 5, () => Show(maps));
            errorText = GameUi.Text(maps, "LoadError", "", new Vector2(0, 1), new Vector2(0, 1), new Vector2(800, -300), new Vector2(500, 100), 20, GameUi.Accent);
            Add(pause, "Resume", "CONTINUAR", 0, session.Resume);
            Add(pause, "Options", "OPCIONES", 1, OpenOptions);
            Add(pause, "Restart", "REINICIAR MAPA", 2, session.Restart);
            Add(pause, "Finish", "FINALIZAR SESIÓN", 3, session.Finish);
            Add(pause, "MainMenu", "MENÚ PRINCIPAL", 4, session.ReturnToMenu);
            resultText = GameUi.Text(results, "SessionResults", "", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(900, 290), 30, GameUi.Paper);
            Add(results, "Replay", "VOLVER A JUGAR", 4, session.Restart);
            Add(results, "MainMenu", "MENÚ PRINCIPAL", 5, session.ReturnToMenu);
            GameUi.Text(credits, "CreditsText", "MEATGRINDER\nDirección y diseño: David\n\nImplementación y recursos procedurales propios.\nModelos aportados en el proyecto.\n\nReferencias de diseño: Quake, DOOM, ULTRAKILL y Left 4 Dead.\nConsulta de SourceEngineMovementUnity, Salvation’s Descent y doom.\nNo se han incorporado sus scripts ni sus recursos comerciales.", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(1300, 330), 24, GameUi.Paper);
            Add(credits, "Back", "VOLVER", 5, Back);
            loadingText = GameUi.Text(loading, "Progress", "PREPARANDO ARENA", new Vector2(0, 1), new Vector2(0, 1), Vector2.zero, new Vector2(1300, 100), 35, GameUi.Paper);
            volume = Slider("VOLUMEN", 0, 0, 1, session.Volume);
            sensitivity = Slider("SENSIBILIDAD", 1, .2f, 3, session.Sensitivity);
            fov = Slider("CAMPO DE VISIÓN", 2, 70, 120, session.FieldOfView);
            motionButton = Add(options, "Motion", "", 3, () => SaveOptions(!session.ReduceMotion, session.ReduceFlashes));
            flashesButton = Add(options, "Flashes", "", 4, () => SaveOptions(session.ReduceMotion, !session.ReduceFlashes));
            GameUi.Button(options, "Fullscreen", "PANTALLA COMPLETA", new Vector2(580, -210), new Vector2(500, 56), () => Screen.fullScreen = !Screen.fullScreen);
            GameUi.Button(options, "Resolution", "1366 × 768 / 1920 × 1080", new Vector2(580, -280), new Vector2(500, 56), () => { bool low = Screen.width > 1400; Screen.SetResolution(low ? 1366 : 1920, low ? 768 : 1080, Screen.fullScreenMode); });
            Add(options, "Back", "GUARDAR Y VOLVER", 5, () => { PlayerPrefs.Save(); Back(); });
            volume.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
            sensitivity.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
            fov.onValueChanged.AddListener(_ => SaveOptions(session.ReduceMotion, session.ReduceFlashes));
        }
        RectTransform Page(string name) => GameUi.Rect(canvas.transform, name, new Vector2(0, 1), new Vector2(0, 1), new Vector2(68, -310), new Vector2(1450, 480));
        UnityEngine.UI.Button Add(Transform page, string name, string label, int row, UnityEngine.Events.UnityAction action, float width = 500)
        {
            return GameUi.Button(page, name, label, new Vector2(0, -row * 70), new Vector2(width, 56), () => { Click(); action(); });
        }
        UnityEngine.UI.Slider Slider(string label, int row, float min, float max, float value)
        {
            GameUi.Text(options, label, label, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, -row * 70), new Vector2(300, 50), 23, GameUi.Paper);
            var rect = GameUi.Rect(options, label + "Slider", new Vector2(0, 1), new Vector2(0, 1), new Vector2(340, -row * 70 - 12), new Vector2(600, 28));
            GameUi.Image(rect, new Color(.17f, .18f, .19f)).raycastTarget = true;
            var handle = GameUi.Image(GameUi.Rect(rect, "Handle", Vector2.zero, new Vector2(.5f, .5f), Vector2.zero, new Vector2(20, 38)), GameUi.Accent);
            var slider = rect.gameObject.AddComponent<UnityEngine.UI.Slider>(); slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
            slider.minValue = min; slider.maxValue = max; slider.value = value;
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
            optionsReturn = setup.gameObject.activeSelf ? setup : session.Phase == LocalSessionPhase.Paused ? pause : main;
            UpdateOptionLabels(); Show(options);
        }
        void Back() => Show(options.gameObject.activeSelf && optionsReturn != null ? optionsReturn : session.Phase == LocalSessionPhase.Paused ? pause : main);
        void Click() { if (navigationSound != null) source.PlayOneShot(navigationSound, .45f); }
        void Refresh()
        {
            if (errorText != null) errorText.text = session.LoadError;
            resultText.text = "SESIÓN LOCAL FINALIZADA\n\nTiempo   " + System.TimeSpan.FromSeconds(session.Elapsed).ToString(@"mm\:ss") +
                "     Disparos   " + session.Shots + "\nBlancos destruidos   " + session.TargetsDestroyed + "\nMuertes   " + session.Deaths + "     Recogidas   " + session.Pickups;
            Show(session.Phase == LocalSessionPhase.Loading ? loading : session.Phase == LocalSessionPhase.Results ? results : session.Phase == LocalSessionPhase.Paused ? pause : main);
        }
        void Show(RectTransform page)
        {
            foreach (var item in new[] { main, maps, options, credits, pause, results, loading, setup }) item.gameObject.SetActive(item == page);
            shown = 0;
            var first = page.GetComponentInChildren<UnityEngine.UI.Selectable>();
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(first != null ? first.gameObject : null);
        }
    }
}
