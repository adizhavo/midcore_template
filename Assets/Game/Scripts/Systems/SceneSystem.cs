using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using System.Linq;
using System.Collections.Generic;
using MergeWar.Data;

namespace MergeWar.Game.Systems
{
    /// <summary>
    /// Add all scene related queries and functionalities
    /// </summary>

    public class SceneSystem
    {
        [Inject] DatabaseService database;

        public GameEntity GetEntityWithView(GameObject viewObject)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().Find(ge => ge.viewObject == viewObject);
        }

        public GameEntity GetEntityWithObjectId(string objectId)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().Find(
                ge => !string.IsNullOrEmpty(objectId) 
                && string.Equals(objectId, ge.objectId));
        }

        public List<GameEntity> GetAllEntitiesWithType(string typeId)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().FindAll(
                ge => !string.IsNullOrEmpty(typeId) 
                && string.Equals(typeId, ge.typeId));
        }

        public List<GameEntity> GetAllEntitiesWithObjectId(string objectId)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().FindAll(
                ge => !string.IsNullOrEmpty(objectId) 
                && string.Equals(objectId, ge.objectId));
        }

        public GameEntity GetEntityWithTypeAndLevel(string typeId, int level)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().Find(
                ge => !string.IsNullOrEmpty(typeId) 
                && string.Equals(typeId, ge.typeId) 
                && database.Get<GameGridObjectData>(ge.objectId).level == level);
        }

        public GameEntity GetEntityWithUniqueId(string uniqueId)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().Find(
                ge => !string.IsNullOrEmpty(uniqueId) 
                && string.Equals(uniqueId, ge.uniqueId));
        }
    }
}