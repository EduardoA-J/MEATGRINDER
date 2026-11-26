using UnityEngine;

namespace Meatgrinder.Core
{
    [CreateAssetMenu(menuName = "MEATGRINDER/Map")]
    public sealed class MapDefinition : ScriptableObject
    {
        [SerializeField] private string mapId;
        [SerializeField] private string displayName;
        [SerializeField] private string sceneName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Color accent = new Color(.85f, .24f, .1f);
        [SerializeField] private Texture2D loadingBackground;
        [SerializeField] private AudioClip firstSong;
        public Texture2D LoadingBackground => loadingBackground;
        public AudioClip FirstSong => firstSong;
        public string MapId => mapId;
        public string DisplayName => displayName;
        public string SceneName => sceneName;
        public string Description => description;
        public Color Accent => accent;
    }
}
