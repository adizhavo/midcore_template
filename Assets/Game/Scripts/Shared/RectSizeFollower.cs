using Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MidcoreTemplate.Game.Utilities
{
    [ExecuteInEditMode]
    public class RectSizeFollower : MonoBehaviour
    {
        public RectTransform selectedSize;
        public RectTransform followSize;

        private void Update()
        {
            if (followSize != null && selectedSize != null)
            {
                selectedSize.SetWidth(followSize.GetWidth());
                selectedSize.SetHeight(followSize.GetHeight());
            }
        }
    }
}
