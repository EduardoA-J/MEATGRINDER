using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class CombatReadout : MonoBehaviour
    {
        [SerializeField] private WeaponController weapons;
        [SerializeField] private PlayerImpactFeedback feedback;
        [SerializeField] private Meatgrinder.Combat.HealthComponent health;

        void OnGUI()
        {
            WeaponDefinition weapon = weapons != null ? weapons.ActiveDefinition : null;
            string name = weapon != null ? weapon.DisplayName : "-";
            int loaded = weapons != null ? weapons.Loaded : 0;
            int reserve = weapons != null ? weapons.Reserve : 0;
            string reload = weapons != null && weapons.IsReloading ? " recarga" : string.Empty;
            float current = health != null ? health.Current : 0f;
            GUI.Label(new Rect(16f, 40f, 560f, 22f), name + "  " + loaded + "/" + reserve + reload);
            GUI.Label(new Rect(16f, 62f, 560f, 22f), "salud " + current.ToString("0"));
            if (feedback != null && feedback.TargetConfirms > 0)
            {
                GUI.Label(new Rect(16f, 84f, 560f, 22f), "impacto");
            }
        }

        internal void Bind(WeaponController weaponController, PlayerImpactFeedback impact, Meatgrinder.Combat.HealthComponent body)
        {
            weapons = weaponController;
            feedback = impact;
            health = body;
        }
    }
}
