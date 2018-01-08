using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Grid;
using MergeWar.Data;
using MergeWar.Game.Command;
using System.Collections;

namespace MergeWar.Game
{
    /// <summary>
    /// Will execute commands from the game
    /// Commands must extend the BaseCommand class and be added in the GameInstaller
    /// </summary>

    public class CommandSystem
    {
        [Inject] DatabaseService database;
        [Inject] GridService gridService;

        private Hashtable commands = new Hashtable();

        public CommandSystem AddCommand(string key, BaseCommand command)
        {
            command.commandSystem = this;
            commands.Add(key, command);
            return this;
        }

        #region Execute methods

        public void Execute(string commandId, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var commandData = database.Get<CommandData>(commandId);
                Execute(commandData, trigger);
            }
        }

        public void Execute(string commandId, Vector3 executePos, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var commandData = database.Get<CommandData>(commandId);
                Execute(commandData, executePos, trigger);
            }
        }

        public void Execute(string commandId, GameEntity executeCell, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var commandData = database.Get<CommandData>(commandId);
                Execute(commandData, executeCell, trigger);
            }
        }

        public void Execute(string commandId, Vector3 executePos, GameEntity executeCell, GameEntity trigger = null)
        {
            if (!string.IsNullOrEmpty(commandId))
            {
                var commandData = database.Get<CommandData>(commandId);
                Execute(commandData, executePos, executeCell, trigger);
            }
        }

        public void Execute(CommandData commandData, GameEntity trigger = null)
        {
            var command = commands[commandData.type] as BaseCommand;
            var executePos = trigger != null ? trigger.position : Vector3.zero;
            var executeCell = trigger != null ? trigger.grid.pivot : gridService.GetClosestCell(executePos);
            if (executePos.sqrMagnitude < Mathf.Epsilon && executeCell != null) executePos = executeCell.position;
            command.ExecuteCommand(commandData, executePos, executeCell, trigger);
        }

        public void Execute(CommandData commandData, Vector3 executePos, GameEntity trigger = null)
        {
            var command = commands[commandData.type] as BaseCommand;
            var executeCell = gridService.GetClosestCell(executePos);
            command.ExecuteCommand(commandData, executePos, executeCell, trigger);
        }

        public void Execute(CommandData commandData, GameEntity executeCell, GameEntity trigger = null)
        {
            var command = commands[commandData.type] as BaseCommand;
            var executePos = trigger != null ? trigger.position : executeCell.position;
            command.ExecuteCommand(commandData, executePos, executeCell, trigger);
        }

        public void Execute(CommandData commandData, Vector3 executePos, GameEntity executeCell, GameEntity trigger = null)
        {
            var command = commands[commandData.type] as BaseCommand;
            command.ExecuteCommand(commandData, executePos, executeCell, trigger);
        }

        #endregion
    }
}