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
    /// Handler all animations and setup
    /// </summary>

    public class CameraService : IInitializeSystem, IExecuteSystem, BindingComponent<Camera>
    {
        [Inject] DataBindingService databinding;

        public Camera activeCamera
        {
            private set;
            get;
        }
        
        public Vector3 position
        {
            private set;
            get;
        }
        
        public float zoom
        {
            private set;
            get;
        }

        public float boundaryRadius 
        {
            private set;
            get;
        }
        
        public bool lockCamera;
        public bool ignoreBoundaries;

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
                LeanTween.cancel(zoomAnim.uniqueId, true);
                zoomAnim = null;
            }

            this.zoom = zoom;
            databinding.AddData(Constants.DATABINDING_CAMERA_ZOOM, zoom, true);
        }

        public void LerpZoom(float zoom, float duration = 0.3f)
        {
            if (lockCamera)
                return;
            
            if (zoomAnim != null)
            {
                LeanTween.cancel(zoomAnim.uniqueId, true);
            }

			zoomAnim = LeanTween.value(this.zoom, zoom, duration).setOnUpdate(
                (float value) =>
                {
                    this.zoom = value;
                    databinding.AddData(Constants.DATABINDING_CAMERA_ZOOM, value, true);
                }).setEaseOutExpo()
			.setIgnoreTimeScale(true)
            .setOnComplete(() => zoomAnim = null);
        }

        public void SetPosition(Vector3 position)
        {
            if (lockCamera)
                return;
            
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId, true);
                posAnim = null;
            }

            this.position = ClampPosition(position);
            databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, this.position, true);
        }

        public void LerpPosition(Vector3 position, float duration = 0.3f)
        {
            if (lockCamera)
                return;
            
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId, true);
            }

            posAnim = LeanTween.value(activeCamera.gameObject, activeCamera.transform.position, position, duration).setOnUpdate(
                (Vector3 value) =>
                {
                    this.position = ClampPosition(position);
                    databinding.AddData(Constants.DATABINDING_CAMERA_POSITON, this.position, true);
                }).setEaseInOutQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => posAnim = null);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            #if !FREE_CAMERA
            if (!ignoreBoundaries)
            {
                var distanceVector = boundaryCenter - position;
                if (distanceVector.sqrMagnitude > Mathf.Pow(boundaryRadius, 2))
                {
                    distanceVector = Vector3.ClampMagnitude(distanceVector, boundaryRadius);
                    position = boundaryCenter - distanceVector;
                }
            }
            #endif
            return position;
        }

        #region IExecuteSystem implementation

        public void Execute()
        {
            #if UNITY_EDITOR
            if (activeCamera != null)
                Utils.DrawEllipse(boundaryCenter, activeCamera.transform.forward, activeCamera.transform.up, boundaryRadius, boundaryRadius, 180, Color.cyan, 0f);
            #endif
        }

        #endregion
        
        public void Initialize()
        {
            databinding.Bind<Camera>(Constants.DATABINDING_CAMERA_ACTIVE, this);
        }

        public void OnValueChanged(string branch, Camera value)
        {
            activeCamera = value;
        }
    }
}