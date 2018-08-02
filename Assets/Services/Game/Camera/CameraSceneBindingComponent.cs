using UnityEngine;
using Services.Core.Databinding;
using Services.Core;

namespace Services.Game.SceneCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraSceneBindingComponent : MonoBehaviour, BindingComponent<float>, BindingComponent<Vector3>, BindingComponent<bool>
    {
        public string cameraInstancePath;

        private DataBindingService dataBinding;

        public Camera cameraComp
        {
            private set;
            get;
        }

        private void Awake()
        {
            cameraComp = GetComponent<Camera>();
        }

        private void Start()
        {
            dataBinding = CoreServicesInstaller.Resolve<DataBindingService>();
            dataBinding.Bind<float>(Constants.DATABINDING_CAMERA_ZOOM, this);
            dataBinding.Bind<Vector3>(Constants.DATABINDING_CAMERA_POSITON, this);
            dataBinding.Bind<bool>(cameraInstancePath, this);
        }

        #region BindingComponent implementation

        public void OnValueChanged(string branch, float value)
        {
            cameraComp.orthographicSize = value;
            cameraComp.fieldOfView = value;
        }

        public void OnValueChanged(string branch, Vector3 value)
        {
            transform.position = value;
        }

        public void OnValueChanged(string branch, bool value)
        {
            gameObject.SetActive(value);

            if (value)
            {
                dataBinding.AddData<Camera>(Constants.DATABINDING_CAMERA_ACTIVE, cameraComp, true);
            }
        }

        #endregion
    }
}