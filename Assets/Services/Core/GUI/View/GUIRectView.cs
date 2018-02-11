using UnityEngine;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(RectTransform))]
    public class GUIRectView : MonoBehaviour, IGUIView
    {
        public string id;
        public bool enableOnAwake = true;

        public bool EnableOnAwake
        {
            get { return enableOnAwake; }
        }

        public RectTransform rectTransform
        {
            private set;
            get;
        }

        public virtual void Init()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public string GetId()
        {
            return id;
        }
    }
}
