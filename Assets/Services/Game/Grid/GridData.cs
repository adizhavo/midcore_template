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
        public List<Cell> cells = new List<Cell>();

        public GridData(string mapPath, IntVector2 size, GridSettings settings)
        {
            this.mapPath = mapPath;
            this.size = size;
            this.settings = settings;
        }
    }

    public class Cell
    {
        public int row;
        public int column;
        public string typeId;
        public string objectId;
        public GameEntity occupant;
        public GameEntity entity;

        public Cell(string typeId)
        {
            this.typeId = typeId;
        }
    }

    public class NullCell : Cell
    {
        public NullCell() : base(string.Empty)
        {
            row = -1;
            column = -1;
        }
    }

    public class GridSettings
    {
        public FloatVector3 startPos = new FloatVector3(0f, 0f, 0f);
        public FloatVector2 cellSpacing = new FloatVector2(1f, 1f);

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