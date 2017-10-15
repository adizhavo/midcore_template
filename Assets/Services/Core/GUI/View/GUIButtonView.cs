using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Services.Core.GUI
{
    [RequireComponent(typeof(Button))]
    public class GUIButtonView : MonoBehaviour, IGUIView
    {
        public string id;

        public Button button
        {
            private set;
            get;
        }

        public void Init()
        {
            button = GetComponent<Button>();
        }

        public string GetId()
        {
            return id;
        }

        public void AddListener(UnityAction callback)
        {
            button.onClick.AddListener(callback);
        }
    }
}