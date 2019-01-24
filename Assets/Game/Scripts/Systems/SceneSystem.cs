using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Data;
using System.Collections.Generic;
using MidcoreTemplate.Data;

namespace MidcoreTemplate.Game.Systems
{
    /// <summary>
    /// Add all scene related queries and functionalities
    /// </summary>

    public class SceneSystem
    {
        [Inject] DatabaseService database;

        public GameEntity GetEntityWithView(GameObject viewObject)
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.View);
            foreach (var entity in entities)
            {
                if (entity.viewObject == viewObject) return entity;
            }

            return null;
        }

        public GameEntity GetEntityWithObjectId(string objectId)
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.View, GameMatcher.GameObject));
            foreach (var entity in entities)
            {
                if (entity.objectId == objectId) return entity;
            }

            return null;
        }

        public List<GameEntity> GetAllEntitiesWithType(string typeId)
        {
            var selected = new List<GameEntity>();
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.View, GameMatcher.GameObject));
            foreach (var entity in entities)
            {
                if (entity.typeId == typeId)
                {
                    selected.Add(entity);
                }
            }

            return selected;
        }

        public List<GameEntity> GetAllEntitiesWithObjectId(string objectId)
        {
            var selected = new List<GameEntity>();
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.View, GameMatcher.GameObject));
            foreach (var entity in entities)
            {
                if (entity.objectId == objectId)
                {
                    selected.Add(entity);
                }
            }

            return selected;
        }

        public GameEntity GetEntityWithTypeAndLevel(string typeId, int level)
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.View, GameMatcher.GameObject));
            foreach (var entity in entities)
            {
                if (entity.typeId == typeId && database.Get<GridObjectData>(entity.objectId).level == level) return entity;
            }

            return null;
        }

        public GameEntity GetEntityWithUniqueId(int uniqueId)
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.View, GameMatcher.GameObject));
            foreach (var entity in entities)
            {
                if (entity.uniqueId == uniqueId) return entity;
            }

            return null;
        }
    }
}