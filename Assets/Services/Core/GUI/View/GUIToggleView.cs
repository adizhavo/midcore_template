using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(Toggle))]
    public class GUIToggleView : MonoBehaviour, IGUIView
    {
        public string id;

        public Toggle toggle
        {
            private set;
            get;
        }

        public void Init()
        {
            toggle = GetComponent<Toggle>();
        }

        public string GetId()
        {
            return id;
        }

        public void AddListener(UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }
    }
}