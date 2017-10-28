using Entitas;
using Services.Game.Grid;
using Services.Game.Components;
using System.Collections.Generic;

namespace Services.Game.Factory
{
    /// <summary>
    /// Will build all kind of entities, extend by adding more methods
    /// </summary>

    public sealed partial class FactoryEntity 
    {
        public FactoryEntity()
        {
            Contexts.sharedInstance.game.OnEntityWillBeDestroyed += CleanupEntity;
        }

        ~FactoryEntity()
        {
            Contexts.sharedInstance.game.OnEntityWillBeDestroyed -= CleanupEntity;
        }

        // WIP
        public GameEntity CreateCell(int row, int column, string typeId, string objectId, GameEntity occupant = null)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            // TODO : read this from the database
            var view = FactoryPool.GetPooled("Prefabs/Map/Tile");
            entity.AddView(view);
            entity.AddGameObject(string.Empty, -1);
            entity.AddResource(string.Empty);
            entity.AddCell(row, column, typeId, objectId, occupant);
            return entity;
        }

        // WIP
        public GameEntity CreateGridObject()
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            // TODO : read this from the database
            var view = FactoryPool.GetPooled("Prefabs/Objects/Object");
            entity.AddView(view);
            entity.AddGameObject(string.Empty, -1);
            entity.AddResource(string.Empty);
            entity.AddGrid(null, new List<GameEntity>(), new Footprint());
            return entity;
        }

        private void CleanupEntity(IContext context, IEntity entity)
        {
            var gameEntity = (GameEntity)entity;

            if (gameEntity != null)
            {
                // cleanup process for different components

                if (gameEntity.hasView) // return view to the pool
                {
                    gameEntity.viewObject.SetActive(false);
                }
            }
        }
    }
}