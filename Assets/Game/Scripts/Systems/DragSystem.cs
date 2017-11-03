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
            touched = Utils.GetInputTarget(screenPos, sceneSystem, cameraService.camera);
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
            return false;
        }

        public bool HandleDrag(Vector3 screenPos)
        {
            if (isDragging)
            {
                if (dragged == null)
                {
                    deltaPos = cameraService.camera.ScreenToWorldPoint(currentPos) - cameraService.camera.ScreenToWorldPoint(screenPos);
                    cameraService.SetPosition(cameraService.position + deltaPos);
                    currentPos = screenPos;
                }
                else
                {
                    var pos = Utils.GetPlaneTouchPos(screenPos, cameraService.camera);
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
                PositionDraggedObject(screenPos);
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
                cameraService.SetPosition(cameraService.position + deltaPos);
                deltaPos = Vector3.Lerp(deltaPos, Vector3.zero, timer);
                timer += Time.smoothDeltaTime;
            }
        }

        #endregion

        private void StartDragEntity()
        {
            // start and drag the touched entity
            if (touched != null && touched.isDraggable)
            {
                dragged = touched;
                touched = null;
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DRAG);
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_START_DRAG, dragged);
                draggedInitCell = dragged.grid.pivot;
                gridService.DeAttach(dragged);
                dragged.CancelTween();
            }
        }

        private void PositionDraggedObject(Vector3 screenPos)
        {
            if (dragged != null)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_END_DRAG, dragged);
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DEFAULT);
                var pos = Utils.GetPlaneTouchPos(screenPos, cameraService.camera);
                var closestCell = gridService.GetClosestCell(pos, false);
                gridService.SetEntityOn(dragged, closestCell);
                dragged.TweenToCell();
                dragged = null;
            }
        }

        private void CancelDraggedObject()
        {
            if (dragged != null)
            {
                EventDispatcherService<GameEntity>.Dispatch(Constants.EVENT_ENTITY_CANCEL_DRAG, dragged);
                Utils.SetSortingLayer(dragged, Constants.SORTING_LAYER_DEFAULT);
                gridService.SetEntityOn(dragged, draggedInitCell);
                dragged.TweenToCell();
                dragged = null;
            } 
        }
    }
}