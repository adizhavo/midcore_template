using UnityEngine;

namespace Services.Core.Databinding.Components
{
    [RequireComponent(typeof(T))]
    public abstract class MonoBindingComponent<T, U> : MonoBehaviour, BindingComponent<U>
    {
        public string path;
        protected T component;

        protected virtual void Awake()
        {
            component = GetComponent<T>();
        }

        #region BindingComponent implementation

        public abstract void OnValueChanged(string branch, U value);

        #endregion
    }
}