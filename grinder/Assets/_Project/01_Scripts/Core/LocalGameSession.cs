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
        [SerializeField] private MapDefinition[] maps;
        public MapDefinition[] Maps => maps;
        public LocalSessionPhase Phase { get; private set; } = LocalSessionPhase.Menu;
        public GameObject Player { get; private set; }
        public MapDefinition CurrentMap { get; private set; }
        public float LoadingProgress { get; private set; }
        public string LoadError { get; private set; }
        public float Elapsed { get; private set; }
        public int Shots { get; private set; }
        public int Deaths { get; private set; }
        public int Pickups { get; private set; }
        public int TargetsDestroyed { get; private set; }
        public event Action Changed;
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
            if (Player == null) BindScene(SceneManager.GetActiveScene());
        }
        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            UnbindPlayer();
            Time.timeScale = 1f;
        }
        void SceneLoaded(Scene scene, LoadSceneMode mode) => BindScene(scene);
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
                SetPhase(LocalSessionPhase.Playing);
                ApplyOptions();
            }
            else SetPhase(LocalSessionPhase.Menu);
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
        public bool StartMap(MapDefinition map)
        {
            if (changingScene || map == null) return false;
            if (!Application.CanStreamedLevelBeLoaded(map.SceneName))
            {
                LoadError = "El mapa no está disponible en esta compilación.";
                Changed?.Invoke();
                return false;
            }
            Elapsed = 0; Shots = Deaths = Pickups = TargetsDestroyed = 0;
            StartCoroutine(Load(map.SceneName));
            return true;
        }
        IEnumerator Load(string sceneName)
        {
            changingScene = true;
            LoadError = null;
            SetPhase(LocalSessionPhase.Loading);
            Time.timeScale = 1f;
            LoadingProgress = 0;
            yield return null;
            var operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone) { LoadingProgress = Mathf.Clamp01(operation.progress / .9f); yield return null; }
            LoadingProgress = 1;
            changingScene = false;
        }
        public void Pause() { if (Phase == LocalSessionPhase.Playing) SetPhase(LocalSessionPhase.Paused); }
        public void Resume() { if (Phase == LocalSessionPhase.Paused) SetPhase(LocalSessionPhase.Playing); }
        public void Restart() { if (CurrentMap != null) StartMap(CurrentMap); }
        public void Finish() { if (Player != null && !changingScene) SetPhase(LocalSessionPhase.Results); }
        public void ReturnToMenu() { if (!changingScene) StartCoroutine(Load("Boot")); }
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
