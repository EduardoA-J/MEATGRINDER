using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class PlayerImpactFeedback : MonoBehaviour, IImpactPresenter
    {
        [SerializeField] private PlayerLook look;

        public int TargetConfirms { get; private set; }
        public int WorldMarks { get; private set; }

        public void Present(Vector3 point, bool damagedTarget)
        {
            if (damagedTarget)
            {
                TargetConfirms++;
            }
            else
            {
                WorldMarks++;
            }

            GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Collider body = mark.GetComponent<Collider>();
            if (body != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(body);
                }
                else
                {
                    DestroyImmediate(body);
                }
            }

            mark.name = damagedTarget ? "HitConfirm" : "HitWorld";
            mark.transform.position = point;
            mark.transform.localScale = Vector3.one * 0.16f;
            Renderer renderer = mark.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = damagedTarget
                    ? new Color(0.82f, 0.82f, 0.82f, 1f)
                    : new Color(0.32f, 0.32f, 0.32f, 1f);
            }

            if (Application.isPlaying)
            {
                Destroy(mark, 0.35f);
            }
        }

        public void Kick(float pitch)
        {
            if (look != null)
            {
                look.AddRecoil(pitch);
            }
        }

        internal void SetLook(PlayerLook value)
        {
            look = value;
        }
    }
}
