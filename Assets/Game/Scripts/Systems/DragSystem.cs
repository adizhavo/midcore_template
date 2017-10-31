using UnityEngine;
using Services.Core.Gesture;

namespace MergeWar
{
    public class DragSystem : IDragHandler, IPinchHandler
    {
        #region IDragHandler implementation

        private Vector3 initPos;
        private Vector3 deltaPos;

        public bool HandleDragStart(Vector3 screenPos)
        {
            initPos = screenPos;
            return false;
        }

        public bool HandleDrag(Vector3 screenPos)
        {
            var currPos = screenPos;
            deltaPos = Camera.main.ScreenToWorldPoint(initPos) - Camera.main.ScreenToWorldPoint(currPos);

            Camera.main.transform.position += deltaPos;

            Debug.DrawLine(initPos, currPos);
            initPos = currPos;

            return false;
        }

        public bool HandleDragEnd(Vector3 screenPos)
        {
            return false;
        }

        #endregion

        #region IPinchHandler implementation

        public bool HandlePinchStart(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            return false;
        }

        public bool HandlePinch(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            return false;
        }

        public bool HandlePinchEnd(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            return false;
        }

        #endregion
    }
}