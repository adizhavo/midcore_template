using SRDebugger;
using System.ComponentModel;
using MergeWar.Game;
using MergeWar.Data;
using Services.Game;
using Services.Game.Grid;
using UnityEngine;

public partial class SROptions
{
    private string _objectId;

    [Category("Gameplay")]
    public string objectId
    {
        get { return _objectId; }
        set { _objectId = value; }
    }

    [Category("Gameplay")]
    public void SpawnSingleObject()
    {
        var commandSystem = GameSystemInstaller.Resolve<CommandSystem>();
        var gridService = GameServiceInstaller.Resolve<GridService>();

        var command = new CommandData();
        command.type = Constants.COMMAND_SPAWN_OBJ;
        command.output = _objectId;
        command.count = 1;

        var cell = gridService.GetClosestCell(Vector3.zero, true);
        if (cell != null)
        {
            commandSystem.Execute(command, cell.position, cell);
        }
    }
}
