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
        public string[] awakeActions = new string[0];
        public string[] startActions = new string[0];
        public string[] exitActions = new string[0];
        public float maskAlpha;
        public Rect maskSize;
        public TutorialDialogType dialogType;
        public KeyValuePair<string, string> maskGUIView;
        public string dialogText;
        public Rect dialogSize;
        public UIAnchor maskAnchor;
        public string dialogCustomImage;
    }
}
