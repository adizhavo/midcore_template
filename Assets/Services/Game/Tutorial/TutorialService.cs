﻿using Zenject;
using Entitas;
using Services.Core;
using Services.Core.Data;
using System.Collections.Generic;

namespace Services.Game.Tutorial
{
    public interface TutorialActionHandler
    {
        void HandleAwakeActions(string[] actions);
        void HandleStartActions(string[] actions);
        void HandleExitActions(string[] actions);
    }

    public class TutorialService<T> : IInitializable, IExecuteSystem 
    where T : TutorialStep
    {
        [Inject] DatabaseService database;

        public static List<TutorialActionHandler> actionHandlers 
        {
            private set;
            get;
        }

        public static List<T> allTutSteps
        {
            private set;
            get;
        }

        public static List<ActiveTutorial<T>> availableTutorials
        {
            private set;
            get;
        }

        public static ActiveTutorial<T> activeTut
        {
            private set;
            get;
        }

        private static TutorialService<T> instance;

        public TutorialService()
        {
            actionHandlers = new List<TutorialActionHandler>();
            instance = this;
        }

        #region IInitializable implementation

        public void Initialize()
        {
            availableTutorials = database.Get<List<ActiveTutorial<T>>>(Constants.TUTORIALS_DB_KEY);
            allTutSteps = database.Get<List<T>>(Constants.TUTORIAL_STEPS_DB_KEY);
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            if (activeTut != null)
                activeTut.Update();
        }

        #endregion

        public static void AddActionHandler(TutorialActionHandler handler)
        {
            if (!actionHandlers.Contains(handler))
                actionHandlers.Add(handler);
        }

        public static void RemoveActionHandler(TutorialActionHandler handler)
        {
            actionHandlers.Remove(handler);
        }

        public static void Notify(string trigger)
        {
            LogWrapper.DebugLog("[{0}] received trigger {1}", instance.GetType(), trigger);

            if (activeTut != null)
            {
                activeTut.Notify(trigger);
            }
            else
            {
                TryLoadTutorial(trigger);
            }
        }

        public static void CompleteTutorial(string id)
        {
            if (activeTut != null && activeTut.id.Equals(id))
            {
                if (!activeTut.hasComplete)
                {
                    activeTut.ForceComplete();
                }

                activeTut = null;
            }

            instance.database.Set("complete_" + id, true, true, true);

            LogWrapper.DebugLog("[{0}] Mark tutorial {1} as completed", instance.GetType(), id);
        }

        private static void TryLoadTutorial(string trigger)
        {
            foreach(var tutorial in availableTutorials)
            {
                bool hasComplete = instance.database.Get<bool>("complete_" + tutorial.id);
                bool hasCompletePrerequisite = string.IsNullOrEmpty(tutorial.prerequisite) || instance.database.Get<bool>("complete_" + tutorial.prerequisite);
                bool matchTrigger = string.Equals(tutorial.trigger, trigger);

                if (hasComplete && hasCompletePrerequisite && matchTrigger)
                {
                    activeTut = tutorial;
                    var steps = allTutSteps.FindAll(s => s.id.Equals(tutorial.id));
                    steps.Sort((s1, s2) => s1.index.CompareTo(s2.index));
                    activeTut.Init(steps.ToArray());
                    break;
                }
            }
        }
    }
}