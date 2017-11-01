using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Gesture;
using Services.Game.SceneCamera;
using Services.Core.Data;

namespace MergeWar
{
    /// <summary>
    /// Handle camera movement and drag/drop of objects
    /// </summary>

    public class DragSystem : IInitializeSystem, IExecuteSystem, IDragHandler
    {
        [Inject] CameraService cameraService;
        [Inject] DataProviderSystem dataProvider;

        #region IDragHandler implementation

        #if UNITY_EDITOR
        private Vector3 startPos;
        #endif

        private Vector3 currentPos;
        private Vector3 deltaPos;
        private float timer;
        private bool inertia = false;
        private bool isDragging = false;
        private GameConfig gameConfig;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            gameConfig = dataProvider.GetGameConfig();
        }

        #endregion

        public bool HandleDragStart(Vector3 screenPos)
        {
            if (!GestureService.IsOnUI())
            {
                inertia = false;
                isDragging = true;
                currentPos = screenPos;
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
                deltaPos = cameraService.camera.ScreenToWorldPoint(currentPos) - cameraService.camera.ScreenToWorldPoint(screenPos);
                cameraService.SetPosition(cameraService.position + deltaPos);
                currentPos = screenPos;

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
            }
            return false;
        }

        public bool HandleDragCancel(Vector3 screenPos)
        {
            inertia = false;
            isDragging = false;
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
    }
}