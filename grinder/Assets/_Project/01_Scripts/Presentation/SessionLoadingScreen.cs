using Meatgrinder.Core;
using UnityEngine;

namespace Meatgrinder.Presentation
{
    [RequireComponent(typeof(LocalGameSession))]
    public sealed class SessionLoadingScreen : MonoBehaviour
    {
        [SerializeField] private Texture2D steelTexture;
        [SerializeField] private Texture2D oxideTexture;
        [SerializeField] private Texture2D menuBackground;
        ScreenBackground background;

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
            background.Texture = session.LoadingMap != null ? session.LoadingMap.LoadingBackground : menuBackground;
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
            GameUi.Image(GameUi.Fill(root, "Base"), GameUi.Ink);
            var steel = GameUi.Fill(root, "BlackSteel");
            Texture(steel, steelTexture, GameUi.Charcoal, new Rect(0, 0, 3, 2));

            var middle = new Vector2(.5f, .47f);
            var plate = GameUi.Rect(root, "OxidePlate", new Vector2(.83f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(560, 2000));
            plate.localRotation = Quaternion.Euler(0, 0, -24);
            Texture(plate, oxideTexture, GameUi.Blood, new Rect(0, 0, 1, 4));
            GameUi.Image(GameUi.Rect(plate, "Edge", Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(9, 2000)), GameUi.Accent);

            background = ScreenBackground.Create(root, menuBackground);
            var nameplate = GameUi.Rect(root, "DestinationPlate", middle, new Vector2(.5f, .5f), new Vector2(0, -90), new Vector2(920, 74));
            GameUi.Panel(nameplate).color = GameUi.Ink;

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
            track.offsetMax = new Vector2(0, 80);
            GameUi.Panel(track, false).color = GameUi.Ink;
            fill = GameUi.Fill(track, "ProgressFill");
            GameUi.Image(fill, GameUi.Accent);
            for (int i = 1; i < 40; i++)
                GameUi.Image(GameUi.Rect(track, "Division", new Vector2(i / 40f, 0), Vector2.zero, Vector2.zero, new Vector2(3, 16)), GameUi.Ink);
            GameUi.Text(root, "Status", "CARGANDO", new Vector2(.06f, 0), Vector2.zero, new Vector2(0, 90),
                new Vector2(240, 34), 19, GameUi.Paper, TextAnchor.MiddleLeft, UiFontRole.System);
            percent = GameUi.Text(root, "Percent", "0%", new Vector2(.94f, 0), Vector2.right, new Vector2(0, 90),
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
