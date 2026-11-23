using UnityEngine;
using UnityEngine.Events;

namespace Meatgrinder.Presentation
{
    public static class GameUi
    {
        public static readonly Color Ink = new Color(.035f, .041f, .047f, .96f);
        public static readonly Color Paper = new Color(.9f, .88f, .8f);
        public static readonly Color Accent = new Color(1f, .39f, .12f);
        public static RectTransform Rect(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = anchor; rect.pivot = pivot;
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }
        public static RectTransform Fill(Transform parent, string name)
        {
            var rect = Rect(parent, name, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
            rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            return rect;
        }
        public static UnityEngine.UI.Image Image(RectTransform rect, Color color)
        {
            var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            image.color = color; image.raycastTarget = false;
            return image;
        }
        public static UnityEngine.UI.Text Text(Transform parent, string name, string value, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var rect = Rect(parent, name, anchor, pivot, position, size);
            var text = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value; text.fontSize = fontSize; text.color = color; text.alignment = alignment;
            text.raycastTarget = false; text.supportRichText = true;
            return text;
        }
        public static UnityEngine.UI.Button Button(Transform parent, string name, string caption, Vector2 position, Vector2 size, UnityAction action)
        {
            var rect = Rect(parent, name, new Vector2(0, 1), new Vector2(0, 1), position, size);
            var image = Image(rect, new Color(.12f, .13f, .14f, .95f)); image.raycastTarget = true;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = image;
            var colors = button.colors; colors.normalColor = Color.white; colors.highlightedColor = new Color(1f, .64f, .38f);
            colors.selectedColor = colors.highlightedColor; colors.pressedColor = Accent; colors.disabledColor = new Color(.35f, .35f, .35f); button.colors = colors;
            Text(rect, "Caption", caption, Vector2.zero, Vector2.zero, new Vector2(20, 0), size - new Vector2(30, 0), 23, Paper);
            button.onClick.AddListener(action);
            return button;
        }
        public static Canvas Canvas(Transform parent, string name, int order)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler));
            root.transform.SetParent(parent, false);
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = order;
            var scaler = root.GetComponent<UnityEngine.UI.CanvasScaler>(); scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900); scaler.matchWidthOrHeight = .5f;
            return canvas;
        }
    }
}
