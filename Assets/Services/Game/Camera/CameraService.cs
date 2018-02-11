using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Databinding;
using System.Collections.Generic;
using Services.Core;

namespace Services.Game.SceneCamera
{
    /// <summary>
    /// Adapter to the scene camera
    /// Handler all aniamtions and setup
    /// </summary>

    public class CameraService : IExecuteSystem
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

        public float boundaryRadius
        {
            private set;
            get;
        }

        private LTDescr zoomAnim;
        private LTDescr posAnim;
        private Vector3 boundaryCenter;

        public void SetBoundary(Vector3 center, float radius)
        {
            this.boundaryCenter = center;
            this.boundaryRadius = radius;
        }

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

			zoomAnim = LeanTween.value(this.zoom, zoom, duration).setOnUpdate(
                (float value) => databinding.AddData(Constants.DATABINDING_CAMERA_ZOOM, value, true)
            )
			.setOnComplete(()=> zoomAnim = null)
			.setEaseOutExpo();
        }

        public void SetPosition(Vector3 position)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
                posAnim = null;
            }

            databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, ClampPosition(position), true);
        }

        public void LerpPosition(Vector3 position, float duration = 0.3f)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
            }

            posAnim = LeanTween.value(activeCamera.gameObject, activeCamera.transform.position, position, duration).setOnUpdate(
                (Vector3 value) => databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, ClampPosition(value), true)
            )
            .setOnComplete(()=> posAnim = null)
            .setEaseInOutQuad();
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            var distanceVector = boundaryCenter - position;
            if (distanceVector.sqrMagnitude > Mathf.Pow(boundaryRadius, 2))
            {
                distanceVector = Vector3.ClampMagnitude(distanceVector, boundaryRadius);
                position = boundaryCenter - distanceVector;
            }

            return position;
        }

        #region IExecuteSystem implementation

        public void Execute()
        {
            #if UNITY_EDITOR
            Utils.DrawEllipse(boundaryCenter, activeCamera.transform.forward, activeCamera.transform.up, boundaryRadius, boundaryRadius, 180, Color.cyan, 0f);
            #endif
        }

        #endregion
    }
}
