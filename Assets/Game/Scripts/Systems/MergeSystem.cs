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

                        // play merge animation and trigger the command at the end of it
                        var animationLength = 0.4f;
                        AnimateMerge(occupant, spawnPos, animationLength);
                        SceneAttachment.AttachCoroutine(ExecuteMergeWithDelay(occupant, mergeComboData, spawnPos, cell, animationLength));

                        // consume the gesture event
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        private IEnumerator ExecuteMergeWithDelay(GameEntity occupant, MergeComboData mergeComboData, Vector3 spawnPos, GameEntity cell, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
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
        }

        private void AnimateMerge(GameEntity occupant, Vector3 spawnPos, float animDuration)
        {
            float xDistance = 0.7f;
            float yDistance = 0.2f;
            float destroyScale = 0.5f;

            dragged.CancelTween();
            LeanTween.move(dragged.viewObject, occupant.position + new Vector3(-1 * xDistance, yDistance, 0f), animDuration / 2f)
                .setEaseOutExpo()
                .setIgnoreTimeScale(true)
                .setOnComplete(() => 
            {
                LeanTween.move(dragged.viewObject, spawnPos, animDuration / 2f)
                        .setIgnoreTimeScale(true)
                        .setEaseInExpo();
                dragged.TweenScale(Vector3.one, Vector3.one * destroyScale, animDuration / 2f);
            });
            
            occupant.CancelTween();
            LeanTween.move(occupant.viewObject, occupant.position + new Vector3(xDistance, yDistance, 0f), animDuration / 2f)
                .setEaseOutExpo()
                .setIgnoreTimeScale(true)
                .setOnComplete(() => 
            {
                LeanTween.move(occupant.viewObject, spawnPos, animDuration / 2f)
                        .setIgnoreTimeScale(true)
                        .setEaseInExpo();
                occupant.TweenScale(Vector3.one, Vector3.one * destroyScale, animDuration / 2f);
            });
        }
        
    }
}