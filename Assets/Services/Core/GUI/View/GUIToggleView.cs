using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(Toggle))]
    public class GUIToggleView : GUIRectView
    {
        public Toggle toggle
        {
            private set;
            get;
        }

        public override void Init()
        {
            base.Init();
            toggle = GetComponent<Toggle>();
        }

        public void AddListener(UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }
    }
}