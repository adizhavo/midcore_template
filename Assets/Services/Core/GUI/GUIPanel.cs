using System;
using UnityEngine;

namespace Services.Core.GUI
{
    public class GUIPanel
    {
        public string panelId;
        public string prefabPath;
        public int siblingIndex;

        private GameObject panelView;

        internal GameObject Instantiate(Transform parent, bool worldPositionStays)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
                throw new Exception("[GUIPanel] prefab not found in path " + prefabPath);

            panelView = GameObject.Instantiate<GameObject>(prefab); 
            #if UNITY_EDITOR
            panelView.name = prefab.name;
            #endif
            panelView.transform.SetParent(parent, worldPositionStays);
            panelView.SetActive(false);
            return prefab;
        }

        internal void UpdateSiblingIndex()
        {
            panelView.transform.SetSiblingIndex(siblingIndex);
        }

        internal void Destroy()
        {
            if (panelView != null)
                GameObject.DestroyImmediate(panelView);
        }

        internal T GetView<T>() where T : GUIPanelView
        {
            return panelView != null ? panelView.GetComponent<T>() : null;
        }
    }
}