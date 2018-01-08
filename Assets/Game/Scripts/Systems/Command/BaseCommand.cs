using Entitas;
using Zenject;
using UnityEngine;
using System.Collections.Generic;
using MergeWar.Data;
using Services.Core.Data;
using Services.Game.Data;
using Services.Game.Grid;
using Services.Game.Factory;
using Services.Game.Tutorial;

namespace MergeWar.Game.Command
{
    /// <summary>
    /// Extend this class for different commands
    /// </summary>

    public abstract class BaseCommand 
    {
        [Inject] protected DatabaseService database;
        [Inject] protected GridService gridService;
        [Inject] protected FactoryEntity factoryEntity;

        public CommandSystem commandSystem;

        public virtual void ExecuteCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            ExecuteHeadCommand(commandData, executePos, cell, trigger);
            ExecuteBodyCommand(commandData, executePos, cell, trigger);
            ExecuteTailCommand(commandData, executePos, cell, trigger);
        }

        protected virtual void ExecuteHeadCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            if (commandData.destroyTrigger && trigger != null)
            {
                var onDestroyCommand = database.Get<GridObjectData>(trigger.objectId).onDestroyCommand;
                commandSystem.Execute(onDestroyCommand, cell.position, cell, trigger);

                gridService.DeAttach(trigger);

                trigger.Destroy();
            }
        }

        protected virtual void ExecuteTailCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            factoryEntity.CreateVFX(commandData.vfx, executePos);

            if (!string.IsNullOrEmpty(commandData.chainedCommand))
            {
                commandSystem.Execute(commandData.chainedCommand, executePos, cell, trigger);
            }

            if (!string.IsNullOrEmpty(commandData.tutorialTrigger))
            {
                TutorialService<TutorialStep>.Notify(commandData.tutorialTrigger);
            }
        }

        protected abstract void ExecuteBodyCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null);
    }
}