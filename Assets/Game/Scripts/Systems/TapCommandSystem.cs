using Zenject;
using UnityEngine;
using Services.Core.Gesture;
using Services.Core.Data;
using Services.Game.SceneCamera;
using MergeWar.Data;
using MergeWar.Game.Utilities;

namespace MergeWar.Game.Systems
{
    public class TapCommandSystem : ITouchHandler
    {
        [Inject] CommandSystem commandSystem;
        [Inject] SceneSystem sceneSystem;
        [Inject] CameraService cameraService;
        [Inject] DatabaseService databaseService;

        #region ITouchHandler implementation

        public bool HandleTouchDown(Vector3 screenPos) { return false; }

        public bool HandleTouchUp(Vector3 screenPos)
        {
            var touched = Utils.GetInputTarget(screenPos, sceneSystem, cameraService.camera);
            if (touched != null)
            {
                var objectData = databaseService.Get<GameGridObjectData>(touched.objectId);
                var cell = touched.hasGrid ? touched.grid.pivot : null;
                commandSystem.Execute(objectData.onTapCommand, touched.position, cell, touched);
            }
            return false;
        }

        public bool HandleDoubleTouch(Vector3 screenPos) { return false; }

        #endregion
    }
}