using UnityEngine;

namespace Services.Core.Databinding.Components
{
    public class GameObjectToggleBindingComponent : MonoBehaviour, BindingComponent<bool>
    {
        #region BindingComponent implementation

        public void OnValueChanged(string branch, bool value)
        {
            gameObject.SetActive(value);
        }

        #endregion

    }
}