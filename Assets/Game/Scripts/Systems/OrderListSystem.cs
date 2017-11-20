using Zenject;
using UnityEngine;
using Services.Core;
using Services.Core.Gesture;
using Services.Core.Event;
using Services.Game.SceneCamera;
using Services.Game.Grid;
using Services.Core.Data;
using MergeWar.Data;
using System.Collections;
using Utils = MergeWar.Game.Utilities.Utils;

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
                var gridPos = Utils.GetPlaneTouchPos(screenPos, cameraService.activeCamera);
                var cell = gridService.GetCell(gridPos);

                if (cell != null && gridService.IsOccupied(cell))
                {
                    bool isObjectOrder = cell.cell.occupant.orderList.HasOrder(dragged.objectId);
                    bool isTypeOrder = cell.cell.occupant.orderList.HasOrder(dragged.typeId);

                    if (cell.cell.occupant.hasGameObject
                        && cell.cell.occupant.hasOrderList
                        && (isObjectOrder || isTypeOrder)
                        && !cell.cell.occupant.orderList.Filled())
                    {
                        float animationLength = 0.3f;
                        dragged.TweenToPosition(cell.cell.occupant.position, animationLength, LeanTweenType.easeInExpo);
                        var orderId = isObjectOrder ? dragged.objectId : dragged.typeId;
                        SceneAttachment.AttachCoroutine(AddOrderWithDelay(orderId, cell, animationLength));
                        return true;
                    }
                }

                dragged = null;
            }

            return false;
        }

        public bool HandleDragCancel(Vector3 screenPos) { return false; }

        #endregion

        private IEnumerator AddOrderWithDelay(string orderId, GameEntity cell, float delay)
        {
            yield return new WaitForSeconds(delay);
            cell.cell.occupant.orderList.AddOrder(orderId);
            TryCompleteOrder(cell);
        }

        private void TryCompleteOrder(GameEntity cell)
        {
            if (gridService.IsOccupied(cell))
            {
                var occupant = cell.cell.occupant;
                var objectData = database.Get<GameGridObjectData>(occupant.gameObject.objectId);
                var isFilled = occupant.orderList.Filled();
                var commandId = isFilled ? objectData.onOrderCompleteCommand : objectData.onOrderUpdateCommand;
                commandSystem.Execute(commandId, occupant.position, cell, occupant);
            }

            dragged.Destroy();
        }
    }
}