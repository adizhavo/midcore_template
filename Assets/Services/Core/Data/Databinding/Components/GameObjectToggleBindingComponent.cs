using UnityEngine;

namespace Services.Core.Databinding.Components
{
    public class GameObjectToggleBindingComponent : MonoBindingComponent<Transform, bool>
    {
        #region BindingComponent implementation

        public override void OnValueChanged(string branch, bool value)
        {
            component.gameObject.SetActive(value);
        }

        #endregion

    }
}