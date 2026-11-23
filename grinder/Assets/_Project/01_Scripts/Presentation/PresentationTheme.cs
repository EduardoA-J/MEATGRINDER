using Meatgrinder.Pickups;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    [CreateAssetMenu(menuName = "MEATGRINDER/Presentation Theme")]
    public sealed class PresentationTheme : ScriptableObject
    {
        [SerializeField] private Sprite[] powerIcons;
        [SerializeField] private AudioClip[] feedbackSounds;
        public Sprite Icon(PowerUpType type) => powerIcons != null && (int)type < powerIcons.Length ? powerIcons[(int)type] : null;
        public AudioClip Sound(int index) => feedbackSounds != null && index >= 0 && index < feedbackSounds.Length ? feedbackSounds[index] : null;
        public static Color PowerColor(PowerUpType type) => type switch
        {
            PowerUpType.QuadDamage => new Color(.65f, .35f, 1f),
            PowerUpType.Megahealth => new Color(.18f, .75f, 1f),
            PowerUpType.Haste => new Color(1, .8f, .15f),
            PowerUpType.Invulnerability => new Color(.9f, .95f, 1),
            PowerUpType.Berserk => new Color(1, .2f, .15f),
            _ => new Color(.3f, 1, .5f)
        };
        public static string PowerName(PowerUpType type) => type switch
        {
            PowerUpType.QuadDamage => "QUAD ×4", PowerUpType.Megahealth => "MEGASALUD", PowerUpType.Haste => "HASTE",
            PowerUpType.Invulnerability => "INVULNERABLE", PowerUpType.Berserk => "BERSERK", _ => "REGENERACIÓN"
        };
    }
}
