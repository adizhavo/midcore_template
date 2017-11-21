using SRDebugger;
using UnityEngine;
using System.ComponentModel;
using MergeWar.Game;
using MergeWar.Data;
using Services.Game;
using Services.Game.Grid;
using Services.Game.Tutorial;

public partial class SROptions
{
    private string _objectId;
    private string _tutorialId;

    [Category("Gameplay")]
    public string objectId
    {
        get { return _objectId; }
        set { _objectId = value; }
    }

    [Category("Gameplay")]
    public void SpawnSingleObject()
    {
        if (!string.IsNullOrEmpty(_objectId))
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

    [Category("Gameplay")]
    public string tutorialId
    {
        get { return _tutorialId; }
        set { _tutorialId = value; }
    }

    [Category("Tutorial")]
    public void ForceCompleteCurrent()
    {
        TutorialService<TutorialStep>.CompleteCurrentTutorial();
    }

    [Category("Tutorial")]
    public void ForceCompleteSelected()
    {
        TutorialService<TutorialStep>.CompleteTutorial(tutorialId);
    }
}
