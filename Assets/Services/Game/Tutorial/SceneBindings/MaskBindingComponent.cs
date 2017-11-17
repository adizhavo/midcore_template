using UnityEngine;
using UnityEngine.UI;
using Services.Core;
using Services.Core.GUI;
using Services.Core.Databinding;
using Services.Core.Databinding.Components;
using System.Collections.Generic;

namespace Services.Game.Tutorial.Bindings
{
    public class MaskBindingComponent :
    MonoBindingComponent<Image, Services.Core.Rect>,
    BindingComponent<UIAnchor>,
    BindingComponent<KeyValuePair<string, string>>  
    {
        public Services.Core.Rect mask;

        protected override void Bind()
        {
            base.Bind();
            databinding.Bind<UIAnchor>(Constants.DATABINDING_TUTORIAL_MASK_ANCHOR_PATH, this);
            databinding.Bind<KeyValuePair<string, string>>(Constants.DATABINDING_TUTORIAL_MASK_VIEW_PATH, this);
        }

        #region BindingComponent implementation

        public override void OnValueChanged(string branch, Services.Core.Rect value)
        {
            mask = value;
        }

        #endregion

        #region BindingComponent implementation

        public void OnValueChanged(string branch, UIAnchor value)
        {
            if (mask != null)
            {
                var pos = Utils.GetScreenPos(mask, value);
                mask.x = pos.x;
                mask.y = pos.y;
            }
        }

        #endregion

        #region BindingComponent implementation

        public void OnValueChanged(string branch, KeyValuePair<string, string> value)
        {
            if (!string.IsNullOrEmpty(value.Key) && !string.IsNullOrEmpty(value.Value) )
            {
                var guiService = CoreServicesInstaller.Resolve<GUIService>();
                var rectTransform = guiService.GetPanelView(value.Key).GetView<GUIRectView>(value.Value).rectTransform;
                mask = rectTransform.rect.ToServiceRect();
                mask.x += rectTransform.position.x;
                mask.y += rectTransform.position.y;
            }
        }

        #endregion

        private void Update()
        {
            if (mask != null)
            {
                var x = mask.x / Screen.width;
                var y = mask.y / Screen.height;
                var width = x + mask.width / Screen.width;
                var height = y + mask.height / Screen.height;
                component.material.SetVector("_Mask", new Vector4(x, y, width, height));
            }
        }
    }
}
