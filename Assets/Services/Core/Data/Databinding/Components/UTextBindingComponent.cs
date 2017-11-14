using UnityEngine;
using UnityEngine.UI;

namespace Services.Core.Databinding.Components
{
    public class UTextBindingComponent : MonoBindingComponent<Text, string>
    {
        #region BindingComponent implementation

        public override void OnValueChanged(string branch, string value)
        {
            component.text = value;
        }

        #endregion

    }
}
