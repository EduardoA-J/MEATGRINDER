using Meatgrinder.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Meatgrinder.Presentation
{
    public sealed class TargetBillboard : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private string displayName = "Objetivo";
        [SerializeField] private Material bodyMaterial;
        [SerializeField] private Color bodyColor = new Color(0.8f, 0.42f, 0.12f, 1f);
        [SerializeField] private bool bulky;

        Transform canvas;
        Text readout;
        Renderer[] body;
        Color resting;
        float flashUntil;
        bool dead;
        Camera view;

        void Awake()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }

            resting = bodyColor;
            EnsureSilhouette();
            EnsureCanvas();
        }

        void OnEnable()
        {
            if (health == null)
            {
                return;
            }

            health.Damaged += OnDamaged;
            health.Died += OnDied;
        }

        void OnDisable()
        {
            if (health == null)
            {
                return;
            }

            health.Damaged -= OnDamaged;
            health.Died -= OnDied;
        }

        void LateUpdate()
        {
            if (view == null)
            {
                view = Camera.main;
            }

            if (canvas != null && view != null)
            {
                Vector3 toward = view.transform.position - canvas.position;
                if (toward.sqrMagnitude > 0.001f)
                {
                    canvas.rotation = Quaternion.LookRotation(toward, Vector3.up);
                }
            }

            if (dead == false && flashUntil > 0f && Time.time >= flashUntil)
            {
                flashUntil = 0f;
                Paint(resting);
            }

            WriteReadout();
        }

        void OnDamaged(float amount)
        {
            if (dead || amount <= 0f)
            {
                return;
            }

            flashUntil = Time.time + 0.12f;
            Paint(new Color(0.95f, 0.2f, 0.12f, 1f));
            WriteReadout();
        }

        void OnDied()
        {
            dead = true;
            flashUntil = 0f;
            Paint(new Color(0.12f, 0.12f, 0.12f, 1f));
            WriteReadout();
        }

        void EnsureSilhouette()
        {
            Renderer host = GetComponent<Renderer>();
            if (host != null)
            {
                host.enabled = false;
            }

            Transform existing = transform.Find("Silhouette");
            if (existing != null)
            {
                body = existing.GetComponentsInChildren<Renderer>();
                return;
            }

            GameObject root = new GameObject("Silhouette");
            root.transform.SetParent(transform, false);
            Vector3 scale = transform.localScale;
            root.transform.localScale = new Vector3(
                scale.x != 0f ? 1f / scale.x : 1f,
                scale.y != 0f ? 1f / scale.y : 1f,
                scale.z != 0f ? 1f / scale.z : 1f);
            float width = bulky ? 0.72f : 0.42f;
            Part(root.transform, "Legs", new Vector3(0f, -0.55f, 0f), new Vector3(width, 0.55f, 0.28f));
            Part(root.transform, "Torso", new Vector3(0f, 0.05f, 0f), new Vector3(width, 0.62f, 0.32f));
            Part(root.transform, "Head", new Vector3(0f, 0.58f, 0f), new Vector3(0.28f, 0.28f, 0.28f));
            body = root.GetComponentsInChildren<Renderer>();
            Paint(resting);
        }

        void Part(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(collider);
                }
                else
                {
                    DestroyImmediate(collider);
                }
            }

            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null && bodyMaterial != null)
            {
                renderer.sharedMaterial = bodyMaterial;
            }
        }

        void EnsureCanvas()
        {
            Transform existing = transform.Find("TargetReadout");
            if (existing != null)
            {
                canvas = existing;
                readout = existing.GetComponentInChildren<Text>();
                return;
            }

            GameObject root = new GameObject("TargetReadout", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            canvas = root.transform;
            Vector3 scale = transform.localScale;
            canvas.localPosition = new Vector3(0f, 0.72f, 0f);
            canvas.localScale = new Vector3(
                scale.x != 0f ? 0.004f / scale.x : 0.004f,
                scale.y != 0f ? 0.004f / scale.y : 0.004f,
                scale.z != 0f ? 0.004f / scale.z : 0.004f);
            Canvas board = root.AddComponent<Canvas>();
            board.renderMode = RenderMode.WorldSpace;
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(360f, 120f);
            Image back = root.AddComponent<Image>();
            back.sprite = White();
            back.color = new Color(0.03f, 0.03f, 0.04f, 0.72f);
            back.raycastTarget = false;
            GameObject textObject = new GameObject("Label", typeof(RectTransform));
            textObject.transform.SetParent(root.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 6f);
            textRect.offsetMax = new Vector2(-8f, -6f);
            readout = textObject.AddComponent<Text>();
            readout.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            readout.fontSize = 28;
            readout.alignment = TextAnchor.MiddleCenter;
            readout.color = Color.white;
            readout.raycastTarget = false;
            readout.horizontalOverflow = HorizontalWrapMode.Overflow;
            WriteReadout();
        }

        void WriteReadout()
        {
            if (readout == null)
            {
                return;
            }

            if (health == null)
            {
                readout.text = displayName;
                return;
            }

            if (dead || health.IsAlive == false)
            {
                readout.text = displayName + "\nmuerto";
                return;
            }

            float distance = 0f;
            if (view != null)
            {
                distance = Vector3.Distance(view.transform.position, transform.position);
            }

            readout.text = displayName
                + "\n"
                + health.Current.ToString("0")
                + " / "
                + health.Max.ToString("0")
                + "   "
                + distance.ToString("0")
                + " m";
        }

        void Paint(Color color)
        {
            if (body == null)
            {
                return;
            }

            for (int i = 0; i < body.Length; i++)
            {
                if (body[i] != null)
                {
                    body[i].material.color = color;
                }
            }
        }

        static Sprite white;

        static Sprite White()
        {
            if (white != null)
            {
                return white;
            }

            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            white = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return white;
        }
    }
}
