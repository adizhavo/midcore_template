using UnityEngine;

namespace Services.Core.Gesture
{
    /// <summary>
    /// Use this if the execution order do not matter 
    /// This is for usage in the scene
    /// Otherwise is suggested to use the one of the gesture handler interface
    /// </summary>

    public class GestureEventDispatcher : ITouchHandler, IDragHandler, IPinchHandler, ITouchHoldHandler
    {
        public delegate void TouchDown(Vector3 screenPos);
        public static event TouchDown OnTouchDown;

        public delegate void TouchUp(Vector3 screenPos);
        public static event TouchUp OnTouchUp;

        public delegate void DoubleTouch(Vector3 screenPos);
        public static event DoubleTouch OnDoubleTouch;

        public delegate void DragStart(Vector3 screenPos);
        public static event DragStart OnDragStart;

        public delegate void Drag(Vector3 screenPos);
        public static event Drag OnDrag;

        public delegate void DragEnd(Vector3 screenPos);
        public static event DragEnd OnDragEnd;

        public delegate void PinchStart(Vector3 firstScreenPos, Vector3 secondScreenPos);
        public static event PinchStart OnPinchStart;

        public delegate void Pinch(Vector3 firstScreenPos, Vector3 secondScreenPos);
        public static event Pinch OnPinch;

        public delegate void PinchEnd(Vector3 firstScreenPos, Vector3 secondScreenPos);
        public static event PinchEnd OnPinchEnd;

        public delegate void HoldStart(Vector3 screenPos, float holdTime);
        public static event HoldStart OnHoldStart;

        public delegate void Hold(Vector3 screenPos, float holdTime);
        public static event Hold OnHold;

        public delegate void HoldEnd(Vector3 screenPos, float holdTime);
        public static event HoldEnd OnHoldEnd;

        #region ITouchHandler implementation

        public bool HandleTouchDown(Vector3 screenPos)
        {
            if (OnTouchDown != null)
                OnTouchDown(screenPos);
            return false;
        }

        public bool HandleTouchUp(Vector3 screenPos)
        {
            if (OnTouchUp != null)
                OnTouchUp(screenPos);
            return false;
        }

        public bool HandleDoubleTouch(Vector3 screenPos)
        {
            if (OnDoubleTouch != null)
                OnDoubleTouch(screenPos);
            return false;
        }

        #endregion

        #region IDragHandler implementation

        public bool HandleDragStart(Vector3 screenPos)
        {
            if (OnDragStart != null)
                OnDragStart(screenPos);
            return false;
        }

        public bool HandleDrag(Vector3 screenPos)
        {
            if (OnDrag != null)
                OnDrag(screenPos);
            return false;
        }

        public bool HandleDragEnd(Vector3 screenPos)
        {
            if (OnDragEnd != null)
                OnDragEnd(screenPos);
            return false;
        }

        #endregion

        #region IPinchHandler implementation

        public bool HandlePinchStart(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            if (OnPinchStart != null)
                OnPinchStart(firstScreenPos, secondScreenPos);
            return false;
        }

        public bool HandlePinch(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            if (OnPinch != null)
                OnPinch(firstScreenPos, secondScreenPos);
            return false;
        }

        public bool HandlePinchEnd(Vector3 firstScreenPos, Vector3 secondScreenPos)
        {
            if (OnPinchEnd != null)
                OnPinchEnd(firstScreenPos, secondScreenPos);
            return false;
        }

        #endregion

        #region ITouchHoldHandler implementation

        public bool HandleTouchHoldStart(Vector3 screenPos, float holdTime)
        {
            if (OnHoldStart != null)
                OnHoldStart(screenPos, holdTime);
            return false;
        }

        public bool HandleTouchHold(Vector3 screenPos, float holdTime)
        {
            if (OnHold != null)
                OnHold(screenPos, holdTime);
            return false;
        }

        public bool HandleTouchHoldEnd(Vector3 screenPos, float holdTime)
        {
            if (OnHoldEnd != null)
                OnHoldEnd(screenPos, holdTime);
            return false;
        }

        #endregion
    }
}