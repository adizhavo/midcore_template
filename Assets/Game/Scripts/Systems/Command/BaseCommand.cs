﻿using Entitas;
using Zenject;
using UnityEngine;
using System.Collections.Generic;
using MergeWar.Data;
using Services.Core.Data;
using Services.Game.Grid;
using Services.Game.Factory;

namespace MergeWar.Game.Command
{
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
                var onDestroyCommand = database.Get<GameGridObjecData>(trigger.objectId).onDestroyCommand;
                commandSystem.Execute(onDestroyCommand, cell.position, cell, trigger);
                trigger.Destroy();
            }
        }

        protected virtual void ExecuteTailCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandData.vfx))
            {
                var vfx = factoryEntity.CreateVFX(commandData.vfx);
                vfx.position = executePos;
            }

            if (!string.IsNullOrEmpty(commandData.chainedCommand))
            {
                commandSystem.Execute(commandData.chainedCommand, executePos, cell, trigger);
            }
        }

        protected abstract void ExecuteBodyCommand(CommandData commandData, Vector3 executePos, GameEntity cell, GameEntity trigger = null);
    }
}