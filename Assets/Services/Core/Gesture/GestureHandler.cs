using UnityEngine;

namespace Services.Core.Gesture
{
    public interface ITouchHandler
    {
        bool HandleTouchDown        (Vector3 screenPos);
        bool HandleTouchUp          (Vector3 screenPos);
        bool HandleDoubleTouch      (Vector3 screenPos);
    }

    public interface IDragHandler
    {
        bool HandleDragStart        (Vector3 screenPos);
        bool HandleDrag             (Vector3 screenPos);
        bool HandleDragEnd          (Vector3 dragEnd);
    }

    public interface IPinchHandler
    {
        bool HandlePinchStart       (Vector3 firstScreenPos, Vector3 secondScreenPos);
        bool HandlePinch            (Vector3 firstScreenPos, Vector3 secondScreenPos);
        bool HandlePinchEnd         (Vector3 firstScreenPos, Vector3 secondScreenPos);
    }

    public interface ITouchHoldHandler
    {
        bool HandleTouchHoldStart   (Vector3 screenPos, float holdTime);
        bool HandleTouchHold        (Vector3 screenPos, float holdTime);
        bool HandleTouchHoldEnd     (Vector3 screenPos, float holdTime);
    }
}