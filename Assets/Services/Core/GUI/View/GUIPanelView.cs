using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Services.Core.GUI
{
    public class GUIPanelView : MonoBehaviour
    {
        private List<IGUIController> controllers = new List<IGUIController>();
        private List<IGUIView> views = new List<IGUIView>();

        protected virtual void Awake()
        {
            transform.GetComponentsInChildren<IGUIView>(true, views);

            foreach (var view in views)
                view.Init();
        }

        public GUIPanelView AssignController(IGUIController c)
        {
            if (!controllers.Contains(c))
                controllers.Add(c);

            return this;
        }

        public GUIPanelView RemoveController(IGUIController c)
        {
            if (controllers.Contains(c))
                controllers.Remove(c);

            return this;
        }

        public T GetView<T>(string id) where T : MonoBehaviour
        {
            foreach (var v in views)
                if (string.Equals(v.GetId(), id))
                    return (T)v;

            LogWrapper.Error("[{0}] Requested view is null: {1}", GetType(), id);

            return null;
        }

        public MonoBehaviour GetView(string id)
        {
            return GetView<MonoBehaviour>(id);
        }

        internal void OnRefresh()
        {
            foreach (var c in controllers)
                c.OnRefresh();
        }

        internal void OnAppear()
        {
            gameObject.SetActive(true);

            foreach (var c in controllers)
                c.OnAppear();
        }

        internal void OnDisappear()
        {
            foreach (var c in controllers)
                c.OnDisappear();

            gameObject.SetActive(false);
        }
    }
}
