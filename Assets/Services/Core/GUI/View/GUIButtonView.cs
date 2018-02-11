using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(Button))]
    public class GUIButtonView : GUIRectView
    {
        public Button button
        {
            private set;
            get;
        }

        public override void Init()
        {
            base.Init();
            button = GetComponent<Button>();
        }

        public void AddListener(UnityAction callback)
        {
            button.onClick.AddListener(callback);
        }
    }
}