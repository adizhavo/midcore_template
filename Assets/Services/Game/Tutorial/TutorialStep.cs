using Services.Core;
using System.Collections.Generic;

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
        public float maskAlpha;
        public Rect maskSize;
        public TutorialDialogType dialogType;
        public KeyValuePair<string, string> maskGUIView;
        public string dialogText;
        public Rect dialogSize;
        public string dialogCustomImage;
    }
}