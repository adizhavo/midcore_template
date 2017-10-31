using Services.Game.Data;
using System.Collections.Generic;

namespace MergeWar
{
    /// <summary>
    /// Will contain all game specifi data structure
    /// </summary>

    public class ObjectDataRoot
    {
        public List<ObjectData> root;   
    }

    public class GridObjectDataRoot
    {
        public List<GridObjectData> root;
    }

    public class GameConfig
    {
        public string id;
        public Rect cameraBoundaries;
    }

    public class Rect
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }
}
