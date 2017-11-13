using Entitas;
using Zenject;
using Services.Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Core.GUI
{
    // Will load all panels/states
    // can query panels and states by id
    // can switch states
    // can open/close/validate panels by id
    public class GUIService : IInitializeSystem, ITearDownSystem
    {
        public GUIState currentState
        {
            private set;
            get;
        }

        public List<string> globalActivePanels
        {
            private set;
            get;
        }

        public List<string> stateActivePanels
        {
            private set;
            get;
        }

        public string[] availablePanels
        {
            get
            {
                string[] panels = new string[guiConfig.availablePanels.Length];
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i] = guiConfig.availablePanels[i].panelId;
                }
                return panels;
            }
        }

        public string[] availableStates
        {
            get
            { 
                string[] states = new string[guiConfig.states.Length];
                for (int i = 0; i < states.Length; i++)
                {
                    states[i] = guiConfig.states[i].state;
                }
                return states;
            }
        }

        public GameObject Canvas
        {
            private set;
            get;
        }

        [Inject] private DatabaseService database;
        private GUIConfig guiConfig;
        private string configPath;

        public GUIService()
        {
            stateActivePanels = new List<string>();
            globalActivePanels = new List<string>();
        }

        #region IInitializeSystem implementation

        public void Initialize()
        {
            // Load config and canvas object
            var configPath = database.Get<string>(Constants.DB_KEY_GUI);
            guiConfig = Utils.ReadJsonFromResources<GUIConfig>(configPath);
            var canvasObject = Utils.ReadFromResources<GameObject>(guiConfig.canvasObjectPath);
            Canvas = GameObject.Instantiate<GameObject>(canvasObject);

            // Instatiation
            foreach (var panelObject in guiConfig.availablePanels)
                panelObject.Instantiate(Canvas.transform, false);

            // Set render order / sibling index
            foreach (var panelObject in guiConfig.availablePanels)
                panelObject.UpdateSiblingIndex();

            ChangeState(guiConfig.startState);
        }

        #endregion

        #region ITearDownSystem implementation

        public void TearDown()
        {
            foreach (var panelObject in guiConfig.availablePanels)
                panelObject.Destroy();
        }

        #endregion

        public void ChangeState(string stateId)
        {
            LogWrapper.DebugLog("[{0}] receive command to change state to: {1}", GetType(), stateId);

            if (!IsValidState(stateId))
            {
                LogWrapper.Error("[{0}] State is note valid \"{1}\", nothing will happen", GetType(), stateId);
                return;
            }

            if (currentState == null)
            {
                currentState = GetState(stateId);
                // Reassign active Panels
                stateActivePanels = currentState.GetAllPanels(this);
                ShowStatePanels();
            }
            else if (!currentState.state.Equals(stateId))
            {
                var newState = GetState(stateId);

                // Refresh panels that will remain active
                var refreshPanels = newState.GetFilteredPanels(this, stateActivePanels);
                foreach (var panel in refreshPanels)
                    GetPanelView(panel).OnRefresh();

                // Close panels that will not remain active
                foreach (var panel in stateActivePanels)
                    if (!refreshPanels.Contains(panel))
                        GetPanelView(panel).OnDisappear();
                
                LogWrapper.DebugLog("[{0}] Switching state from: {1} to {2}", GetType(), currentState.state, newState);
                currentState = newState;

                // Reassign active Panels
                stateActivePanels = currentState.GetAllPanels(this);
                ShowStatePanels();
            }

            if (currentState == null)
                LogWrapper.DebugLog("[{0}] Current state is null: \"{1}\", are you passing the right state ? is the state configured ?", GetType(), stateId);
        }

        public string[] GetPanelsInState(string stateId)
        {
            return GetState(stateId).activePanels;
        }

        public bool IsValidState(string stateId)
        {
            return GetState(stateId, true) != null;
        }

        public T GetPanelView<T>(string paneId) where T : GUIPanelView
        {
            return GetPanel(paneId).GetView<T>();
        }

        public GUIPanelView GetPanelView(string paneId)
        {
            return GetPanel(paneId).GetView<GUIPanelView>();
        }

        public void OpenPanelView(string panelId, bool isGlobal = false)
        {
            var activePanels = isGlobal ? globalActivePanels : stateActivePanels;

            if (activePanels.Contains(panelId))
            {
                GetPanel(panelId).GetView<GUIPanelView>().OnRefresh();
            }
            else
            {
                GetPanel(panelId).GetView<GUIPanelView>().OnAppear();
                if (IsValidPanel(panelId))
                    activePanels.Add(panelId);
            }
        }

        public void ClosePanelView(string panelId)
        {
            GetPanel(panelId).GetView<GUIPanelView>().OnDisappear();
            stateActivePanels.Remove(panelId);
            globalActivePanels.Remove(panelId);
        }

        public void RefreshPanelView(string panelId)
        {
            GetPanel(panelId).GetView<GUIPanelView>().OnRefresh();
        }

        public bool IsValidPanel(string panelId)
        {
            return GetPanel(panelId, true) != null;
        }

        public void CloseAllPanels(bool closeAllGlobal = false)
        {
            for (int i = stateActivePanels.Count - 1; i >= 0; i--)
                ClosePanelView(stateActivePanels[i]);

            if (closeAllGlobal)
                for (int i = globalActivePanels.Count - 1; i >= 0; i--)
                    ClosePanelView(globalActivePanels[i]);
        }

        internal GUIState GetState(string stateId, bool silent = false)
        {
            foreach (var stateObject in guiConfig.states)
                if (stateObject.state.Equals(stateId))
                    return stateObject;

            if (!silent)
                LogWrapper.Error("[{0}] Requested GUIState is null {1}", GetType(), stateId);

            return null;
        }

        internal GUIPanel GetPanel(string panelId, bool silent = false)
        {
            foreach (var panelObject in guiConfig.availablePanels)
                if (panelObject.panelId.Equals(panelId))
                    return panelObject;

            if (!silent)
                LogWrapper.Error("[{0}] Requested Panel is null {1}", GetType(), panelId);

            return null;
        }

        private void ShowStatePanels()
        {
            // Enable the rest of the panels
            // Refresh the current active panels marked as global
            foreach (var panel in stateActivePanels)
            {
                if (globalActivePanels.Contains(panel))
                {
                    GetPanelView(panel).OnRefresh();
                }
                else
                {
                    GetPanelView(panel).OnAppear();
                }
            }
        }

        public class GUIConfig
        {
            public string canvasObjectPath;
            public string startState;
            public GUIPanel[] availablePanels;
            public GUIState[] states;
        }
    }
}