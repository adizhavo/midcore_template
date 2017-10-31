using Zenject;
using Entitas;
using UnityEngine;
using Services.Game.GUI;
using Services.Core.Data;
using Services.Core.GUI;

namespace Services.Game.Factory
{
    /// <summary>
    /// Responsible in creating any kind of GUI object
    /// GUI elements are not entities, from experience, using Entitas for UI objects
    /// makes the development unessecary complicated
    /// </summary>

    public sealed partial class FactoryGUI : IInitializeSystem
    {
        [Inject] DatabaseService database;
        [Inject] GUIService guiService;

        private GameObject floatingUIParent;

        public FactoryGUI()
        {
            floatingUIParent = new GameObject("_FloatingUI");
            floatingUIParent.AddComponent<RectTransform>();
        }

        #region IInitializable implementation

        public void Initialize()
        {
            floatingUIParent.transform.SetParent(guiService.Canvas.transform, false);
        }

        #endregion

        public FloatingUI CreateFloatingUI(string id)
        {
            var prefabPath = database.Get<string>(id);
            var view = FactoryPool.GetPooled(prefabPath);
            view.transform.SetParent(floatingUIParent.transform, false);
            return view.GetComponent<FloatingUI>();
        }

        public FloatingUI AnimateFloatingUIScreenPos(string id, Vector3 fromScreenPos, string panelId, string viewId, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            floatingUI.Move(fromScreenPos, guiService, panelId, viewId, duration);
            return floatingUI;
        }

        public FloatingUI AnimateFloatingUIScreenPos(string id, Vector3 fromScreenPos, RectTransform to, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            floatingUI.Move(fromScreenPos, to, duration);
            return floatingUI;
        }

        public FloatingUI AnimateFloatingUIWorldPos(string id, Vector3 fromWorldPos, string panelId, string viewId, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            var screenPos = Camera.main.WorldToScreenPoint(fromWorldPos);
            floatingUI.Move(screenPos, guiService, panelId, viewId, duration);
            return floatingUI;
        }

        public FloatingUI AnimateFloatingUIWorldPos(string id, Vector3 fromWorldPos, RectTransform to, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            var screenPos = Camera.main.WorldToScreenPoint(fromWorldPos);
            floatingUI.Move(screenPos, to, duration);
            return floatingUI;
        }

        public FloatingUI AnimateFloatingUIWorldPos(string id, GameEntity fromEntity, string panelId, string viewId, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            var screenPos = Camera.main.WorldToScreenPoint(fromEntity.position);
            floatingUI.Move(screenPos, guiService, panelId, viewId, duration);
            return floatingUI;
        }

        public FloatingUI AnimateFloatingUIWorldPos(string id, GameEntity fromEntity, RectTransform to, float duration = 0.5f)
        {
            var floatingUI = CreateFloatingUI(id);
            var screenPos = Camera.main.WorldToScreenPoint(fromEntity.position);
            floatingUI.Move(screenPos, to, duration);
            return floatingUI;
        }
    }
}

