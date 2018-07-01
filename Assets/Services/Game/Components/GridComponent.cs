using Entitas;
using Services.Game.Grid;
using System.Collections.Generic;

namespace Services.Game.Components
{
    [Game]
    public class GridComponent : IComponent
    {
        public GameEntity pivot;
        // will contain the pivot cell
        public List<GameEntity> cells;
        public Footprint footprint;
		public bool canSwap;
    }

    public class Footprint
    {
        // 2x2 sample footprint data
        // [
        // [1, 1],
        // [1, 1]
        // ]

        // default footprint data for 1x1
        public List<List<int>> data = new List<List<int>>()
        {
            new List<int>() { 1 }
        };

        public Footprint() { }

        public Footprint(List<List<int>> data)
        {
            this.data = data;
        }
    }
}
