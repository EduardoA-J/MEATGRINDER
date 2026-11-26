using UnityEngine;
using UnityEngine.UI;

namespace Meatgrinder.Presentation
{
    public sealed class ZoneSign : MonoBehaviour
    {
        [SerializeField] private string title = "Zona";
        [SerializeField] private Color color = Color.white;

        Transform board;
        Camera view;

        void Awake()
        {
            if (transform.Find("Board") != null)
            {
                board = transform.Find("Board");
                var existingLabel = board.GetComponentInChildren<Text>();
                if (existingLabel != null) GameUi.StyleText(existingLabel, UiFontRole.Industrial);
                var existingBack = board.GetComponent<Image>();
                if (existingBack != null) existingBack.color = GameUi.Ink;
                return;
            }

            GameObject root = new GameObject("Board", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            board = root.transform;
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280f, 64f);
            rect.localScale = Vector3.one * 0.01f;
            Image back = root.AddComponent<Image>();
            back.color = GameUi.Ink;
            back.raycastTarget = false;
            Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            back.sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            GameObject textObject = new GameObject("Title", typeof(RectTransform));
            textObject.transform.SetParent(root.transform, false);
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            Text label = textObject.AddComponent<Text>();
            GameUi.StyleText(label, UiFontRole.Industrial);
            label.fontSize = 32;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 18;
            label.resizeTextMaxSize = 32;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = GameUi.Paper;
            label.text = title;
            label.raycastTarget = false;
        }

        void LateUpdate()
        {
            if (board == null)
            {
                return;
            }

            if (view == null)
            {
                view = Camera.main;
            }

            if (view == null)
            {
                return;
            }

            Vector3 toward = view.transform.position - board.position;
            if (toward.sqrMagnitude > 0.001f)
            {
                board.rotation = Quaternion.LookRotation(toward, Vector3.up);
            }
        }
    }
}
