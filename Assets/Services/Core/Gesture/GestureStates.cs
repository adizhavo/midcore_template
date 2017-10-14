using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Core.Gesture
{
    public enum GestureState
    {
        NO_TOUCH,
        TOUCH_DOWN,
        TOUCH_HOLD,
        TOUCH_UP,
        TOUCH_DRAG,
        TOUCH_PINCH
    }

    public enum GestureEvent
    {
        DOWN,
        HOLD,
        DRAG,
        UP,
        PINCH,
        DOUBLE,
    }
}