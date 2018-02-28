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
            component.material = new Material(Shader.Find("Tutorial/UIMask"));
        }

        #region BindingComponent implementation

        public override void OnValueChanged(string branch, Services.Core.Rect value)
        {
            mask = value;
            if (mask != null)
            {
                mask.width *= Utils.GetUIScaleFactor();
            }
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
                var rectTransform = guiService.GetPanelView(value.Key).GetView<MonoBehaviour>(value.Value).GetComponent<RectTransform>();
                mask = rectTransform.rect.ToServiceRect();
                mask.width *= Utils.GetUIScaleFactor(); 
                mask.x = -mask.width / 2f;
                mask.x += rectTransform.position.x / Screen.width * Constants.SCREEN_WIDTH;
                mask.y += rectTransform.position.y / Screen.height * Constants.SCREEN_HEIGHT;
            }
        }

        #endregion

        private void Update()
        {
            if (mask != null)
            {
                var x = mask.x / Constants.SCREEN_WIDTH;
                var y = mask.y / Constants.SCREEN_HEIGHT;
                var width = x + mask.width / Constants.SCREEN_WIDTH;
                var height = y + mask.height / Constants.SCREEN_HEIGHT;
                component.material.SetVector("_Mask", new Vector4(x, y, width, height));
            }
        }
    }
}
