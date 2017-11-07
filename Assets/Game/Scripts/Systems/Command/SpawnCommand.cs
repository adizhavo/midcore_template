using UnityEngine;
using MergeWar.Data;
using Services.Core;

namespace MergeWar.Game.Command
{
    /// <summary>
    /// Command for spawning object
    /// </summary>

    public class SpawnCommand : BaseCommand
    {
        #region implemented abstract members of BaseCommand

        protected override void ExecuteBodyCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            for (int i = 0; i < commandData.count; i ++)
            {
                if (!string.IsNullOrEmpty(commandData.output))
                {
                    var entity = factoryEntity.CreateGameGridObject(commandData.output);

                    if (!gridService.DoesFit(entity, cell))
                    {
                        cell = gridService.GetClosestFitPivotCell(entity, cell);

                        if (cell == null)
                        {
                            LogWrapper.Error("[{0}] Could not fit the entity, the command will stop execution, command id: {1}", GetType(), commandData.id);
                            entity.Destroy();
                            return;
                        }
                    }

                    entity.position = executePos;
                    gridService.SetEntityOn(entity, cell);
                    var onSpawnCommand = database.Get<GameGridObjectData>(entity.objectId).onSpawnCommand;
                    commandSystem.Execute(onSpawnCommand, cell.position, cell, entity);
                }
            }
        }

        #endregion
        
    }
}