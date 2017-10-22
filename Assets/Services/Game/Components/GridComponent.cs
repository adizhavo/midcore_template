using Entitas;
using Services.Game.Grid;
using System.Collections.Generic;

namespace Services.Game.Components
{
    [Game]
    public class GridComponent : IComponent
    {
        public Cell pivot;
        // will contain the pivot cell
        public List<Cell> cells;
        public Footprint footprint;
    }

    public class Footprint
    {
        // default footprint data for 1x1
        // int[,] -> { {0, 0} }

        // 2x2 sample footprint data
        // int[,] -> {
        //             {0, 0},
        //             {0, 1},
        //             {1, 0},
        //             {1, 1},
        //           }
        public int[,] data;
    }
}