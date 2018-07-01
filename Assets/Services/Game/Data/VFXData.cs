using System;

namespace Services.Game.Data
{
    [System.Serializable]
    public partial class VFXData
    {
        public string id;
        public string prefab;
        public float activeTime;
        public bool ignoreTimescale;
        public bool isGUI;
        public string moveToPanel;
    }
}

