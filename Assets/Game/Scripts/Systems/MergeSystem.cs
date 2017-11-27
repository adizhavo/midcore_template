using Zenject;
using UnityEngine;
using Services.Core;
using Services.Core.Gesture;
using Services.Core.Data;
using Services.Core.Event;
using Services.Game.Grid;
using Services.Game.Factory;
using Services.Game.SceneCamera;
using MergeWar.Data;
using System.Collections;
using Utils = MergeWar.Game.Utilities.Utils;

namespace MergeWar.Game.Systems
{
    public class MergeSystem : IEventListener<GameEntity> , IDragHandler
    {
        [Inject] DatabaseService database;
        [Inject] GridService gridService;
        [Inject] DataProviderSystem dataProvider;
        [Inject] CameraService cameraService;
        [Inject] CommandSystem commandSystem;
        [Inject] FactoryEntity factoryEntity;

        private GameEntity dragged;

        public MergeSystem()
        {
            EventDispatcherService<GameEntity>.Subscribe(this, Constants.EVENT_ENTITY_START_DRAG);
            EventDispatcherService<GameEntity>.Subscribe(this, Constants.EVENT_ENTITY_CANCEL_DRAG);
        }

        #region IEventListener implementation

        public void Receive(string eventId, GameEntity value)
        {
            if (eventId.Equals(Constants.EVENT_ENTITY_START_DRAG))
            {
                dragged = value;
            }
            else if (eventId.Equals(Constants.EVENT_ENTITY_CANCEL_DRAG))
            {
                dragged = null;
            }
        }

        #endregion

        #region IDragHandler implementation

        public bool HandleDragStart(Vector3 screenPos) { return false; }

        public bool HandleDrag(Vector3 screenPos) { return false; }

        public bool HandleDragCancel(Vector3 screenPos) { return false; }

        public bool HandleDragEnd(Vector3 screenPos)
        {
            if (dragged != null && dragged.hasGameObject)
            {
                var gridPos = Utils.GetPlaneTouchPos(screenPos, cameraService.activeCamera);
                var cell = gridService.GetCell(gridPos);

                if (gridService.IsOccupied(cell))
                {
                    var occupant = cell.cell.occupant;
                    var mergeComboData = dataProvider.GetMergeComboDataForInput(dragged.objectId);
                    var canMerge = occupant != dragged && mergeComboData != null && dragged.objectId.Equals(occupant.objectId);

                    if (canMerge)
                    {
                        Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DEFAULT);

                        var spawnPos = occupant.position;

                        dragged.isDraggable = false;
                        occupant.isDraggable = false;

                        // destroy the entities
                        gridService.DeAttach(dragged);
                        gridService.DeAttach(occupant);
                        dragged.Destroy();
                        occupant.Destroy();

                        // spawn the output entity
                        var command = new CommandData();
                        command.type = Constants.COMMAND_SPAWN_OBJ;
                        command.output = mergeComboData.output;
                        command.count = 1;
                        commandSystem.Execute(command, spawnPos, cell);
                        factoryEntity.CreateVFX(mergeComboData.vfx, spawnPos);

                        // execute the merge complete command
                        commandSystem.Execute(mergeComboData.mergeCompleteCommand, spawnPos, cell);

                        // consume the gesture event
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}
