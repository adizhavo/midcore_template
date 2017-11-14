using UnityEngine;
using Services.Core.Databinding;
using Services.Core.Databinding.Components;

namespace Services.Game.Tutorial.Bindings
{
    public class DialogBindingComponent : MonoBindingComponent<MonoBehaviour, TutorialDialogType>
    {
        [System.Serializable]
        public class SceneDialogs
        {
            public TutorialDialogType type;
            public GameObject dialog;
        }

        public SceneDialogs[] dialogs;

        #region BindingComponent implementation

        public override void OnValueChanged(string branch, TutorialDialogType value)
        {
            foreach(var dialog in dialogs)
            {
                dialog.dialog.SetActive(value.Equals(dialog.type));
            }
        }

        #endregion
    }
}