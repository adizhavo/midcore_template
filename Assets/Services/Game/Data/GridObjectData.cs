using System.Collections.Generic;

namespace Services.Game.Data
{
    [System.Serializable]
    public partial class GridObjectData : ObjectData 
    {
        // 2x2 sample footprint data
        // [
        // [1, 1],
        // [1, 1]
        // ]

        // default footprint data for 1x1
        public List<List<int>> footprintData;
        public bool canSwap;
    }
}