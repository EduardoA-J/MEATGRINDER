using System;
using System.Collections;
using Meatgrinder.Combat;
using Meatgrinder.Movement;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Core
{
    public enum LocalSessionPhase { Menu, Loading, Playing, Paused, Results }

    // Local session lifecycle. Competitive rules and Fusion authority remain separate consumers.
    public sealed class LocalGameSession : MonoBehaviour
    {
        const string MenuScene = "Boot";
        [SerializeField] private MapDefinition[] maps;
        public MapDefinition[] Maps => maps;
        public LocalSessionPhase Phase { get; private set; } = LocalSessionPhase.Menu;
        public GameObject Player { get; private set; }
        public MapDefinition CurrentMap { get; private set; }
        public float LoadingProgress { get; private set; }
        public string LoadingDestination { get; private set; }
        public string LoadError { get; private set; }
        public float Elapsed { get; private set; }
        public int Shots { get; private set; }
        public int Deaths { get; private set; }
        public int Pickups { get; private set; }
        public int TargetsDestroyed { get; private set; }
        public event Action Changed;
        public event Action<float> LoadingProgressChanged;
        HealthComponent health;
        PlayerImpactFeedback feedback;
        PlayerLife life;
        bool changingScene;
        public bool ReduceMotion { get; private set; }
        public bool ReduceFlashes { get; private set; }
        public float Volume { get; private set; }
        public float Sensitivity { get; private set; }
        public float FieldOfView { get; private set; }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Volume = PlayerPrefs.GetFloat("Meatgrinder.Volume", .7f);
            Sensitivity = PlayerPrefs.GetFloat("Meatgrinder.Sensitivity", 1f);
            FieldOfView = PlayerPrefs.GetFloat("Meatgrinder.Fov", 90f);
            ReduceMotion = PlayerPrefs.GetInt("Meatgrinder.ReduceMotion", 0) != 0;
            ReduceFlashes = PlayerPrefs.GetInt("Meatgrinder.ReduceFlashes", 0) != 0;
            AudioListener.volume = Volume;
        }
        void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
        void Start()
        {
            if (!changingScene && Player == null) BindScene(SceneManager.GetActiveScene());
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            UnbindPlayer();
            Time.timeScale = 1f;
        }
        void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!changingScene || mode == LoadSceneMode.Single) BindScene(scene);
        }
        void BindScene(Scene scene)
        {
            UnbindPlayer();
            CurrentMap = Array.Find(maps ?? Array.Empty<MapDefinition>(), m => m != null && m.SceneName == scene.name);
            Player = null;
            foreach (var body in FindObjectsByType<PlayerLife>(FindObjectsSortMode.None))
                if (body.gameObject.scene == scene) { Player = body.gameObject; life = body; break; }
            if (Player != null)
            {
                health = Player.GetComponent<HealthComponent>();
                feedback = Player.GetComponent<PlayerImpactFeedback>();
                if (health != null) health.Died += OnDeath;
                if (feedback != null) feedback.Recoiled += OnShot;
                life.Respawned += OnRespawn;
                foreach (var pickup in FindObjectsByType<PickupController>(FindObjectsSortMode.None)) pickup.PickedUp += OnPickup;
                foreach (var target in FindObjectsByType<HealthComponent>(FindObjectsSortMode.None))
                    if (target != health) target.Died += OnTargetDeath;
                ApplyOptions();
            }
            // The destination scene may activate before the async operation reports completion.
            if (changingScene) GatePlayer();
            else SetPhase(Player != null ? LocalSessionPhase.Playing : LocalSessionPhase.Menu);
        }
        void UnbindPlayer()
        {
            if (health != null) health.Died -= OnDeath;
            if (feedback != null) feedback.Recoiled -= OnShot;
            if (life != null) life.Respawned -= OnRespawn;
            foreach (var pickup in FindObjectsByType<PickupController>(FindObjectsSortMode.None)) pickup.PickedUp -= OnPickup;
            foreach (var target in FindObjectsByType<HealthComponent>(FindObjectsSortMode.None)) target.Died -= OnTargetDeath;
            health = null; feedback = null; life = null;
        }
        void Update()
        {
            if (Phase == LocalSessionPhase.Playing) Elapsed += Time.deltaTime;
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            {
                if (Phase == LocalSessionPhase.Playing) Pause();
                else if (Phase == LocalSessionPhase.Paused) Resume();
            }
        }
        public bool StartMap(MapDefinition map) => map != null && TryLoad(map.SceneName, map.DisplayName, resetStats: true);

        bool TryLoad(string sceneName, string destination, bool resetStats)
        {
            if (changingScene) return false;
            if (string.IsNullOrEmpty(sceneName) || !Application.CanStreamedLevelBeLoaded(sceneName))
            {
                LoadError = "El destino no está disponible en esta compilación.";
                Changed?.Invoke();
                return false;
            }
            StartCoroutine(Load(sceneName, destination, resetStats));
            return true;
        }

        IEnumerator Load(string sceneName, string destination, bool resetStats)
        {
            var previousPhase = Phase;
            changingScene = true;
            LoadingDestination = destination;
            LoadError = null;
            LoadingProgress = 0;
            SetPhase(LocalSessionPhase.Loading);
            yield return null;

            AsyncOperation operation = null;
            try
            {
                operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[LocalGameSession] No se pudo iniciar la carga de {sceneName}: {exception.Message}");
            }
            if (operation == null)
            {
                changingScene = false;
                LoadError = "No se pudo iniciar la carga. Vuelve a intentarlo.";
                SetPhase(previousPhase);
                yield break;
            }

            while (!operation.isDone)
            {
                SetLoadingProgress(Mathf.Clamp01(operation.progress));
                yield return null;
            }
            SetLoadingProgress(1);
            yield return null;

            if (resetStats)
            {
                Elapsed = 0;
                Shots = Deaths = Pickups = TargetsDestroyed = 0;
            }
            changingScene = false;
            SetPhase(Player != null ? LocalSessionPhase.Playing : LocalSessionPhase.Menu);
        }

        void SetLoadingProgress(float progress)
        {
            if (LoadingProgress == progress) return;
            LoadingProgress = progress;
            LoadingProgressChanged?.Invoke(progress);
        }
        public void Pause() { if (Phase == LocalSessionPhase.Playing) SetPhase(LocalSessionPhase.Paused); }
        public void Resume() { if (Phase == LocalSessionPhase.Paused) SetPhase(LocalSessionPhase.Playing); }
        public void Restart() { if (CurrentMap != null) StartMap(CurrentMap); }
        public void Finish() { if (Player != null && !changingScene) SetPhase(LocalSessionPhase.Results); }
        public void ReturnToMenu() => TryLoad(MenuScene, "MENÚ PRINCIPAL", resetStats: false);
        public void Quit() { PlayerPrefs.Save(); Application.Quit(); }
        void SetPhase(LocalSessionPhase phase)
        {
            Phase = phase;
            Time.timeScale = phase == LocalSessionPhase.Paused || phase == LocalSessionPhase.Results ? 0f : 1f;
            GatePlayer();
            Changed?.Invoke();
        }
        void GatePlayer()
        {
            bool playing = Phase == LocalSessionPhase.Playing;
            bool alive = health != null && health.IsAlive;
            if (Player != null)
            {
                if (Player.TryGetComponent<PlayerInputReader>(out var input)) input.enabled = playing && alive;
                if (Player.TryGetComponent<PlayerLook>(out var look)) { look.MenuControlsCursor = true; look.enabled = playing && alive; }
                if (Player.TryGetComponent<PlayerMovement>(out var move)) move.enabled = playing && alive;
            }
            Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !playing;
        }
        void OnDeath() { Deaths++; GatePlayer(); }
        void OnRespawn() => GatePlayer();
        void OnShot(float value) => Shots++;
        void OnPickup(PickupDefinition definition, GameObject collector) { if (collector == Player) Pickups++; }
        void OnTargetDeath() => TargetsDestroyed++;
        public void SetOptions(float volume, float sensitivity, float fov, bool reducedMotion, bool reducedFlashes)
        {
            Volume = Mathf.Clamp01(volume); Sensitivity = Mathf.Clamp(sensitivity, .2f, 3f); FieldOfView = Mathf.Clamp(fov, 70, 120);
            ReduceMotion = reducedMotion; ReduceFlashes = reducedFlashes;
            PlayerPrefs.SetFloat("Meatgrinder.Volume", Volume);
            PlayerPrefs.SetFloat("Meatgrinder.Sensitivity", Sensitivity);
            PlayerPrefs.SetFloat("Meatgrinder.Fov", FieldOfView);
            PlayerPrefs.SetInt("Meatgrinder.ReduceMotion", ReduceMotion ? 1 : 0);
            PlayerPrefs.SetInt("Meatgrinder.ReduceFlashes", ReduceFlashes ? 1 : 0);
            ApplyOptions();
        }
        void ApplyOptions()
        {
            AudioListener.volume = Volume;
            if (Player != null && Player.TryGetComponent<PlayerLook>(out var look))
            { look.SensitivityScale = Sensitivity; look.FieldOfViewOverride = FieldOfView; }
        }
    }
}
