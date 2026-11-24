using Meatgrinder.Core;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    [RequireComponent(typeof(LocalGameSession))]
    public sealed class SessionLoadingScreen : MonoBehaviour
    {
        [SerializeField] private Texture2D steelTexture;
        [SerializeField] private Texture2D oxideTexture;

        LocalGameSession session;
        Canvas canvas;
        RectTransform fill;
        UnityEngine.UI.Text destination;
        UnityEngine.UI.Text percent;

        void Awake()
        {
            session = GetComponent<LocalGameSession>();
            Build();
            Refresh();
        }

        void OnEnable()
        {
            session.Changed += Refresh;
            session.LoadingProgressChanged += UpdateProgress;
            Refresh();
        }

        void OnDisable()
        {
            session.Changed -= Refresh;
            session.LoadingProgressChanged -= UpdateProgress;
            canvas.enabled = false;
        }

        void Refresh()
        {
            canvas.enabled = session.Phase == LocalSessionPhase.Loading;
            if (!canvas.enabled) return;
            destination.text = session.LoadingDestination;
            UpdateProgress(session.LoadingProgress);
        }

        void UpdateProgress(float progress)
        {
            fill.anchorMax = new Vector2(progress, 1);
            percent.text = $"{Mathf.FloorToInt(progress * 100)}%";
        }

        void Build()
        {
            canvas = GameUi.Canvas(transform, "LoadingScreen", 300);
            Transform root = canvas.transform;
            GameUi.Image(GameUi.Fill(root, "Base"), new Color(.034f, .028f, .027f));
            var steel = GameUi.Fill(root, "BlackSteel");
            Texture(steel, steelTexture, new Color(.18f, .17f, .17f, .43f), new Rect(0, 0, 3, 2));

            var middle = new Vector2(.5f, .47f);
            var plate = GameUi.Rect(root, "OxidePlate", new Vector2(.83f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(560, 2000));
            plate.localRotation = Quaternion.Euler(0, 0, -24);
            Texture(plate, oxideTexture, new Color(.45f, .17f, .08f, .43f), new Rect(0, 0, 1, 4));
            GameUi.Image(GameUi.Rect(plate, "Edge", Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(9, 2000)), new Color(.82f, .34f, .14f, .52f));

            var title = GameUi.Text(root, "Wordmark", "MEATGRINDER", middle, new Vector2(.5f, .5f), new Vector2(0, 40),
                new Vector2(900, 150), 100, GameUi.Paper, TextAnchor.MiddleCenter, UiFontRole.Display);
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 48;
            title.resizeTextMaxSize = 100;
            GameUi.Image(GameUi.Rect(root, "BrandRule", middle, new Vector2(.5f, .5f), new Vector2(0, -47), new Vector2(68, 4)), GameUi.Accent);
            destination = GameUi.Text(root, "Destination", "", middle, new Vector2(.5f, .5f), new Vector2(0, -90),
                new Vector2(900, 60), 27, GameUi.Paper, TextAnchor.MiddleCenter, UiFontRole.Industrial);

            var track = GameUi.Rect(root, "ProgressTrack", new Vector2(.06f, 0), Vector2.zero, Vector2.zero, Vector2.zero);
            track.anchorMax = new Vector2(.94f, 0);
            track.offsetMin = new Vector2(0, 64);
            track.offsetMax = new Vector2(0, 69);
            GameUi.Image(track, new Color(.30f, .24f, .22f));
            fill = GameUi.Fill(track, "ProgressFill");
            GameUi.Image(fill, GameUi.Accent);
            GameUi.Text(root, "Status", "CARGANDO", new Vector2(.06f, 0), Vector2.zero, new Vector2(0, 76),
                new Vector2(240, 34), 19, GameUi.Paper, TextAnchor.MiddleLeft, UiFontRole.System);
            percent = GameUi.Text(root, "Percent", "0%", new Vector2(.94f, 0), Vector2.right, new Vector2(0, 76),
                new Vector2(120, 34), 19, GameUi.Paper, TextAnchor.MiddleRight, UiFontRole.System);
        }

        static void Texture(RectTransform rect, Texture2D texture, Color tint, Rect uv)
        {
            var image = rect.gameObject.AddComponent<UnityEngine.UI.RawImage>();
            image.texture = texture;
            image.color = tint;
            image.uvRect = uv;
            image.raycastTarget = false;
        }
    }
}
