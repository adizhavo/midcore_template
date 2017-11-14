using UnityEngine;
using Services.Core;
using Services.Core.Event;
using System.Collections.Generic;
using Services.Core.GUI;
using Services.Core.Databinding;
using Rect = Services.Core.Rect;

namespace Services.Game.Tutorial
{
    /// <summary>
    /// Active tutorial that contains all teh step data
    /// Handle activation/deactivation of the panels and databinding
    /// </summary>

    public class ActiveTutorial<T> where T : TutorialStep
    {
        public enum TutorialState
        {
            Idle, 
            AwakeStep,
            EnterStep,
            ExitStep
        }

        private TutorialState current;
        private int stepIndex;
        private T[] steps;

        private T currentStep
        {
            get { return steps != null && steps.Length > 0 ? steps[stepIndex] : null; }
        }

        public string currentSucctrigger
        {
            get { return steps != null && steps.Length > 0 ? steps[stepIndex].successTrigger : string.Empty; }
        }

        public bool hasComplete
        {
            get { return stepIndex >= steps.Length || stepIndex < 0; }
        }

        public string id;
        public string prerequisite;
        public string trigger;

        private float elapseTime;
        private GUIService guiService;
        private DataBindingService databinding;
        private GUIPanelView panelView;
        private LTDescr panelAnimation;

        public void Init(T[] steps, GUIService guiService, DataBindingService databinding)
        {
            this.steps = steps;
            this.guiService = guiService;
            this.databinding = databinding;

            stepIndex = 0;
            LogWrapper.DebugLog("[{0}] {1} init with step count {2}", GetType(), id, steps.Length);

            // setup the awake function
            current = TutorialState.AwakeStep;
            elapseTime = 0f;

            panelView = guiService.GetPanelView(Constants.PANEL_VIEW_ID_TUTORIAL);
            if (panelView == null)
            {
                throw new System.NullReferenceException("Could not find tutorial panel, please condif a panle in the gui config file with id: " + Constants.PANEL_VIEW_ID_TUTORIAL);
            }

            EventDispatcherService<ActiveTutorial<T>>.Dispatch(Constants.EVENT_TUT_INIT, this);
        }

        public void ForceComplete()
        {
            LogWrapper.DebugLog("[{0}] {1} force complete at index {2}", GetType(), id, currentStep.index);

            foreach(var step in steps)
                ExitStep(step, false);
        }

        public virtual void Update()
        {
            if (elapseTime > -1 * Mathf.Epsilon)
            {
                elapseTime -= Time.unscaledDeltaTime;

                if (elapseTime < 0f)
                {
                    if (current.Equals(TutorialState.AwakeStep))
                    {
                        AwakeStep(currentStep);
                    }
                    else if (current.Equals(TutorialState.EnterStep))
                    {
                        StartStep(currentStep);
                        current = TutorialState.Idle;
                    }
                    else if (current.Equals(TutorialState.ExitStep))
                    {
                        ExitStep(currentStep);
                    }
                }
            }
        }

        public virtual void Notify(string trigger)
        {
            if (current.Equals(TutorialState.Idle) && !hasComplete)
            {
                if (currentStep.successTrigger.Equals(trigger))
                {
                    LogWrapper.DebugLog("[{0}] {1} Match step index {2} succ condition {3}", GetType(), id, currentStep.index, currentStep.successTrigger);
                    elapseTime = currentStep.exitAnimationLength;
                    current = TutorialState.ExitStep;
                    AnimateOutTutorialUI();
                }
            }
            else
            {
                LogWrapper.DebugLog("[{0}] {1} received event when not in state {2}, will ignore the event", GetType(), id, current.ToString());
            }
        }

        protected virtual void AwakeStep(T step)
        {
            LogWrapper.DebugLog("[{0}] {1} awake tutorial step with index {2}", GetType(), id, currentStep.index);

            current = TutorialState.EnterStep;
            elapseTime = currentStep.entryAnimLength;

            foreach(var handler in TutorialService<T>.actionHandlers)
                handler.HandleAwakeActions(currentStep.awakeActions);

            if (currentStep.pause) UnityEngine.Time.timeScale = 0f;

            EnableTutorialUI();
            guiService.OpenPanelView(Constants.PANEL_VIEW_ID_TUTORIAL, true);
            AnimateInTutorialUI();
        }

