using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Data;
using System;
using System.Collections.Generic;

namespace Services.Core.Gesture
{
    public class GestureService : IInitializeSystem, IExecuteSystem
    {
        private static int firstFingerId = -1;
        private static float tapUpTime = 0f;
        private static float holdTime = 0f;
        private static Vector2 touchPos;
        private static Vector2 touchPosOffset;
        private static Vector2 tapDownPosition;
        private static ApplicationConfig appConfig;

        public bool enableDrag = true;
        public bool enableHold = true;
        public bool enableTapDown = true;
        public bool enableTapUp = true;
        public bool enablePinch = true;

        [Inject] private DatabaseService database;

        private List<Transition> transitions;
        private GestureState current;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            appConfig = database.Get<ApplicationConfig>(Constants.APP_CONFIG_ID);
            SetupTransitions();
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            if (DetectAnyTouch())
            {
                UpdateTouchStats();

                if (enableTapDown && HasTappedDown())
                {
                    Handle(GestureEvent.DOWN);
                }
                else if (enableTapUp && HasTappedUp())
                {
                    Handle(IsDoubleTap() ? GestureEvent.DOUBLE : GestureEvent.UP);
                }
                else if (enableDrag && IsPerformingDrag())
                {
                    Handle(GestureEvent.DRAG);
                }
                else if (enablePinch && IsPinching())
                {
                    Handle(GestureEvent.PINCH);
                }
                else if (enableHold && IsHolding())
                {
                    Handle(GestureEvent.HOLD);
                }
            }
        }

        #endregion

