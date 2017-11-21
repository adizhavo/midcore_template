using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Databinding;
using System.Collections.Generic;

namespace Services.Game.SceneCamera
{
    /// <summary>
    /// Adapter to the scene camera
    /// Handler all aniamtions and setup
    /// </summary>

    public class CameraService
    {
        [Inject] DataBindingService databinding;

        public Camera activeCamera
        {
            get{ return databinding.GetData<Camera>(Constants.DATABINDING_CAMERA_ACTIVE).value; }
        }

        public Vector3 position
        {
            get { return databinding.GetData<Vector3>(Constants.DATABINDING_CAMERA_POSITON).value; }
        }

        public float zoom
        {
            get { return databinding.GetData<float>(Constants.DATABINDING_CAMERA_ZOOM).value; }
        }

        private LTDescr zoomAnim;
        private LTDescr posAnim;

        public void SetZoom(float zoom)
        {
            if (zoomAnim != null)
            {
                LeanTween.cancel(zoomAnim.uniqueId);
                zoomAnim = null;
            }

            databinding.AddData(Constants.DATABINDING_CAMERA_ZOOM, zoom, true);
        }

        public void LerpZoom(float zoom, float duration = 0.3f)
        {
            if (zoomAnim != null)
            {
                LeanTween.cancel(zoomAnim.uniqueId);
            }

            zoomAnim = LeanTween.value(activeCamera.orthographicSize, zoom, duration).setOnUpdate(
                (float value) => databinding.AddData(Constants.DATABINDING_CAMERA_ZOOM, value, true)
            ).setEaseOutExpo();
        }

        public void SetPosition(Vector3 position)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
                posAnim = null;
            }

            databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, position, true);
        }

        public void LerpPosition(Vector3 position, float duration = 0.3f)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
            }

            posAnim = LeanTween.value(activeCamera.gameObject, activeCamera.transform.position, position, duration).setOnUpdate(
                (Vector3 value) => databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, value, true)
            ).setEaseOutExpo();
        }
    }
}