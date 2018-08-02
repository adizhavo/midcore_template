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

        public delegate void TouchUp(Vector3 screenPos);

        public delegate void DoubleTouch(Vector3 screenPos);

        public delegate void DragStart(Vector3 screenPos);

        public delegate void Drag(Vector3 screenPos);

        public delegate void DragEnd(Vector3 screenPos);

        public delegate void DragCancel(Vector3 screenPos);

        public delegate void PinchStart(Vector3 firstScreenPos, Vector3 secondScreenPos);

        public delegate void Pinch(Vector3 firstScreenPos, Vector3 secondScreenPos);

        public delegate void PinchEnd();

        public delegate void HoldStart(Vector3 screenPos, float holdTime);

        public delegate void Hold(Vector3 screenPos, float holdTime);

        public delegate void HoldEnd(Vector3 screenPos, float holdTime);

        public static event TouchUp OnTouchUp;

        public static event TouchDown OnTouchDown;

        public static event DoubleTouch OnDoubleTouch;

        public static event DragStart OnDragStart;

        public static event Drag OnDrag;

        public static event DragEnd OnDragEnd;

        public static event DragCancel OnDragCancel;

        public static event PinchStart OnPinchStart;

        public static event Pinch OnPinch;

        public static event PinchEnd OnPinchEnd;

        public static event HoldStart OnHoldStart;

        public static event Hold OnHold;

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

        public bool HandleDragCancel(Vector3 screenPos)
        {
            if (OnDragCancel != null)
                OnDragCancel(screenPos);
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

        public bool HandlePinchEnd()
        {
            if (OnPinchEnd != null)
                OnPinchEnd(); 
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