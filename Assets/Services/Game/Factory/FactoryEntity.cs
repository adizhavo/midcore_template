using Zenject;
using Entitas;
using Services.Core;
using Services.Core.Data;
using Services.Core.Event;
using Services.Game.Data;
using Services.Game.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Game.Factory
{
    /// <summary>
    /// Will build all kind of entities, extend by adding more methods
    /// </summary>

    public sealed partial class FactoryEntity 
    {
        [Inject] DatabaseService database;
        [Inject] FactoryGUI factoryGUI;

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
            entity.AddGrid(null, new List<GameEntity>(), new Footprint(objectData.footprintData == null ? defaultFootprint : objectData.footprintData), objectData.canSwap);
            var view = FactoryPool.GetPooled(prefabPath);
            entity.AddView(view);
            #if UNITY_EDITOR
            entity.viewObject.name = string.Format("ent_{0}_{1}_{2}", entity.objectId, entity.typeId, entity.uniqueId);
            #endif
            EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_GRID_ENTITY_CREATION, entity);
            return entity;
        }

        public GameEntity CreateVFX(string vfxId, Vector3 pos)
        {
            if (!string.IsNullOrEmpty(vfxId))
            {
                var entity = Contexts.sharedInstance.game.CreateEntity();
                var vfxData = database.Get<VFXData>(vfxId);
                if (vfxData.isGUI)
                {
                    var split = vfxData.moveToPanel.Split('.');
                    if (split.Length == 2)
                    {
                        factoryGUI.AnimateFloatingUIWorldPos(vfxData.prefab, pos, split[0], split[1], vfxData.activeTime);
                    }
                }
                else
                {
                    var prefabPath = database.Get<string>(vfxData.prefab);
                    entity.AddResource(prefabPath);
                    var view = FactoryPool.GetPooled(prefabPath);
                    entity.AddView(view);
                    entity.position = pos;
                }

                if (vfxData.activeTime > 0f)
                {
                    entity.AddAutoDestroy(vfxData.activeTime, vfxData.ignoreTimescale);
                }

                entity.position = pos;

                return entity;
            }
            else return null;
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
                    gameEntity.localScale = Vector3.one;
                    gameEntity.viewObject.SetActive(false);
                }
            }
        }
    }
}