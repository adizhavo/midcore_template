using System;
using UnityEngine;
using System.Collections.Generic;

namespace Services.Core.GUI
{
    [System.Serializable]
    public class GUIState
    {
        public string state;
        public string[] additiveStates;
        public string[] activePanels;

        internal List<string> GetAllPanels(GUIService gramGUI)
        {
            List<string> allPanels = new List<string>();

            if (additiveStates != null)
            {
                foreach(var stateId in additiveStates)
                {
                    var result = gramGUI.GetState(stateId).GetAllPanels(gramGUI);
                    foreach(var panel in result)
                        if (!allPanels.Contains(panel))
                            allPanels.Add(panel);
                }
            }

            foreach(var panelId in activePanels)
            {
                if (!allPanels.Contains(panelId))
                    allPanels.Add(panelId);
            }

            return allPanels;
        }

        internal List<string> GetFilteredPanels(GUIService gramGUI, List<string> filter)
        {
            List<string> collected = new List<string>();
            if (additiveStates != null)
            {
                foreach(var stateId in additiveStates)
                {
                    var result = gramGUI.GetState(stateId).GetFilteredPanels(gramGUI, filter);
                    foreach(var panel in result)
                        if (!collected.Contains(panel))
                            collected.Add(panel);
                }
            }

            foreach(var panelId in activePanels)
            {
                if (filter.Contains(panelId) && !collected.Contains(panelId))
                    collected.Add(panelId);
            }

            return collected;
        }
    }
}