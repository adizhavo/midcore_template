using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Data;
using MidcoreTemplate.Data;

namespace MidcoreTemplate.Game.Systems
{
    public class TimedCommandSystem : IExecuteSystem
    {
        [Inject] CommandSystem commandSystem;
        [Inject] DatabaseService database;

        #region IExecuteSystem implementation

        public void Execute()
        {
            var entities = Contexts.sharedInstance.game.GetEntities(GameMatcher.AllOf(GameMatcher.GameObject, GameMatcher.TimedCommand, GameMatcher.Command));
            foreach(var entity in entities)
            {
                entity.timedCommand.remainingTime -= Time.deltaTime;
                if (entity.timedCommand.remainingTime < 0f)
                {
                    var time = entity.timedCommand.timeout.GetRange();
                    entity.ReplaceTimedCommand(time, time, entity.timedCommand.timeout);
                    var cell = entity.hasGrid ? entity.grid.pivot : null;
                    commandSystem.Execute(entity.command.onTimeoutCommand, entity.position, cell, entity);
                }
            }
        }

        #endregion
        
    }
}