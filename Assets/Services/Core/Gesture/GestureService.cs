using Entitas;
using Zenject;
using UnityEngine;
using Services.Core.Data;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Services.Core.Gesture
{
    /// <summary>
    /// Will provider these gestures:
    /// - Touch down
    /// - Touch up
    /// - Double Touch
    /// - Pinch
    /// - Drag
    /// - Hold
    /// Implement the handlers interfaces to listen to the events
    /// or use the gesture event dispatcher 
    /// </summary>

    public class GestureService : IInitializeSystem, IExecuteSystem
    {
        public bool enableDrag = true;
        public bool enableHold = true;
        public bool enableTapDown = true;
        public bool enableTapUp = true;
        public bool enablePinch = true;

        #if !UNITY_EDITOR
        private static int firstFingerId = -1;
        #endif
        private static float tapUpTime = 0f;
        private static float holdTime = 0f;
        private static Vector2 touchPos;
        private static Vector2 touchPosOffset;
        private static Vector2 tapDownPosition;
        private static ApplicationConfig appConfig;

        [Inject] private DatabaseService database;
        private List<Transition> transitions;
        private GestureState current;

        private List<ITouchHandler> touchHandlers = new List<ITouchHandler>();
        private List<ITouchHoldHandler> touchHoldHandlers = new List<ITouchHoldHandler>();
        private List<IDragHandler> dragHandlers = new List<IDragHandler>();
        private List<IPinchHandler> pinchHandlers = new List<IPinchHandler>();

        #region IInitializeSystem implementation

        public void Initialize()
        {
            appConfig = database.Get<ApplicationConfig>(Constants.DB_KEY_APP_CONFIG);
            SetupTransitions();
            SetupGestureEventDispatcher();
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            if (DetectAnyTouch())
            {
                UpdateTouchStats();

                if (enableTapDown && HasTouchedDown())
                {
                    Handle(GestureEvent.DOWN);
                }
                else if (enableTapUp && HasTauchedUp())
                {
                    Handle(HasDoubleTouched() ? GestureEvent.DOUBLE : GestureEvent.UP);
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

            #if UNITY_EDITOR
            if (enablePinch && IsPinching())
            {
                Handle(GestureEvent.PINCH);
            }
            #endif
        }

        #endregion

        #region STATE_HANDLER

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
                    new Transition(GestureState.TOUCH_HOLD,     GestureEvent.PINCH,     GestureState.TOUCH_PINCH,   ()=>{HandleTouchHoldEnd();HandlePinchStart();}),
                    new Transition(GestureState.TOUCH_DRAG,     GestureEvent.DRAG,      GestureState.TOUCH_DRAG,    HandleDrag),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.DOUBLE,    GestureState.NO_TOUCH,      HandleDoubleTouch),
                    new Transition(GestureState.TOUCH_DOWN,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleTouchUp),
                    new Transition(GestureState.TOUCH_HOLD,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleTouchHoldEnd),
                    new Transition(GestureState.TOUCH_PINCH,    GestureEvent.UP,        GestureState.NO_TOUCH,      HandlePinchEnd),
                    new Transition(GestureState.TOUCH_DRAG,     GestureEvent.UP,        GestureState.NO_TOUCH,      HandleDragEnd),
                    new Transition(GestureState.TOUCH_PINCH,    GestureEvent.DRAG,      GestureState.TOUCH_DRAG,    ()=>{HandlePinchEnd();HandleDragStart();}),
                    new Transition(GestureState.TOUCH_DRAG,     GestureEvent.PINCH,     GestureState.TOUCH_PINCH,   ()=>{HandleDragCancel();HandlePinchStart();})
                };
        }

        private void Handle(GestureEvent gestureEvent)
        {
            foreach (var transition in transitions)
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

        private void SetupGestureEventDispatcher()
        {
            var gestureEventDispatcher = new GestureEventDispatcher();
            AddTouchHandler(gestureEventDispatcher);
            AddTouchHoldHandler(gestureEventDispatcher);
            AddDragHandler(gestureEventDispatcher);
            AddPinchHandler(gestureEventDispatcher);
        }

        #endregion

        #region INPUT_PROCESSING

        private static void UpdateTouchStats()
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

        public static bool IsOnUI()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            return raycastResults.Count > 0;
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

        public static bool HasTouchedDown()
        {
            return
                #if UNITY_EDITOR
                Input.GetMouseButtonDown(0);
            #else
                Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Began);
            #endif
        }

        public static bool HasTauchedUp()
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
            return !IsPerformingDrag() && !IsPinching() && holdTime > appConfig.holdMinElapseTime &&
                #if UNITY_EDITOR
            Input.GetMouseButton(0);
            #else
                Input.touchCount > 0;
            #endif
        }

        public static bool HasDoubleTouched()
        {
            CheckDoubleTouchElapseTime();

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

        private static void CheckDoubleTouchElapseTime()
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

        private void ResetTouchStats()
        {
            #if !UNITY_EDITOR
            firstFingerId = -1;
            #endif
            touchPosOffset = Vector2.zero;
            holdTime = 0f;
        }

        #endregion

        #region INPUT_HANDLER

        public void AddTouchHandler(ITouchHandler touchHandler)
        {
            touchHandlers.Add(touchHandler);
        }

        public void AddTouchHoldHandler(ITouchHoldHandler touchHoldHandler)
        {
            touchHoldHandlers.Add(touchHoldHandler);
        }

        public void AddDragHandler(IDragHandler dragHandler)
        {
            dragHandlers.Add(dragHandler);
        }

        public void AddPinchHandler(IPinchHandler pinchHandler)
        {
            pinchHandlers.Add(pinchHandler);
        }

        private void HandleTouchDown()
        {
            tapDownPosition = GetTouchPos();
            foreach (var th in touchHandlers)
                if (th.HandleTouchDown(tapDownPosition))
                    break;
        }

        private void HandleTouchUp()
        {
            foreach (var th in touchHandlers)
                if (th.HandleTouchUp(GetTouchPos()))
                    break;
            ResetTouchStats();
        }

        private void HandleDragStart()
        {
            foreach (var dh in dragHandlers)
                if (dh.HandleDragStart(GetTouchPos()))
                    break;
        }

        private void HandleDrag()
        {
            foreach (var dh in dragHandlers)
                if (dh.HandleDrag(GetTouchPos()))
                    break;
        }

        private void HandleDragEnd()
        {
            foreach (var dh in dragHandlers)
                if (dh.HandleDragEnd(GetTouchPos()))
                    break;
            ResetTouchStats();
        }

        private void HandleDragCancel()
        {
            foreach (var dh in dragHandlers)
                if (dh.HandleDragCancel(GetTouchPos()))
                    break;
            ResetTouchStats();
        }

        private void HandleDoubleTouch()
        {
            foreach (var th in touchHandlers)
                if (th.HandleDoubleTouch(GetTouchPos()))
                    break;
            ResetTouchStats();
        }

        private void HandlePinchStart()
        {
            foreach (var ph in pinchHandlers)
                #if UNITY_EDITOR
                if (ph.HandlePinchStart(Input.mousePosition, Input.mousePosition))
                #else
                if (ph.HandlePinchStart(Input.GetTouch(0).position, Input.GetTouch(1).position))
                #endif
                    break;
        }

        private void HandlePinch()
        {
            foreach (var ph in pinchHandlers)
                #if UNITY_EDITOR
                if (ph.HandlePinch(Input.mousePosition, Input.mousePosition))
                #else
                if (ph.HandlePinch(Input.GetTouch(0).position, Input.GetTouch(1).position))
                #endif
                    break;
        }

        private void HandlePinchEnd()
        {
            foreach (var ph in pinchHandlers)
                #if UNITY_EDITOR
                if (ph.HandlePinchEnd())
                #else
                if (ph.HandlePinchEnd())
                #endif
                    break;
            ResetTouchStats();
        }

        private void HandleTouchHoldStart()
        {
            foreach (var hh in touchHoldHandlers)
                if (hh.HandleTouchHoldStart(GetTouchPos(), holdTime))
                    break;
        }

        private void HandleTouchHold()
        {
            foreach (var hh in touchHoldHandlers)
                if (hh.HandleTouchHold(GetTouchPos(), holdTime))
                    break;
        }

        private void HandleTouchHoldEnd()
        {
            foreach (var hh in touchHoldHandlers)
                if (hh.HandleTouchHoldEnd(GetTouchPos(), holdTime))
                    break;
            ResetTouchStats();
        }

        #endregion
    }

    public class Transition
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