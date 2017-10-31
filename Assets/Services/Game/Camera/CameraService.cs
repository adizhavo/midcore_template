using Entitas;
using UnityEngine;
using System.Collections.Generic;

namespace Services.Game.SceneCamera
{
    /// <summary>
    /// Adapter to the scene camera
    /// Handler all aniamtions and setup
    /// </summary>

    public class CameraService : IInitializeSystem
    {
        public Camera camera
        {
            private set;
            get;
        }

        public Vector3 position
        {
            get { return camera.transform.position; }
        }

        private LTDescr zoomAnim;
        private LTDescr posAnim;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            camera = Camera.main;
        }

        #endregion

        public void SetZoom(float zoom)
        {
            camera.orthographicSize = zoom;
        }

        public void LerpZoom(float zoom, float duration = 0.3f)
        {
            if (zoomAnim != null)
            {
                LeanTween.cancel(zoomAnim.uniqueId);
            }

            zoomAnim = LeanTween.value(camera.orthographicSize, zoom, duration).setOnUpdate(
                (value) => camera.orthographicSize = value
            );
        }

        public void SetPosition(Vector3 position)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
                posAnim = null;
            }

            camera.transform.position = position;
        }

        public void LerpPosition(Vector3 position, float duration = 0.3f)
        {
            if (posAnim != null)
            {
                LeanTween.cancel(posAnim.uniqueId);
            }

            posAnim = LeanTween.move(camera.gameObject, position, duration);
        }
    }
}