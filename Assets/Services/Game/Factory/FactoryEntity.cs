using Zenject;
using Entitas;
using Services.Core;
using Services.Core.Data;
using Services.Core.Event;
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
            #if UNITY_EDITOR
            entity.viewObject.name = string.Format("cell_{0}_{1}_{2}_{3}_{4}", entity.objectId, entity.typeId, entity.row, entity.column, entity.uniqueId);
            #endif
            EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_CELL_ENTITY_CREATION, entity);
            return entity;
        }

        public GameEntity CreateGridObject(string objectId)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            var objectData = database.Get<GridObjectData>(objectId);
            var prefabPath = database.Get<string>(objectData.prefab);
            entity.AddGameObject(objectData.objectId, objectData.typeId, Utils.GenerateUniqueId());
            entity.AddResource(prefabPath);
            var defaultFootprint = new List<List<int>>() { new List<int>() { 1 } };
            entity.AddGrid(null, new List<GameEntity>(), new Footprint(objectData.footprintData == null ? defaultFootprint : objectData.footprintData));
            var view = FactoryPool.GetPooled(prefabPath);
            entity.AddView(view);
            #if UNITY_EDITOR
            entity.viewObject.name = string.Format("ent_{0}_{1}_{2}", entity.objectId, entity.typeId, entity.uniqueId);
            #endif
            EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_GRID_ENTITY_CREATION, entity);
            return entity;
        }

        public GameEntity CreateVFX(string vfxId)
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();
            var vfxData = database.Get<VFXData>(vfxId);
            var prefabPath = database.Get<string>(vfxData.prefab);
            entity.AddResource(prefabPath);
            var view = FactoryPool.GetPooled(prefabPath);
            entity.AddView(view);

            if (vfxData.disableTime > 0f)
            {
                entity.AddAutoDestroy(vfxData.disableTime, vfxData.ignoreTimescale);
            }

            return entity;
        }

        public void CleanupEntity(IContext context, IEntity entity)
        {
            var gameEntity = (GameEntity)entity;

            if (gameEntity != null)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_DESTRUCTION, gameEntity);
                // cleanup process for different components

                if (gameEntity.hasView) // return view to the pool
                {
                    gameEntity.viewObject.SetActive(false);
                }
            }
        }
    }
}