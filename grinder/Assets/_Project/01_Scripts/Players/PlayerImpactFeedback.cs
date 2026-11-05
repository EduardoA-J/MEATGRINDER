using Meatgrinder.Weapons;
using UnityEngine;

namespace Meatgrinder.Players
{
    public sealed class PlayerImpactFeedback : MonoBehaviour, IImpactPresenter
    {
        [SerializeField] private PlayerLook look;

        const int PoolSize = 16;

        readonly GameObject[] pool = new GameObject[PoolSize];
        readonly float[] poolUntil = new float[PoolSize];
        int poolCursor;
        bool poolReady;

        public int TargetConfirms { get; private set; }
        public int WorldMarks { get; private set; }
        public event System.Action<bool> Presented;
        public event System.Action<float> Recoiled;

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

            Presented?.Invoke(damagedTarget);
            EnsurePool();
            ShowMark(
                poolCursor,
                point,
                damagedTarget ? new Color(0.9f, 0.25f, 0.15f, 1f) : new Color(0.85f, 0.78f, 0.45f, 1f));
            poolCursor = (poolCursor + 1) % PoolSize;
        }

        void Update()
        {
            if (poolReady == false)
            {
                return;
            }

            for (int i = 0; i < PoolSize; i++)
            {
                if (pool[i] != null && pool[i].activeSelf && Time.time >= poolUntil[i])
                {
                    pool[i].SetActive(false);
                }
            }
        }

        public void Kick(float pitch)
        {
            Recoiled?.Invoke(pitch);
            if (look != null)
            {
                look.AddRecoil(pitch);
            }
        }

        void EnsurePool()
        {
            if (poolReady)
            {
                return;
            }

            for (int i = 0; i < PoolSize; i++)
            {
                GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mark.name = "ImpactMark";
                mark.transform.SetParent(transform, false);
                mark.transform.localScale = Vector3.one * 0.14f;
                Collider body = mark.GetComponent<Collider>();
                if (body != null)
                {
                    DestroyImmediate(body);
                }

                mark.SetActive(false);
                pool[i] = mark;
            }

            poolReady = true;
        }

        void ShowMark(int index, Vector3 point, Color color)
        {
            GameObject mark = pool[index];
            mark.transform.position = point;
            Renderer renderer = mark.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }

            mark.SetActive(true);
            poolUntil[index] = Time.time + 0.28f;
        }

        internal void SetLook(PlayerLook value)
        {
            look = value;
        }
    }
}