        private void SetupTransitions()
        {
            current = GestureState.NO_TOUCH;

            transitions = new List<Transition>()
                {
                    new Transition(GestureState.NO_TOUCH,       GestureEvent.DOWN,      GestureState.TOUCH_DOWN,    HandleTouchDown),
                    new Transition(GestureState.NO_TOUCH,       GestureEvent.PINCH,     GestureState.TOUCH_PINCH,   HandlePinchStart),
                    new Transition(GestureState.TOUCH_PINCH,    GestureEvent.PINCH,     GestureState.TOUCH_PINCH,   HandlePinch),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.HOLD,      GestureState.TOUCH_HOLD,    HandleTouchHoldStart),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.PINCH,     GestureState.TOUCH_PINCH,   HandlePinchStart),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.DRAG,      GestureState.TOUCH_DRAG,    HandleDragStart),
                    new Transition(GestureState.TOUCH_HOLD,     GestureEvent.HOLD,      GestureState.TOUCH_HOLD,    HandleTouchHold),
                    new Transition(GestureState.TOUCH_HOLD,     GestureEvent.DRAG,      GestureState.TOUCH_DRAG,    ()=>{HandleTouchHoldEnd();HandleDragStart();}),
                    new Transition(GestureState.TOUCH_DRAG,     GestureEvent.DRAG,      GestureState.TOUCH_DRAG,    HandleDrag),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.DOUBLE,    GestureState.NO_TOUCH,      HandleDoubleTouch),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleTouchUp),
                    new Transition(GestureState.TOUCH_HOLD,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleTouchHoldEnd),
                    new Transition(GestureState.TOUCH_PINCH,    GestureEvent.UP,        GestureState.NO_TOUCH,      HandlePinchEnd),
                    new Transition(GestureState.TOUCH_DRAG,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleDragEnd),
                };
        }

        private void Handle(GestureEvent gestureEvent)
        {
            foreach(var transition in transitions)
            {
                if (transition.current.Equals(current) && transition.gestureEvent.Equals(gestureEvent))
                {
                    if (transition.action != null)
                        transition.action();

                    current = transition.next;
                    break;
                }
            }
        }

        private void UpdateTouchStats()
        {
            holdTime += Time.unscaledDeltaTime;
            #if UNITY_EDITOR
            touchPos = Input.mousePosition;
            #else
            if (firstFingerId == -1)
            {
                firstFingerId = Input.GetTouch(0).fingerId;
                touchPos = Input.GetTouch(0).position;
            }
            else
            {
                foreach(var t in Input.touches)
                    if (t.fingerId == firstFingerId)
                        touchPos = t.position;
            }

            if (firstFingerId != Input.GetTouch(0).fingerId)
            {
                touchPosOffset = Input.GetTouch(0).position - touchPos;
            }
            #endif
        }

        public static bool DetectAnyTouch()
        {
            return 
                #if UNITY_EDITOR
                Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0);
                #else
                Input.touchCount > 0;
                #endif
        }

        public static float ScreenToInches(float screenPixels)
        {
            return screenPixels / Screen.dpi;
        }

        public static bool HasTappedDown()
        {
            return
                #if UNITY_EDITOR
                Input.GetMouseButtonDown(0);
                #else
                Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Began);
                #endif
        }

        public static bool HasTappedUp()
        {
            return 
                #if UNITY_EDITOR
                Input.GetMouseButtonUp(0);
                #else
                Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Ended);
                #endif
        }

        public static bool IsHolding()
        {
            return !IsPerformingDrag() && !IsPinching() && holdTime > appConfig.holdMinElapseTime &
                #if UNITY_EDITOR
                Input.GetMouseButton(0);
                #else
                Input.touchCount > 0;
                #endif
        }

        public static bool IsDoubleTap()
        {
            CheckDoubleTapElapseTime();

            if (tapUpTime == -1)
            {
                tapUpTime = Time.realtimeSinceStartup;
                return false;
            }
            else
            {
                float elapseTime = Time.realtimeSinceStartup - tapUpTime;
                tapUpTime = -1;
                return elapseTime > Mathf.Epsilon && elapseTime < appConfig.doubleTapElapseTime;
            }
        }

        private static void CheckDoubleTapElapseTime()
        {
            if (tapUpTime != -1 && Time.realtimeSinceStartup - tapUpTime > appConfig.doubleTapElapseTime)
                tapUpTime = -1;
        }

        public static bool IsPerformingDrag()
        {
            float dragDistance = ScreenToInches((GetTouchPos() - tapDownPosition).magnitude);
            return !IsPinching() && Mathf.Abs(dragDistance) > appConfig.dragMinDistance &&
                #if UNITY_EDITOR
                DetectAnyTouch();
                #else
                DetectAnyTouch() && Input.GetTouch(0).phase.Equals(TouchPhase.Moved);
                #endif
        }

        public static bool IsPinching()
        {
            return 
                #if UNITY_EDITOR
                Input.GetAxis("Mouse ScrollWheel") != 0;
                #else
                Input.touchCount >= 2 && (Input.GetTouch(0).phase.Equals(TouchPhase.Moved) || Input.GetTouch(1).phase.Equals(TouchPhase.Moved));
                #endif
        }

        public static Vector2 GetTouchPos()
        {
            return touchPos + touchPosOffset;
        }

        private void HandleTouchDown()
        {
            tapDownPosition = GetTouchPos();
            Debug.Log("T D");
        }

        private void HandleTouchUp()
        {
            Debug.Log("T U");
            ResetTouchStats();
        }

        private void HandleDragStart()
        {
            Debug.Log("D S");
        }

        private void HandleDrag()
        {
            Debug.Log("D");
        }

        private void HandleDragEnd()
        {
            Debug.Log("D E");
            ResetTouchStats();
        }

        private void HandleDoubleTouch()
        {
            Debug.Log("D T");
            ResetTouchStats();
        }

        private void HandlePinchStart()
        {
            Debug.Log("P S");
        }

        private void HandlePinch()
        {
            Debug.Log("P");
        }

        private void HandlePinchEnd()
        {
            Debug.Log("P E");
            ResetTouchStats();
        }

        private void HandleTouchHoldStart()
        {
            Debug.Log("H S");
        }

        private void HandleTouchHold()
        {
            Debug.Log("H");
        }

        private void HandleTouchHoldEnd()
        {
            Debug.Log("H E");
            ResetTouchStats();
        }

        private void ResetTouchStats()
        {
            firstFingerId = -1;
            touchPosOffset = Vector2.zero;
            holdTime = 0f;
        }

        private class Transition
        {
            public GestureState current;
            public GestureEvent gestureEvent;
            public GestureState next;
            public Action action;

            public Transition(GestureState current, GestureEvent gestureEvent, GestureState next, Action action)
            {
                this.current = current;
                this.gestureEvent = gestureEvent;
                this.next = next;
                this.action = action;
            }
        }
    }
}