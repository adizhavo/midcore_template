using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Gesture;
using Services.Game.SceneCamera;
using MergeWar.Data;

namespace MergeWar.Game.Systems
{
    public class PinchSystem : IInitializeSystem, IPinchHandler
    {
        [Inject] CameraService cameraService;
        [Inject] DataProviderSystem dataProvider;

        private Vector3 startPos;
        #if !UNITY_EDITOR
        private Vector3 firstScreenPos;
        private Vector3 secondScreenPos;
        #endif
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
            #if !UNITY_EDITOR
            this.firstScreenPos = firstScreenPos;
            this.secondScreenPos = secondScreenPos;
            #endif
            startPos = cameraService.activeCamera.ScreenToWorldPoint((firstScreenPos + secondScreenPos) / 2);
            return false;
        }

        public bool HandlePinch(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            #if UNITY_EDITOR
            int direction = Input.GetAxis("Mouse ScrollWheel") > 0 ? -1 : 1;
            var zoom = cameraService.zoom + direction * 0.3f;
            #else
            var firstDelta = (firstScreenPos - secondScreenPos).magnitude;
            var secondDelta = (this.firstScreenPos - this.secondScreenPos).magnitude;
            var delta = firstDelta - secondDelta;
            var zoom = cameraService.zoom - delta * gameConfig.cameraZoomSpeed * Time.unscaledDeltaTime;
            this.firstScreenPos = firstScreenPos;
            this.secondScreenPos = secondScreenPos;
            #endif
            zoom = Mathf.Clamp(zoom, gameConfig.cameraStretchedZoomRange.min, gameConfig.cameraStretchedZoomRange.max);
            cameraService.SetZoom(zoom);
            var cursorPos = cameraService.activeCamera.ScreenToWorldPoint((firstScreenPos + secondScreenPos) / 2);
            var deltaPos = startPos - cursorPos;
            cameraService.SetPosition(cameraService.position + deltaPos);
            return false;
        }

        public bool HandlePinchEnd()
        {
            if (cameraService.zoom < gameConfig.cameraZoomRange.min)
            {
                cameraService.LerpZoom(gameConfig.cameraZoomRange.min, 0.5f);
            }
            else if (cameraService.zoom > gameConfig.cameraZoomRange.max)
            {
                cameraService.LerpZoom(gameConfig.cameraZoomRange.max, 0.5f);
            }

            return false;
        }

        #endregion
    }
}