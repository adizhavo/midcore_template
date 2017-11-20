using UnityEngine;
using Services.Game.Data;
using System.Collections.Generic;

namespace MergeWar.Data
{
    /// <summary>
    /// Will contain all game specifi data structure
    /// </summary>

    [System.Serializable]
    public class FloatRange
    {
        public float min;
        public float max;

        public float GetRange()
        {
            return Random.Range(min, max);
        }
    }

    [System.Serializable]
    public class IntRange
    {
        public int min;
        public int max;

        public int GetRange()
        {
            return Random.Range(min, max);
        }
    }

    public class VFXDataRoot
    {
        public List<VFXData> root;
    }

    public class ObjectDataRoot
    {
        public List<ObjectData> root;   
    }

    public class GameGridObjecDataRoot
    {
        public List<GameGridObjectData> root;
    }

    [System.Serializable]
    public class GameGridObjectData : GridObjectData
    {
        public int level;
        public bool canDrag;
        public string onDragEndCommand;
        public string onTapCommand;
        public string onDiscoverCommand;
        public string onSpawnCommand;
        public string onDestroyCommand;
        public string onTimeoutCommand;
        public FloatRange timeout;
        public string onOrderCompleteCommand;
        public string onOrderUpdateCommand;
        public List<KeyValuePair<string, int>> objectOrderList;
        public List<KeyValuePair<string, int>> typeOrderList;
    }

    [System.Serializable]
    public class GameConfig
    {
        public string id;
        public float cameraInertiaDuration;
        public float cameraZoomSpeed;
        public float cameraInitZoom;
        public FloatRange cameraZoomRange;
        public FloatRange cameraStretchedZoomRange;
    }

    public class CommandDataRoot
    {
        public List<CommandData> root;
    }

    [System.Serializable]
    public class CommandData
    {
        public string id;
        public string type;
        public bool destroyTrigger;
        public string output;
        public int count;
        public string chainedCommand;
        public string vfx;
        public string tutorialTrigger;
    }

    [System.Serializable]
    public class MergeComboDataRoot
    {
        public List<MergeComboData> root;
    }

    [System.Serializable]
    public class MergeComboData
    {
        public string input;
        public string output;
        public string vfx;
        public string mergeCompleteCommand;
    }
}
