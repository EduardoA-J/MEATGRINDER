using UnityEngine;

namespace Meatgrinder.Presentation
{
    // Center-crop to cover the screen, keeping the original image proportions at any resolution.
    [RequireComponent(typeof(UnityEngine.UI.RawImage))]
    public sealed class ScreenBackground : MonoBehaviour
    {
        UnityEngine.UI.RawImage image;
        public Texture2D Texture
        {
            set
            {
                if (image == null) image = GetComponent<UnityEngine.UI.RawImage>();
                image.texture = value;
                image.enabled = value != null;
                Refresh();
            }
        }
        void LateUpdate() => Refresh();
        void Refresh()
        {
            if (image == null || image.texture == null) return;
            var size = image.rectTransform.rect.size;
            if (size.x <= 0 || size.y <= 0) return;
            float ratio = size.x / size.y / ((float)image.texture.width / image.texture.height);
            image.uvRect = ratio >= 1 ? new Rect(0, (1 - 1 / ratio) * .5f, 1, 1 / ratio)
                : new Rect((1 - ratio) * .5f, 0, ratio, 1);
        }
        public static ScreenBackground Create(Transform parent, Texture2D texture)
        {
            var rect = GameUi.Fill(parent, "Artwork");
            var image = rect.gameObject.AddComponent<UnityEngine.UI.RawImage>();
            image.raycastTarget = false;
            var background = rect.gameObject.AddComponent<ScreenBackground>();
            background.Texture = texture;
            return background;
        }
    }
}
