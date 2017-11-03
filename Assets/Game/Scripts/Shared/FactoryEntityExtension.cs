using MergeWar.Data;
using Services.Core.Event;

namespace Services.Game.Factory
{
    public sealed partial class FactoryEntity 
    {
        public GameEntity CreateGameGridObject(string objectId)
        {
            var entity = CreateGridObject(objectId);

            var objectData = database.Get<GameGridObjecData>(objectId);

            if (objectData.canDrag)
            {
                entity.isDraggable = true;
            }

            return entity;
        }
    }
}