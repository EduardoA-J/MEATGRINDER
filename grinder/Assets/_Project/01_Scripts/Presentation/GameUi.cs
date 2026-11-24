using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meatgrinder.Presentation
{
    public enum UiFontRole { Body, System, Industrial, Display }

    public static class GameUi
    {
        public static readonly Color Ink = new Color(.035f, .041f, .047f, .96f);
        public static readonly Color Paper = new Color(.9f, .88f, .8f);
        public static readonly Color Accent = new Color(1f, .39f, .12f);
        static readonly Font[] fonts = new Font[4];

        public static Font FontFor(UiFontRole role)
        {
            int index = (int)role;
            if (fonts[index] == null)
            {
                string file = role switch
                {
                    UiFontRole.System => "IBMPlexMono-Medium",
                    UiFontRole.Industrial => "BarlowSemiCondensed-SemiBold",
                    UiFontRole.Display => "MetalMania-Regular",
                    _ => "BarlowSemiCondensed-Regular"
                };
                fonts[index] = Resources.Load<Font>("MeatgrinderFonts/" + file);
                if (fonts[index] == null) fonts[index] = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return fonts[index];
        }

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
        public static UnityEngine.UI.Text Text(Transform parent, string name, string value, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment = TextAnchor.MiddleLeft, UiFontRole role = UiFontRole.Body)
        {
            var rect = Rect(parent, name, anchor, pivot, position, size);
            var text = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
            text.font = FontFor(role);
            text.text = value; text.fontSize = fontSize; text.color = color; text.alignment = alignment;
            text.raycastTarget = false; text.supportRichText = true;
            return text;
        }
        public static UnityEngine.UI.Button Button(Transform parent, string name, string caption, Vector2 position, Vector2 size, UnityAction action,
            Func<bool> reduceMotion = null, Action onFocus = null)
        {
            var rect = Rect(parent, name, new Vector2(0, 1), new Vector2(0, 1), position, size);
            var hitArea = Image(rect, Color.clear);
            hitArea.raycastTarget = true;
            hitArea.canvasRenderer.cullTransparentMesh = false;
            var face = Image(Fill(rect, "Face"), Ink);
            var rail = Image(Rect(face.transform, "FocusRail", Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(4, size.y)), Accent);
            var label = Text(face.transform, "Caption", caption, Vector2.zero, Vector2.zero, new Vector2(22, 0),
                size - new Vector2(36, 0), 25, Paper, role: UiFontRole.Industrial);
            var button = rect.gameObject.AddComponent<MenuButton>();
            button.targetGraphic = hitArea;
            button.transition = UnityEngine.UI.Selectable.Transition.None;
            button.Configure(face, rail, label, reduceMotion, onFocus);
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
