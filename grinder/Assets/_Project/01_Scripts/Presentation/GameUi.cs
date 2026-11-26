using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meatgrinder.Presentation
{
    public enum UiFontRole { Body, System, Industrial, Display }

    public static class GameUi
    {
        public static readonly Color Ink = new Color(.035f, .035f, .035f, .98f);
        public static readonly Color Paper = new Color(.9f, .88f, .82f);
        public static readonly Color Accent = new Color(.72f, .035f, .035f);
        public static readonly Color Blood = new Color(.25f, .025f, .025f);
        public static readonly Color Charcoal = new Color(.11f, .11f, .11f);
        public static readonly Color Metal = new Color(.32f, .32f, .32f);
        public static readonly Color Muted = new Color(.66f, .66f, .64f);
        public static readonly Color Hover = new Color(.20f, .075f, .075f);
        public static readonly Color Pressed = new Color(.40f, .025f, .025f);
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
                    _ => "BarlowSemiCondensed-SemiBold"
                };
                fonts[index] = Resources.Load<Font>("MeatgrinderFonts/" + file);
                if (fonts[index] == null) Debug.LogError("Missing MEATGRINDER UI font: " + file);
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
        public static UnityEngine.UI.Image Panel(RectTransform rect, bool rivets = true)
        {
            var face = Image(rect, Charcoal);
            Frame(rect, rivets);
            return face;
        }
        public static void Frame(RectTransform rect, bool rivets = false)
        {
            Edge(rect, "BevelTop", new Vector2(0, 1), Vector2.one, new Vector2(0, -2), Vector2.zero, Metal);
            Edge(rect, "BevelBottom", Vector2.zero, Vector2.right, Vector2.zero, new Vector2(0, 3), Ink);
            Edge(rect, "BevelLeft", Vector2.zero, Vector2.up, Vector2.zero, new Vector2(2, 0), Metal);
            Edge(rect, "BevelRight", Vector2.right, Vector2.one, new Vector2(-3, 0), Vector2.zero, Ink);
            if (!rivets) return;
            foreach (var corner in new[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one })
            {
                var bolt = Rect(rect, "Rivet", corner, new Vector2(.5f, .5f), new Vector2(corner.x == 0 ? 7 : -7, corner.y == 0 ? 7 : -7), new Vector2(5, 5));
                Image(bolt, Metal);
                Image(Rect(bolt, "Slot", new Vector2(.5f, .5f), new Vector2(.5f, .5f), Vector2.zero, new Vector2(3, 1)), Ink);
            }
        }
        static void Edge(Transform parent, string name, Vector2 min, Vector2 max, Vector2 insetMin, Vector2 insetMax, Color color)
        {
            var edge = Fill(parent, name); edge.anchorMin = min; edge.anchorMax = max;
            edge.offsetMin = insetMin; edge.offsetMax = insetMax;
            Image(edge, color);
        }
        public static UnityEngine.UI.Image Meter(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            var track = Rect(parent, name + "Track", Vector2.zero, Vector2.zero, position - Vector2.one * 2, size + Vector2.one * 4);
            Image(track, Ink);
            var fill = Image(Rect(parent, name, Vector2.zero, Vector2.zero, position, size), color);
            for (int i = 1; i < 12; i++)
                Image(Rect(parent, name + "Division", Vector2.zero, Vector2.zero, position + Vector2.right * (size.x * i / 12), new Vector2(2, size.y)), Ink);
            return fill;
        }
        public static void StyleText(UnityEngine.UI.Text text, UiFontRole role)
        {
            text.font = FontFor(role);
            text.color = Paper;
            text.raycastTarget = false;
            var shadow = text.GetComponent<UnityEngine.UI.Shadow>() ?? text.gameObject.AddComponent<UnityEngine.UI.Shadow>();
            shadow.effectColor = Ink; shadow.effectDistance = new Vector2(1, -2);
        }
        public static UnityEngine.UI.Text Text(Transform parent, string name, string value, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size, int fontSize, Color color, TextAnchor alignment = TextAnchor.MiddleLeft, UiFontRole role = UiFontRole.Body)
        {
            var rect = Rect(parent, name, anchor, pivot, position, size);
            var text = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
            StyleText(text, role);
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
            Frame(face.rectTransform);
            var rail = Image(Rect(face.transform, "FocusRail", Vector2.zero, Vector2.zero, Vector2.zero, new Vector2(4, size.y)), Accent);
            var label = Text(face.transform, "Caption", caption, Vector2.zero, Vector2.zero, new Vector2(22, 0),
                size - new Vector2(48, 0), 25, Paper, role: UiFontRole.Industrial);
            label.resizeTextForBestFit = true; label.resizeTextMinSize = 18; label.resizeTextMaxSize = 25;
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
            scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            return canvas;
        }
    }
}
