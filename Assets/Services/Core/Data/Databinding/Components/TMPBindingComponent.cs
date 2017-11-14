using TMPro;
using UnityEngine;

namespace Services.Core.Databinding.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPBindingComponent : MonoBehaviour, BindingComponent<string>
    {
        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        #region BindingComponent implementation

        public void OnValueChanged(string branch, string value)
        {
            text.text = value;
        }

        #endregion
        
    }
}