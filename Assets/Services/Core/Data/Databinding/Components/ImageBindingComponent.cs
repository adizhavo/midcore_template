using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services.Core;

namespace Services.Core.Databinding.Components
{
    [RequireComponent(typeof(Image))]
    public class ImageBindingComponent : MonoBehaviour, BindingComponent<string>
    {
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        #region BindingComponent implementation

        public void OnValueChanged(string branch, string value)
        {
            image.sprite = Utils.ReadFromResources<Sprite>(value);
        }

        #endregion

    }
}