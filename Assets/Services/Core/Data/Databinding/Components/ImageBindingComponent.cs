using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services.Core;

namespace Services.Core.Databinding.Components
{
    public class ImageBindingComponent : MonoBindingComponent<Image, string>
    {
        public bool setNativeSize;

        #region BindingComponent implementation

        public override void OnValueChanged(string branch, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                component.sprite = Utils.ReadFromResources<Sprite>(value);
                component.enabled = true;

                if (setNativeSize) component.SetNativeSize();
            }
            else
            {
                component.enabled = false;
            }
        }

        #endregion

    }
}