        protected virtual void StartStep(T step)
        {
            LogWrapper.DebugLog("[{0}] {1} start tutorial step with index {2}", GetType(), id, currentStep.index);

            foreach(var handler in TutorialService<T>.actionHandlers)
                handler.HandleAwakeActions(currentStep.startActions);
        }

        protected virtual void ExitStep(T step, bool moveNext = true)
        {
            LogWrapper.DebugLog("[{0}] {1} exit tutorial step with index {2}", GetType(), id, currentStep.index);

            foreach(var handler in TutorialService<T>.actionHandlers)
                handler.HandleAwakeActions(currentStep.exitActions);

            if (currentStep.pause) UnityEngine.Time.timeScale = 1f;

            DisableTutorialUI();
            guiService.ClosePanelView(Constants.PANEL_VIEW_ID_TUTORIAL);

            if (moveNext)
                MoveToNextStep();
        }

        private void MoveToNextStep()
        {
            if (!hasComplete)
            {
                EventDispatcherService<T>.Dispatch(Constants.EVENT_TUT_STEP_COMPLETE, currentStep);

                stepIndex++;

                if (hasComplete)
                {
                    TutorialService<T>.CompleteTutorial(id);
                }
                else
                {
                    current = TutorialState.AwakeStep;
                    elapseTime = 0f;
                }
            }
        }

        private void EnableTutorialUI()
        {
            databinding
                .AddData<bool>(Constants.DATABINDING_TUTORIAL_ENABLE_PATH, true, true)
                .AddData<Rect>(Constants.DATABINDING_TUTORIAL_MASK_PATH, currentStep.maskProperty, true)
                .AddData<KeyValuePair<string, string>>(Constants.DATABINDING_TUTORIAL_MASK_VIEW_PATH, currentStep.maskGUIView, true)
                .AddData<TutorialDialogType>(Constants.DATABINDING_TUTORIAL_DIALOG_PATH, currentStep.dialogType, true)
                .AddData<string>(Constants.DATABINDING_TUTORIAL_DIALOG_TEXT_PATH, currentStep.dialogText, true);
        }

        private void DisableTutorialUI()
        {
            databinding
                .AddData<bool>(Constants.DATABINDING_TUTORIAL_ENABLE_PATH, false, true)
                .AddData<Rect>(Constants.DATABINDING_TUTORIAL_MASK_PATH, null, true)
                .AddData<KeyValuePair<string, string>>(Constants.DATABINDING_TUTORIAL_MASK_VIEW_PATH, currentStep.maskGUIView, true)
                .AddData<TutorialDialogType>(Constants.DATABINDING_TUTORIAL_DIALOG_PATH, TutorialDialogType.NONE, true)
                .AddData<string>(Constants.DATABINDING_TUTORIAL_DIALOG_TEXT_PATH, string.Empty, true);
        }

        private void AnimateInTutorialUI()
        {
            if (panelAnimation != null)
            {
                LeanTween.cancel(panelAnimation.uniqueId, false);
            }

            var rectTransform = panelView.GetComponent<RectTransform>();
            panelAnimation = LeanTween.value(0f, 1f, currentStep.entryAnimLength)
                .setIgnoreTimeScale(true)
                .setOnUpdate((value) =>
                {
                    if (!hasComplete)
                    {
                        var canvasGroup = rectTransform.GetComponent<CanvasGroup>();
                        if (canvasGroup != null) canvasGroup.alpha = value;
                        else rectTransform.SetAlpha(value, true);
                        panelView.GetView<GUIRectView>("mask").rectTransform.SetAlpha(value * currentStep.maskAlpha, true);
                    }
                });
        }

        private void AnimateOutTutorialUI()
        {
            if (panelAnimation != null)
            {
                LeanTween.cancel(panelAnimation.uniqueId, false);
            }

            var rectTransform = panelView.GetComponent<RectTransform>();
            panelAnimation = LeanTween.value(1f, 0f, currentStep.exitAnimationLength)
                .setIgnoreTimeScale(true)
                .setOnUpdate((value) =>
                {
                    if (!hasComplete)
                    {
                        var canvasGroup = rectTransform.GetComponent<CanvasGroup>();
                        if (canvasGroup != null) canvasGroup.alpha = value;
                        else rectTransform.SetAlpha(value, true);
                        panelView.GetView<GUIRectView>("mask").rectTransform.SetAlpha(value * currentStep.maskAlpha, true);
                    }
                });
        }
    }
}