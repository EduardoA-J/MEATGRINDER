using UnityEngine;

namespace Meatgrinder.Combat
{
    public sealed class CombatTargetLabel : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;

        void Awake()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }
        }

        void OnGUI()
        {
            if (health == null)
            {
                return;
            }

            Camera view = Camera.main;
            if (view == null)
            {
                return;
            }

            Vector3 screen = view.WorldToScreenPoint(transform.position + Vector3.up);
            if (screen.z <= 0f)
            {
                return;
            }

            GUI.Label(
                new Rect(screen.x, Screen.height - screen.y, 140f, 22f),
                health.Current.ToString("0") + "/" + health.Max.ToString("0"));
        }
    }
}
