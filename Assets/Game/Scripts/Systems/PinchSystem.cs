using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Gesture;
using Services.Game.SceneCamera;

namespace MergeWar
{
    public class PinchSystem : IInitializeSystem, IPinchHandler
    {
        [Inject] CameraService cameraService;
        [Inject] DataProviderSystem dataProvider;

        private Vector3 startPos;
        private Vector3 firstScreenPos;
        private Vector3 secondScreenPos;
        private GameConfig gameConfig;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            gameConfig = dataProvider.GetGameConfig();
        }

        #endregion

        #region IPinchHandler implementation

        public bool HandlePinchStart(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            this.firstScreenPos = firstScreenPos;
            this.secondScreenPos = secondScreenPos;
            startPos = cameraService.camera.ScreenToWorldPoint((firstScreenPos + secondScreenPos) / 2);
            return false;
        }

        public bool HandlePinch(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            #if UNITY_EDITOR
            int direction = Input.GetAxis("Mouse ScrollWheel") > 0 ? -1 : 1;
            var zoom = cameraService.zoom + direction * 0.3f;
            #else
            firstDelta = (this.firstScreenPos - firstScreenPos).magnitude;
            secondDelta = (this.secondScreenPos - secondScreenPos).magnitude;
            delta = firstScreenPos + secondScreenPos;
            var zoom = cameraService.zoom + delta * gameConfig.cameraZoomSpeed * Time.unscaledDeltaTime;
            #endif
            zoom = Mathf.Clamp(zoom, gameConfig.cameraZoomRange.min, gameConfig.cameraZoomRange.max);
            cameraService.SetZoom(zoom);
            var cursorPos = cameraService.camera.ScreenToWorldPoint((firstScreenPos + secondScreenPos) / 2);
            var deltaPos = startPos - cursorPos;
            cameraService.SetPosition(cameraService.position + deltaPos);
            return false;
        }

        public bool HandlePinchEnd(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            return false;
        }

        #endregion
    }
}