﻿using UnityEngine;

namespace Services.Core.Databinding.Components
{
    [RequireComponent(typeof(T))]
    public abstract class MonoBindingComponent<T, U> : MonoBehaviour, BindingComponent<U>
    {
        public string path;
        protected T component;

        protected DataBindingService databinding; 

        protected bool binded = false;

        protected virtual void Awake()
        {
            component = GetComponent<T>();
            databinding = CoreServicesInstaller.Resolve<DataBindingService>();
        }

        protected virtual void Start()
        {
            if (!binded)
            {
                var isDataValid = databinding.GetData<U>(path) != null;
                if (isDataValid)
                {
                    databinding.Bind(path, this);
                    binded = true;
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (!binded)
            {
                var isDataValid = databinding.GetData<object>(path) != null;
                if (isDataValid)
                {
                    databinding.Bind(path, this);
                    binded = true;
                }
            }
        }

        #region BindingComponent implementation

        public abstract void OnValueChanged(string branch, U value);

        #endregion
    }
}