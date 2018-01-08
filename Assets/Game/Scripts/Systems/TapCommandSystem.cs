using Zenject;
using UnityEngine;
using Services.Core.Gesture;
using Services.Core.Data;
using Services.Core.Event;
using Services.Game.Grid;
using Services.Game.SceneCamera;
using MergeWar.Data;
using Services.Game.Data;
using MergeWar.Game.Utilities;

namespace MergeWar.Game.Systems
{
    public class TapCommandSystem : ITouchHandler
    {
        [Inject] CommandSystem commandSystem;
        [Inject] SceneSystem sceneSystem;
        [Inject] CameraService cameraService;
        [Inject] DatabaseService databaseService;
        [Inject] GridService gridService;

        #region ITouchHandler implementation

        public bool HandleTouchDown(Vector3 screenPos) { return false; }

        public bool HandleTouchUp(Vector3 screenPos)
        {
            var touched = Utils.GetInputTarget(screenPos, sceneSystem, cameraService.activeCamera);
            if (touched == null)
            {
                var worldPos = Utils.GetPlaneTouchPos(screenPos, cameraService.activeCamera);
                var cell = gridService.GetCell(worldPos);
                if (cell != null)
                {
                    touched = cell.cell.occupant;
                }
            }

            if (touched != null)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_TAP_UP, touched);
                var objectData = databaseService.Get<GridObjectData>(touched.objectId);
                var cell = touched.hasGrid ? touched.grid.pivot : null;
                commandSystem.Execute(objectData.onTapCommand, touched.position, cell, touched);
            }
            return false;
        }

        public bool HandleDoubleTouch(Vector3 screenPos) { return false; }

        #endregion
    }
}
