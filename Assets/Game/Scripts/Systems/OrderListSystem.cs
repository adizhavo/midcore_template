using Zenject;
using UnityEngine;
using Services.Core.Gesture;
using Services.Core.Event;
using Services.Game.SceneCamera;
using MergeWar.Game.Utilities;
using Services.Game.Grid;
using Services.Core.Data;
using MergeWar.Data;

namespace MergeWar.Game.Systems
{
    public class OrderListSystem : IDragHandler, IEventListener<GameEntity>
    {
        [Inject] SceneSystem sceneSystem;
        [Inject] CameraService cameraService;
        [Inject] GridService gridService;
        [Inject] CommandSystem commandSystem;
        [Inject] DatabaseService database;

        private GameEntity dragged;

        public OrderListSystem()
        {
            EventDispatcherService<GameEntity>.Subscribe(this, Constants.EVENT_ENTITY_START_DRAG);
            EventDispatcherService<GameEntity>.Subscribe(this, Constants.EVENT_ENTITY_CANCEL_DRAG);
        }

        #region IEventListener implementation

        public void Receive(string eventId, GameEntity value)
        {
            if (eventId.Equals(Constants.EVENT_ENTITY_START_DRAG))
            {
                dragged = value;
            }
            else if (eventId.Equals(Constants.EVENT_ENTITY_CANCEL_DRAG))
            {
                dragged = null;
            }
        }

        #endregion

        #region IDragHandler implementation

        public bool HandleDragStart(Vector3 screenPos) { return false; }

        public bool HandleDrag(Vector3 screenPos) { return false; }

        public bool HandleDragEnd(Vector3 screenPos)
        {
            if (dragged != null && dragged.hasGameObject)
            {
                var gridPos = Utils.GetPlaneTouchPos(screenPos, cameraService.camera);
                var cell = gridService.GetCell(gridPos);

                if (gridService.IsOccupied(cell) 
                    && cell.cell.occupant.hasGameObject
                    && cell.cell.occupant.hasOrderList
                    && cell.cell.occupant.orderList.HasOrder(dragged.objectId)
                    && !cell.cell.occupant.orderList.Filled())
                {
                    dragged.TweenToPosition(cell.cell.occupant.position, 0.3f, LeanTweenType.easeInExpo);
                    LeanTween.delayedCall(0.3f, () =>
                    {
                        cell.cell.occupant.orderList.AddOrder(dragged.objectId);
                        var objectData = database.Get<GameGridObjectData>(cell.cell.occupant.objectId);
                        var isFilled = cell.cell.occupant.orderList.Filled();
                        var commandId = isFilled ? objectData.onOrderCompleteCommand : objectData.onOrderUpdateCommand;
                        commandSystem.Execute(commandId, cell.cell.occupant.position, cell, cell.cell.occupant);
                        dragged.Destroy();
                    });
                    return true;
                }

                dragged = null;
            }

            return false;
        }

        public bool HandleDragCancel(Vector3 screenPos) { return false; }

        #endregion
        
    }
}