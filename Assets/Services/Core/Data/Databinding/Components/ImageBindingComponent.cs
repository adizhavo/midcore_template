using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services.Core;

namespace Services.Core.Databinding.Components
{
    public class ImageBindingComponent : MonoBindingComponent<Image, string>
    {
        #region BindingComponent implementation

        public override void OnValueChanged(string branch, string value)
        {
            component.sprite = Utils.ReadFromResources<Sprite>(value);
        }

        #endregion

    }
}