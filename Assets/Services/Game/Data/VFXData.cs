using System;

namespace Services.Game.Data
{
    [System.Serializable]
    public class VFXData
    {
        public string id;
        public string prefab;
        public float activeTime;
        public bool ignoreTimescale;
    }
}

