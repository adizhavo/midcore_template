using UnityEngine;
using Services.Core;
using Services.Core.Databinding;
using Services.Core.Databinding.Components;

namespace Services.Game.Tutorial.Bindings
{
  public class DialogBindingComponent : MonoBindingComponent<MonoBehaviour, TutorialDialogType>, BindingComponent<Services.Core.Rect>
  {
      [System.Serializable]
      public class SceneDialogs
      {
          public TutorialDialogType type;
          public GameObject dialog;
      }

      public SceneDialogs[] dialogs;

      private SceneDialogs activeDialog;

      protected override void Bind()
      {
          base.Bind();
          CoreServicesInstaller.Resolve<DataBindingService>().Bind<Services.Core.Rect>(Constants.DATABINDING_TUTORIAL_DIALOG_SIZE_PATH, this);
      }

      #region BindingComponent implementation

      public override void OnValueChanged(string branch, TutorialDialogType value)
      {
          activeDialog = null;
          foreach(var dialog in dialogs)
          {
              bool enable = value.Equals(dialog.type);
              dialog.dialog.SetActive(enable);
              if (enable) activeDialog = dialog;
          }
      }

      public void OnValueChanged(string branch, Services.Core.Rect value)
      {
          if (activeDialog != null && value != null)
          {
              var dialogrect = activeDialog.dialog.GetComponent<RectTransform>();
              if (value != null)
              {
                  dialogrect.SetPositionOfPivot(new Vector2(value.x, value.y));
                  dialogrect.SetWidth(value.width);
                  dialogrect.SetHeight(value.height);
              }
              else
              {
                  dialogrect.SetWidth(0f);
                  dialogrect.SetHeight(0f);
              }
          }
      }

      #endregion
  }
}
