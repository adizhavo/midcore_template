using Services.Core;
using System.Collections.Generic;

namespace Services.Game.Grid
{
    public class GridSettings
    {
        public enum GridPivot
        {
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT,
            CENTER
        }

        public FloatVector3 startPos = new FloatVector3(0f, 0f, 0f);
        public FloatVector2 cellSpacing = new FloatVector2(1f, 1f);
        public GridPivot pivot = GridPivot.TOP_LEFT;

        public GridSettings()
        {
        }

        public GridSettings(FloatVector3 startPos, FloatVector2 cellSpacing, GridPivot pivot)
        {
            this.startPos = startPos;
            this.cellSpacing = cellSpacing;
            this.pivot = pivot;
        }
    }

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
}