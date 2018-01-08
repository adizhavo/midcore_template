using UnityEngine;
using MidcoreTemplate.Data;
using Services.Core;
using Services.Game.Data;

namespace MidcoreTemplate.Game.Command
{
    /// <summary>
    /// Command for spawning object
    /// </summary>

    public class SpawnCommand : BaseCommand
    {
        #region implemented abstract members of BaseCommand

        protected override void ExecuteBodyCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandData.output))
            {
                for (int i = 0; i < commandData.count; i ++)
                {
                    var entity = factoryEntity.CreateGameGridObject(commandData.output);

                    if (cell == null)
                    {
                        cell = gridService.GetClosestCell(executePos, true);
                    }

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
                    entity.TweenScale(Vector3.zero, Vector3.one, 0.5f, LeanTweenType.easeOutBack);
                    var onSpawnCommand = database.Get<GridObjectData>(entity.objectId).onSpawnCommand;
                    commandSystem.Execute(onSpawnCommand, cell.position, cell, entity);
                }
            }
        }

        #endregion
        
    }
}