using Entitas;
using UnityEngine;
using System.Linq;

namespace MergeWar.Game.Systems
{
    /// <summary>
    /// Add all scene related queries and functionalities
    /// </summary>

    public class SceneSystem
    {
        public GameEntity GetEntityWithView(GameObject viewObject)
        {
            return Contexts.sharedInstance.game.GetEntities(GameMatcher.View).ToList().Find(ge => ge.viewObject == viewObject);
        }
    }
}