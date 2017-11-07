using MergeWar.Data;
using Services.Core.Event;

namespace Services.Game.Factory
{
    public sealed partial class FactoryEntity 
    {
        // Game specific entity
        public GameEntity CreateGameGridObject(string objectId)
        {
            var entity = CreateGridObject(objectId);

            var objectData = database.Get<GameGridObjecData>(objectId);

            if (objectData.canDrag)
            {
                entity.isDraggable = true;
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