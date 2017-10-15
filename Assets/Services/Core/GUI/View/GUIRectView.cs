using UnityEngine;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class GUIRectView : MonoBehaviour, IGUIView
    {
        public string id;

        public RectTransform rectTransform
        {
            private set;
            get;
        }

        public void Init()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public string GetId()
        {
            return id;
        }
    }
}
