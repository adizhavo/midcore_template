using Services.Core;

namespace Services.Game.Tutorial
{
    public enum TutorialDialogType
    {
        NONE,
        TUT_INTERACTIVE_DIALOG,
        TUT_INFO_DIALOG
    }

    public class TutorialStep 
    {
        public string id;
        public int index;
        public bool pause;
        public string successTrigger;
        public float entryAnimLength;
        public float exitAnimationLength;
        public string[] awakeActions;
        public string[] startActions;
        public string[] exitActions;
        public Rect[] masks;
        public TutorialDialogType dialogType;
        public string dialogText;
    }
}