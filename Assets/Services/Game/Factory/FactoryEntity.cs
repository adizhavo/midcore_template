using Zenject;
using Entitas;
using Services.Core;
using Services.Core.Data;
using Services.Game.Data;
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
        [Inject] DatabaseService database;

        public FactoryEntity()
        {
            Contexts.sharedInstance.game.OnEntityWillBeDestroyed += CleanupEntity;
        }

        ~FactoryEntity()
        {
            Contexts.sharedInstance.game.OnEntityWillBeDestroyed -= CleanupEntity;
        }

        public GameEntity CreateCell(int row, int column, string objectId, GameEntity occupant = null)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            var objectData = database.Get<ObjectData>(objectId);
            var prefabPath = database.Get<string>(objectData.prefab);
            entity.AddGameObject(objectData.objectId, objectData.typeId, Utils.GenerateUniqueId());
            entity.AddResource(prefabPath);
            entity.AddCell(row, column, occupant);
            var view = FactoryPool.GetPooled(prefabPath);
            entity.AddView(view);
            return entity;
        }

        public GameEntity CreateGridObject(string objectId)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            var objectData = database.Get<ObjectData>(objectId);
            var prefabPath = database.Get<string>(objectData.prefab);
            entity.AddGameObject(objectData.objectId, objectData.typeId, Utils.GenerateUniqueId());
            entity.AddResource(prefabPath);
            entity.AddGrid(null, new List<GameEntity>(), new Footprint());
            var view = FactoryPool.GetPooled(prefabPath);
            entity.AddView(view);
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