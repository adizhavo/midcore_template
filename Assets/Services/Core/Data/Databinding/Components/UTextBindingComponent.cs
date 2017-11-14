using UnityEngine;
using UnityEngine.UI;

namespace Services.Core.Databinding.Components
{
    [RequireComponent(typeof(Text))]
    public class UTextBindingComponent : MonoBehaviour, BindingComponent<string>
    {
        private Text text;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        #region BindingComponent implementation

        public void OnValueChanged(string branch, string value)
        {
            text.text = value;
        }

        #endregion

    }
}
