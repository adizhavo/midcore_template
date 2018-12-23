using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services.Core;
using Services.Core.Atlas;

namespace Services.Core.Databinding.Components
{
    public class ImageBindingComponent : MonoBindingComponent<Image, string>
    {
        public bool setNativeSize;

        private SpriteAtlasService spriteAtlas;

        protected override void Awake()
        {
            base.Awake();
            spriteAtlas = CoreServicesInstaller.Resolve<SpriteAtlasService>();
        }

        #region BindingComponent implementation

        public override void OnValueChanged(string branch, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                component.sprite = spriteAtlas.GetSprite(value);
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