using UnityEngine;
using Services.Game.Data;
using System.Collections.Generic;

namespace MergeWar.Data
{
    /// <summary>
    /// Will contain all game specifi data structure
    /// </summary>

    public class FloatRange
    {
        public float min;
        public float max;

        public float GetRange()
        {
            return Random.Range(min, max);
        }
    }

    public class IntRange
    {
        public int min;
        public int max;

        public int GetRange()
        {
            return Random.Range(min, max);
        }
    }

    public class ObjectDataRoot
    {
        public List<ObjectData> root;   
    }

    public class GameGridObjecDataRoot
    {
        public List<GameGridObjecData> root;
    }

    public class GameGridObjecData : GridObjectData
    {
        public bool canDrag;
    }

    public class GameConfig
    {
        public string id;
        public float cameraInertiaDuration;
        public float cameraZoomSpeed;
        public float cameraInitZoom;
        public FloatRange cameraZoomRange;
        public FloatRange cameraStretchedZoomRange;
    }
}
