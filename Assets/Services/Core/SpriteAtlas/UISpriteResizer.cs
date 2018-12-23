using UnityEngine;
using UnityEngine.UI;

namespace Services.Core.Atlas
{
    [RequireComponent(typeof(Image))]
    public class UISpriteResizer : MonoBehaviour
    {
        public float width = -1f;
        public float height = -1f;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (width >= 0)
            {
                rectTransform.SetWidth(width);
            }

            if (height >= 0)
            {
                rectTransform.SetHeight(height);
            }
        }
    }
}
