using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Players;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    public sealed class CameraFeedback : MonoBehaviour
    {
        PlayerLook look;
        PlayerVoidFall fall;
        HealthComponent health;
        LocalGameSession session;
        Quaternion baseRotation;
        float deathTime;
        void Start()
        {
            look = GetComponent<PlayerLook>(); fall = GetComponent<PlayerVoidFall>(); health = GetComponent<HealthComponent>();
            session = FindFirstObjectByType<LocalGameSession>();
        }
        void LateUpdate()
        {
            if (look == null || look.View == null) return;
            if (health != null && health.IsAlive) { baseRotation = look.View.transform.localRotation; deathTime = 0; }
            else deathTime += Time.deltaTime;
            if (session != null && session.ReduceMotion) return;
            float falling = fall != null && fall.IsFalling ? .3f + fall.Progress * .7f : 0;
            float shake = Mathf.Sin(Time.time * 53f) * falling * 1.2f;
            float roll = Mathf.Sin(Time.time * 39f) * falling * 2.5f + Mathf.Min(deathTime, 1f) * 16;
            look.View.transform.localRotation = baseRotation * Quaternion.Euler(shake + Mathf.Min(deathTime, 1f) * 12, 0, roll);
        }
    }
}
