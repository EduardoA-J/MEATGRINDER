using Meatgrinder.Core;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    [RequireComponent(typeof(LocalGameSession))]
    public sealed class SessionMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip afterFirstSong;
        LocalGameSession session;
        AudioSource source;
        MapDefinition playingMap;
        bool initialized, awaitingFollowUp, heardFirstSong, applicationPaused;

        public AudioClip CurrentSong => source != null ? source.clip : null;

        void Awake()
        {
            session = GetComponent<LocalGameSession>();
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0;
            source.ignoreListenerPause = true;
        }
        void OnEnable() => session.Changed += Refresh;
        void Start() => Refresh();
        void OnDisable()
        {
            session.Changed -= Refresh;
            source.Stop();
            initialized = false;
        }
        void OnApplicationPause(bool paused) => applicationPaused = paused;
        void Refresh()
        {
            // Keep the outgoing track during async loading. Only a new destination changes music.
            if (session.Phase == LocalSessionPhase.Loading) return;
            var map = session.Phase == LocalSessionPhase.Menu ? null : session.CurrentMap;
            if (initialized && map == playingMap) return;
            initialized = true;
            playingMap = map;
            awaitingFollowUp = map != null;
            heardFirstSong = false;
            Play(map != null ? map.FirstSong : menuMusic, map == null);
        }
        void Update()
        {
            if (!awaitingFollowUp || applicationPaused) return;
            if (source.isPlaying) { heardFirstSong = true; return; }
            if (!heardFirstSong) return;
            awaitingFollowUp = false;
            Play(afterFirstSong, true);
        }
        void Play(AudioClip clip, bool loop)
        {
            source.Stop();
            source.clip = clip;
            source.loop = loop;
            if (clip != null) source.Play();
        }
    }
}
