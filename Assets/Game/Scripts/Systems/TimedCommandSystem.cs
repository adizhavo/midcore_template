using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Data;
using MergeWar.Data;

namespace MergeWar.Game.Systems
{
    public class TimedCommandSystem : IExecuteSystem
    {
        [Inject] CommandSystem commandSystem;
        [Inject] DatabaseService database;

        #region IExecuteSystem implementation

        public void Execute()
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.GameObject, GameMatcher.TimedCommand));
            foreach(var entity in entities)
            {
                entity.timedCommand.remainingTime -= Time.deltaTime;
                if (entity.timedCommand.remainingTime < 0f)
                {
                    var objectData = database.Get<GridObjectData>(entity.objectId);
                    var time = objectData.timeout.GetRange();
                    entity.ReplaceTimedCommand(time, time);
                    var cell = entity.hasGrid ? entity.grid.pivot : null;
                    commandSystem.Execute(objectData.onTimeoutCommand, entity.position, cell, entity);
                }
            }
        }

        #endregion
        
    }
}