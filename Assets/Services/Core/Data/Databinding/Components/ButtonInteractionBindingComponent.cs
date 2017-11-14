using UnityEngine;
using UnityEngine.UI;

namespace Services.Core.Databinding.Components
{
    public class ButtonInteractionBindingComponent : MonoBindingComponent<Button, bool>
    {
        #region BindingComponent implementation

        public override void OnValueChanged(string branch, bool value)
        {
            component.interactable = value;
        }

        #endregion
    }
}