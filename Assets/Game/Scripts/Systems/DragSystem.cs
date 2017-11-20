using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using Services.Core.Event;
using Services.Core.Gesture;
using Services.Game.Grid;
using Services.Game.SceneCamera;
using MergeWar.Data;
using MergeWar.Game.Utilities;

namespace MergeWar.Game.Systems
{
    /// <summary>
    /// Handle camera movement and drag/drop of objects
    /// </summary>

    public class DragSystem : IInitializeSystem, IExecuteSystem, IDragHandler, ITouchHandler
    {
        [Inject] SceneSystem sceneSystem;
        [Inject] CameraService cameraService;
        [Inject] DataProviderSystem dataProvider;
        [Inject] GridService gridService;
        [Inject] CommandSystem commandSystem;
        [Inject] DatabaseService database;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            gameConfig = dataProvider.GetGameConfig();
        }

        #endregion

        #if UNITY_EDITOR
        private Vector3 startPos;
        #endif

        private Vector3 currentPos;
        private Vector3 deltaPos;
        private float timer;
        private bool inertia = false;
        private bool isDragging = false;
        private GameConfig gameConfig;
        private GameEntity touched;
        private GameEntity dragged;
        private GameEntity draggedInitCell;

        #region ITouchHandler implementation

        public bool HandleTouchDown(Vector3 screenPos)
        {
            touched = Utils.GetInputTarget(screenPos, sceneSystem, cameraService.activeCamera);
            return false;
        }

        public bool HandleTouchUp(Vector3 screenPos) { return false; }

        public bool HandleDoubleTouch(Vector3 screenPos) { return false; }

        #endregion

        #region IDragHandler implementation

        public bool HandleDragStart(Vector3 screenPos)
        {
            if (!GestureService.IsOnUI())
            {
                inertia = false;
                isDragging = true;
                currentPos = screenPos;
                StartDragEntity();

                #if UNITY_EDITOR
                startPos = currentPos;
                #endif
            }
            else
            {
                inertia = false;
                isDragging = false; 
            }

            return false;
        }

        public bool HandleDrag(Vector3 screenPos)
        {
            if (isDragging)
            {
                if (dragged == null || !dragged.hasGameObject)
                {
                    deltaPos = cameraService.activeCamera.ScreenToWorldPoint(currentPos) - cameraService.activeCamera.ScreenToWorldPoint(screenPos);
                    var projected = new Vector3(deltaPos.x, 0f, deltaPos.z).normalized;
                    var angle = Vector3.Angle(deltaPos, projected);
                    var trueOffset = projected * (deltaPos.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad));
                    cameraService.SetPosition(cameraService.position + trueOffset);
                    currentPos = screenPos;
                }
                else
                {
                    var pos = Utils.GetPlaneTouchPos(screenPos, cameraService.activeCamera);
                    dragged.position = pos;
                }

                #if UNITY_EDITOR
                Debug.DrawLine(startPos, currentPos);
                #endif
            }
            return false;
        }

        public bool HandleDragEnd(Vector3 screenPos)
        {
            if (isDragging)
            {
                inertia = true;
                timer = 0f;
                isDragging = false;
                HandleDraggedObject(screenPos);
            }
            return false;
        }

        public bool HandleDragCancel(Vector3 screenPos)
        {
            inertia = false;
            isDragging = false;
            CancelDraggedObject();
            return false;
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            if (inertia && timer <= gameConfig.cameraInertiaDuration)
            {
                var projected = new Vector3(deltaPos.x, 0f, deltaPos.z).normalized;
                var angle = Vector3.Angle(deltaPos, projected);
                var trueOffset = projected * (deltaPos.magnitude / Mathf.Cos(angle * Mathf.Deg2Rad));
                cameraService.SetPosition(cameraService.position + trueOffset);
                deltaPos = Vector3.Lerp(deltaPos, Vector3.zero, timer);
                timer += Time.smoothDeltaTime;
            }
        }

        #endregion

        private void StartDragEntity()
        {
            // start and drag the touched entity
            if (touched != null && touched.isDraggable && touched.hasGameObject)
            {
                dragged = touched;
                touched = null;
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DRAG);
                draggedInitCell = dragged.grid.pivot;
                gridService.DeAttach(dragged);
                dragged.CancelTween();
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_START_DRAG, dragged);
            }
        }

        private void HandleDraggedObject(Vector3 screenPos)
        {
            if (dragged != null && dragged.hasGameObject)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_END_DRAG, dragged);
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DEFAULT);
                var pos = Utils.GetPlaneTouchPos(screenPos, cameraService.activeCamera);
                var closestCell = gridService.GetClosestCell(pos, false);
                gridService.SetEntityOn(dragged, closestCell);

                var objectData = database.Get<GameGridObjectData>(dragged.objectId);
                commandSystem.Execute(objectData.onDragEndCommand, closestCell, dragged);

                dragged = null;
            }
        }

        private void CancelDraggedObject()
        {
            if (dragged != null && dragged.hasGameObject)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_CANCEL_DRAG, dragged);
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DEFAULT);
                gridService.SetEntityOn(dragged, draggedInitCell);
                dragged = null;
            } 
        }
    }
}