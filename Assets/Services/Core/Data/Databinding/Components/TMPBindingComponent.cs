using TMPro;
using UnityEngine;

namespace Services.Core.Databinding.Components
{
    public class TMPBindingComponent : MonoBindingComponent<TextMeshProUGUI, string>
    {
        #region BindingComponent implementation

        public override void OnValueChanged(string branch, string value)
        {
            component.text = value;
        }

        #endregion
        
    }
}