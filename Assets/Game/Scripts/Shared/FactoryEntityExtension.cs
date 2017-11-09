using MergeWar.Data;
using MergeWar.Game.Components;
using Services.Core.Event;
using System.Collections.Generic;

namespace Services.Game.Factory
{
    public sealed partial class FactoryEntity 
    {
        // Game specific entity
        public GameEntity CreateGameGridObject(string objectId)
        {
            var entity = CreateGridObject(objectId);

            var objectData = database.Get<GameGridObjectData>(objectId);

            if (objectData.canDrag)
            {
                entity.isDraggable = true;
            }

            if (!string.IsNullOrEmpty(objectData.onOrderCompleteCommand))
            {
                var orderList = new List<OrderListComponent.Order>();
                foreach(var keyValue in objectData.orderList)
                {
                    orderList.Add(new OrderListComponent.Order(keyValue.key, 0, keyValue.value));
                }
                        
                entity.AddOrderList(orderList);
            }

            if (!string.IsNullOrEmpty(objectData.onTimeoutCommand))
            {
                var time = objectData.timeout.GetRange();
                entity.AddTimedCommand(time, time);
            }

            return entity;
        }
    }
}