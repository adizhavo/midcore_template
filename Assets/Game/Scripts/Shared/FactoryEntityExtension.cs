using MidcoreTemplate.Data;
using MidcoreTemplate.Game.Components;
using Services.Core.Event;
using Services.Game.Data;
using System.Collections.Generic;

namespace Services.Game.Factory
{
    public sealed partial class FactoryEntity 
    {
        // Game specific entity
        public GameEntity CreateGameGridObject(string objectId)
        {
            var entity = CreateGridObject(objectId);

            var objectData = database.Get<GridObjectData>(objectId);

            if (objectData.canDrag)
            {
                entity.isDraggable = true;
            }

            if (!string.IsNullOrEmpty(objectData.onOrderCompleteCommand))
            {
                var orderList = new List<OrderListComponent.Order>();
                if (objectData.objectOrderList != null)
                {
                    foreach(var keyValue in objectData.objectOrderList)
                    {
                        orderList.Add(new OrderListComponent.Order(keyValue.Key, 0, keyValue.Value));
                    }
                }

                if (objectData.typeOrderList != null)
                {
                    foreach(var keyValue in objectData.typeOrderList)
                    {
                        orderList.Add(new OrderListComponent.Order(keyValue.Key, 0, keyValue.Value));
                    }
                }
                        
                entity.AddOrderList(orderList);
            }

            if (!string.IsNullOrEmpty(objectData.onTimeoutCommand))
            {
                var time = objectData.timeout.GetRange();
                entity.AddTimedCommand(time, time, objectData.timeout);
            }

            return entity;
        }
    }
}