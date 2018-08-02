using Zenject;
using UnityEngine;
using Services.Core.Gesture;
using Services.Core.Data;
using Services.Core.Event;
using Services.Game.Grid;
using Services.Game.SceneCamera;
using MidcoreTemplate.Data;
using Services.Game.Data;
using MidcoreTemplate.Game.Utilities;

namespace MidcoreTemplate.Game.Systems
{
    public class TapCommandSystem : ITouchHandler
    {
        [Inject] CommandSystem commandSystem;
        [Inject] SceneSystem sceneSystem;
        [Inject] CameraService cameraService;
        [Inject] GridService gridService;

        #region ITouchHandler implementation

        public bool HandleTouchDown(Vector3 screenPos) { return false; }

        public bool HandleTouchUp(Vector3 screenPos)
        {
            if (!GestureService.IsOnUI())
            {
                var touched = Utils.GetInputTargetOnGrid(screenPos, sceneSystem, cameraService.activeCamera, gridService);
                if (touched != null && 
                    (!touched.hasGrid || (touched.hasGrid && touched.grid.cells.Count > 0 )))
                {
                    if (touched.hasCommand && !string.IsNullOrEmpty(touched.command.onTapCommand))
                    {
                        EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_TAP_UP, touched);
                        var cell = touched.hasGrid ? touched.grid.pivot : null;
                        commandSystem.Execute(touched.command.onTapCommand, touched.position, cell, touched);
                        AnimateObjectTouch(touched);
                    }
                }
            }
            return false;
        }

        public bool HandleDoubleTouch(Vector3 screenPos) { return false; }

        #endregion
        
        private void AnimateObjectTouch(GameEntity entity)
        {
            if (entity.hasView)
            {
                if (entity.hasGrid && entity.grid.pivot != null) entity.PositionOnCell();
                entity.TweenScale(new Vector3(1.2f, 1f, 0.8f), Vector3.one, 0.5f, LeanTweenType.easeOutBack);
            }
        }
    }
}
