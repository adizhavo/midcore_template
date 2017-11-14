using UnityEngine;
using Services.Core.Databinding.Components;

namespace Services.Game.Tutorial.Bindings
{
    public class TutorialButtonBindingComponent : ButtonInteractionBindingComponent
    {
        protected override void Start()
        {
            base.Start();
            component.onClick.AddListener(() => TutorialService<TutorialStep>.Notify("move_next"));
        }
    }
}
