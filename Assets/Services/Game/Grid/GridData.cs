using Entitas;
using Services.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Game.Grid
{
    public class GridData
    {
        public string mapPath;
        public IntVector2 size;
        public GridSettings settings;
        public List<GameEntity> cells = new List<GameEntity>();

        public GridData(string mapPath, IntVector2 size, GridSettings settings)
        {
            this.mapPath = mapPath;
            this.size = size;
            this.settings = settings;
        }
    }

    public enum GridType
    {
        CART,
        ISO
    }

    public class GridSettings
    {
        public FloatVector3 startPos = new FloatVector3(0f, 0f, 0f);
        public FloatVector2 cellSpacing = new FloatVector2(1f, 1f);
        public GridType type = GridType.CART;

        public GridSettings()
        {
        }

        public GridSettings(FloatVector3 startPos, FloatVector2 cellSpacing)
        {
            this.startPos = startPos;
            this.cellSpacing = cellSpacing;
        }
    }
}