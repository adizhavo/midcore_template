using UnityEngine;

namespace Services.Core.Gesture
{
    /// <summary>
    /// Implement one of these interfaces if you want to listener to a gesture event
    /// returning false will not consume the event
    /// returning true will consume the event so the gesture service will not notify the coming handlers
    /// </summary>

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
        bool HandleDragEnd          (Vector3 screenPos);
        bool HandleDragCancel       (Vector3 screenPos);
    }

    public interface IPinchHandler
    {
        bool HandlePinchStart       (Vector3 firstScreenPos, Vector3 secondScreenPos);
        bool HandlePinch            (Vector3 firstScreenPos, Vector3 secondScreenPos);
        bool HandlePinchEnd         ();
    }

    public interface ITouchHoldHandler
    {
        bool HandleTouchHoldStart   (Vector3 screenPos, float holdTime);
        bool HandleTouchHold        (Vector3 screenPos, float holdTime);
        bool HandleTouchHoldEnd     (Vector3 screenPos, float holdTime);
    }
}