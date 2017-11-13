using UnityEngine;
using Services.Core;
using Services.Core.Event;
using System.Collections.Generic;

namespace Services.Game.Tutorial
{
    public class ActiveTutorial<T> where T : TutorialStep
    {
        public enum TutorialState
        {
            Idle, 
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
            get { return stepIndex > steps.Length || stepIndex < 0; }
        }

        public string id;
        public string prerequisite;
        public string trigger;

        private float elapseTime;

        public void Init(T[] steps)
        {
            this.steps = steps;
            stepIndex = 0;
            LogWrapper.DebugLog("[{0}] {1} init with step count {2}", GetType(), id, steps.Length);
            AwakeStep(currentStep);
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
                    if (current.Equals(TutorialState.EnterStep))
                    {
                        StartStep(currentStep);
                    }
                    else if (current.Equals(TutorialState.ExitStep))
                    {
                        ExitStep(currentStep);
                    }

                    current = TutorialState.Idle;
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


                    // fade masks out, check null reference
                    // fade panel out, check null reference
                }
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

            // set masks, check null reference
            // fade masks in
            // set panel, check enum if its none
            // fade panel in
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

            // clear masks, check null reference
            // clear panel, check enum if its none

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
            }
        }

    }
}