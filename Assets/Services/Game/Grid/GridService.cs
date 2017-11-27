using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Game.Tiled;
using Services.Game.Factory;
using Services.Game.Components;
using System;
using System.Collections.Generic;

namespace Services.Game.Grid
{
    /// <summary>
    /// Load / Unload a grid
    /// Provides grid information and some queries on cells and occupants
    /// </summary>

    public class GridService
    {
        [Inject] FactoryEntity factoryEntity;

        public string activeMap
        {
            get { return grid != null ? grid.mapPath : string.Empty; }
        }

        public IntVector2 size
        {
            get { return grid != null ? grid.size : new IntVector2(0, 0); }
        }

        public GridData grid
        {
            private set;
            get;
        }

        public void Load(GridData grid)
        {
            Unload();
            this.grid = grid;
            PositionGridCellsView();
        }

        public void Unload()
        {
            if (grid != null)
            {
                DestroyCells(grid.cells);
                grid.cells.Clear();
                grid = null;
            }
        }

        public bool IsOccupied(int row, int column)
        {
            return GetCell(row, column).cell.occupant != null;
        }

        public bool IsOccupied(GameEntity cell)
        {
            return cell != null && cell.hasCell && cell.cell.occupant != null;
        }

        public GameEntity GetCell(int row, int column)
        {
            return grid.cells.Find(c => c.hasCell && c.cell.row == row && c.cell.column == column);
        }

        public GameEntity GetCell(Vector3 worldPos)
        {
            float xBoundary = grid.settings.cellSpacing.x;
            float yBoundary = grid.settings.cellSpacing.y;

            if (grid.settings.type.Equals(GridType.ISO))
            {
                var cartX = xBoundary;
                var cartY = yBoundary;
                xBoundary = cartX - cartY;
                yBoundary = cartX + cartY / 2;
            }

            var boundary = new Vector2(Mathf.Abs(xBoundary), Mathf.Abs(yBoundary));

            foreach (var cell in grid.cells)
            {
                float xDistance = Mathf.Abs(worldPos.x - cell.position.x);
                float zDistance = Mathf.Abs(worldPos.z - cell.position.z);
                var distance = new Vector2(xDistance, zDistance);
                if (distance.sqrMagnitude < boundary.sqrMagnitude / 4f)
                {
                    return cell;
                }
            }

            return null;
        }

        public List<GameEntity> GetCells(GameEntity occupant)
        {
            return grid.cells.FindAll(c => c.cell.occupant == occupant);
        }

        public List<GameEntity> GetAllOccupiedCells()
        {
            return grid.cells.FindAll(c => c.cell.occupant != null);
        }

        public List<GameEntity> GetAllEmptyCells()
        {
            return GetCells(null);
        }

        public List<GameEntity> GetAllCells()
        {
            return grid.cells;
        }

        public List<GameEntity> GetAllCellsWithTypeId(string typeId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(c.typeId) && c.typeId.Equals(typeId));
        }

        public List<GameEntity> GetAllCellsWithObjectId(string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(objectId) && !string.IsNullOrEmpty(c.objectId) && c.objectId.Equals(objectId));
        }

        public List<GameEntity> GetAllCellsWith(string typeId, string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(objectId) && !string.IsNullOrEmpty(c.typeId) && !string.IsNullOrEmpty(c.objectId) && c.typeId.Equals(typeId) && c.objectId.Equals(objectId));
        }

        public GameEntity GetClosestCell(Vector3 worldPos, bool empty = false, List<GameEntity> ignore = null)
        {
            if (ignore == null)
                ignore = new List<GameEntity>();

            var cellInPos = GetCell(worldPos);
            if (cellInPos != null && !ignore.Contains(cellInPos) && (!empty || (empty && !IsOccupied(cellInPos))))
                return cellInPos;

            GameEntity selected = null;
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if ((!empty || (empty && !IsOccupied(cell)))
                && !ignore.Contains(cell.cell.occupant))
                {
                    float sqrDistance = (worldPos - cell.position).sqrMagnitude;
                    if (sqrDistance < minSqrDistance)
                    {
                        selected = cell;
                        minSqrDistance = sqrDistance;
                    }
                }
            }
            return selected;
        }


        public GameEntity GetClosestCell(int row, int column, bool empty = false, List<GameEntity> ignore = null)
        {
            return GetClosestCell(GetCell(row, column), empty, ignore != null ? ignore : new List<GameEntity>());
        }

        public GameEntity GetClosestCell(GameEntity from, bool empty = false, List<GameEntity> ignore = null)
        {
            if (ignore == null)
                ignore = new List<GameEntity>();

            GameEntity selected = null;
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if (cell != from
                && (!empty || (empty && !IsOccupied(cell)))
                && !ignore.Contains(cell.cell.occupant))
                {
                    float sqrDistance = Mathf.Pow(from.row - cell.row, 2) + Mathf.Pow(from.column - cell.column, 2);
                    if (sqrDistance < minSqrDistance)
                    {
                        selected = cell;
                        minSqrDistance = sqrDistance;
                    }
                }
            }
            return selected;
        }

        public bool DoesFit(GameEntity entity, int row, int column)
        {
            return DoesFit(entity, GetCell(row, column));
        }

        public bool DoesFit(GameEntity entity, GameEntity pivot)
        {
            if (!entity.hasGrid || pivot == null)
                return false;

            return GetOccupants(pivot, entity.grid.footprint).Count == 0 && !IsThereAnyNullCell(pivot, entity.grid.footprint);
        }

        public List<GameEntity> GetOccupants(GameEntity pivot, Footprint footprint)
        {
            var entities = new List<GameEntity>();

            for (int i = 0; i < footprint.data.Count; i++)
            {
                var foot_column = footprint.data[i];

                for (int j = 0; j < foot_column.Count; j++)
                {
                    if (foot_column[j] == 1 && pivot.hasCell)
                    {
                        var f_row = j + pivot.row;
                        var f_column = i + pivot.column;
                        var f_cell = GetCell(f_row, f_column);

                        if (f_cell != null
                        && IsOccupied(f_cell)
                        && !entities.Contains((GameEntity)f_cell.cell.occupant))
                            entities.Add((GameEntity)f_cell.cell.occupant);
                    }
                }
            }

            return entities;
        }

        public bool IsThereAnyNullCell(GameEntity pivot, Footprint footprint)
        {
            for (int i = 0; i < footprint.data.Count; i++)
            {
                var foot_column = footprint.data[i];

                for (int j = 0; j < foot_column.Count; j++)
                {
                    if (foot_column[j] == 1)
                    {
                        var f_row = j + pivot.row;
                        var f_column = i + pivot.column;
                        var f_cell = GetCell(f_row, f_column);

                        if (f_cell == null) return true;
                    }
                }
            }
            return false;
        }

        public GameEntity GetFitPivotCell(GameEntity entity)
        {
            return grid.cells.Find(c => DoesFit(entity, c));
        }

        public GameEntity GetClosestFitPivotCell(GameEntity entity, int row, int column, List<GameEntity> ignoreCells = null)
        {
            return GetClosestFitPivotCell(entity, GetCell(row, column), ignoreCells);
        }

        public GameEntity GetClosestFitPivotCell(GameEntity entity, GameEntity from, List<GameEntity> ignoreCells = null)
        {
            if (ignoreCells == null)
                ignoreCells = new List<GameEntity>();

            GameEntity selected = null;
            float minSqrDistance = float.MaxValue;

            if (from != null)
            {
                foreach (var cell in grid.cells)
                {
                    if (cell != from && !ignoreCells.Contains(cell) && DoesFit(entity, cell))
                    {
                        float sqrDistance = Mathf.Pow(from.row - cell.row, 2) + Mathf.Pow(from.column - cell.column, 2);
                        if (sqrDistance < minSqrDistance)
                        {
                            selected = cell;
                            minSqrDistance = sqrDistance;
                        }
                    }
                }
            }

            return selected;
        }

        public void SetEntityOn(GameEntity entity, int row, int column)
        {
            SetEntityOn(entity, GetCell(row, column));
        }

        public void SetEntityOn(GameEntity entity, GameEntity pivot, bool tweenToCell = true)
        {
            if (DoesFit(entity, pivot))
            {
                Attach(entity, pivot);
                if (tweenToCell) entity.TweenToCell();
                else entity.PositionOnCell();
            }
            else if (entity.hasGrid)
            {
                var occupants = GetOccupants(pivot, entity.grid.footprint);
                var canSwap = occupants.Find(oc => oc.grid.canSwap) != null;
                if (occupants.Count > 1 || !canSwap)
                {
                    var fit = GetClosestFitPivotCell(entity, pivot);
                    if (fit == null)
                    {
                        throw new NullReferenceException("There is not enough space to position the entity");
                    }
                    else
                    {
                        Attach(entity, fit);
                        if (tweenToCell) entity.TweenToCell();
                        else entity.PositionOnCell();
                    }
                }
                else if (occupants.Count == 1)
                {
                    var occupant = occupants[0];
                    var fit = GetClosestFitPivotCell(occupant, pivot, GetAllCells(pivot, entity.grid.footprint));
                    if (fit == null)
                    {
                        fit = GetClosestFitPivotCell(entity, pivot);
                        if (fit == null)
                        {
                            throw new NullReferenceException("There is not enough space to position the entity");
                        }
                        else
                        {
                            Attach(entity, fit);
                            entity.TweenToCell();
                        }
                    }
                    else
                    {
                        Attach(occupant, fit);
                        Attach(entity, pivot);
                        if (tweenToCell)
                        {
                            occupant.TweenToCell();
                            entity.TweenToCell();
                        }
                        else
                        {
                            occupant.PositionOnCell();
                            entity.PositionOnCell();
                        }
                    }
                }
            }
        }

        public List<GameEntity> GetAllCells(GameEntity pivot, Footprint footprint)
        {
            var cells = new List<GameEntity>();

            for (int i = 0; i < footprint.data.Count; i++)
            {
                var foot_column = footprint.data[i];

                for (int j = 0; j < foot_column.Count; j++)
                {
                    if (foot_column[j] == 1)
                    {
                        var f_row = j + pivot.row;
                        var f_column = i + pivot.column;
                        var f_cell = GetCell(f_row, f_column);
                        cells.Add(f_cell);
                    }
                }
            }

            return cells;
        }

        public void DeAttach(GameEntity entity)
        {
            if (entity.hasGrid)
            {
                if (entity.grid.pivot != null && entity.grid.cells.Count > 0)
                {
                    for (int i = 0; i < entity.grid.footprint.data.Count; i++)
                    {
                        var foot_column = entity.grid.footprint.data[i];

                        for (int j = 0; j < foot_column.Count; j++)
                        {
                            if (foot_column[j] == 1)
                            {
                                var f_row = j + entity.grid.pivot.row;
                                var f_column = i + entity.grid.pivot.column;
                                var cell = GetCell(f_row, f_column);
                                if (cell != null)
                                    cell.cell.occupant = null;
                            }
                        }
                    }

                    entity.grid.pivot = null;
                    entity.grid.cells.Clear();
                }
            }
        }

        // Will force position the entity to the cell, ingoring if its occupied or not
        // Suggested to use rarely if you know what you're doing
        // Use 'SetEntityOn' instead. It will resolve all the positioning
        public void Attach(GameEntity entity, GameEntity cell)
        {
            if (entity.hasGrid && DoesFit(entity, cell))
            {
                DeAttach(entity);

                entity.grid.pivot = cell;

                for (int i = 0; i < entity.grid.footprint.data.Count; i++)
                {
                    var foot_column = entity.grid.footprint.data[i];

                    for (int j = 0; j < foot_column.Count; j++)
                    {
                        if (foot_column[j] == 1)
                        {
                            var f_row = j + entity.grid.pivot.row;
                            var f_column = i + entity.grid.pivot.column;
                            var f_cell = GetCell(f_row, f_column);
                            f_cell.cell.occupant = entity;
                            entity.grid.cells.Add(f_cell);
                        }
                    }
                }
            }
        }

        public void DestroyCells(List<GameEntity> cells)
        {
            for (int i = cells.Count - 1; i >= 0; i --)
            {
                var cell = cells[i];
                if (cell.cell.occupant != null && cell.cell.occupant.hasGrid)
                {
                    DeAttach(cell.cell.occupant);
                    cell.cell.occupant.Destroy();
                }

                cell.Destroy();
                this.grid.cells.Remove(cell);
            }
        }

        // Will add grid at runtime with settings of the current grid
        public void AddGrid(GridData grid, GridAnchor anchor)
        {
            var offsets = GetCellOffsets(grid, anchor);

            foreach(var cell in grid.cells)
            {
                cell.cell.row += offsets.x;
                cell.cell.column += offsets.y;

                this.grid.cells.Add(cell);
            }

            PositionGridCellsView();
        }

        public IntVector2 GetCellOffsets(GridData grid, GridAnchor anchor)
        {
            switch (anchor)
            {
                case GridAnchor.TOP : return new IntVector2(0, -1 * grid.size.y);
                case GridAnchor.BOTTOM : return new IntVector2(0, this.grid.size.y);
                case GridAnchor.RIGHT : return new IntVector2(this.grid.size.x, 0);
                case GridAnchor.LEFT : return new IntVector2(-1 * grid.size.x, 0);
            }
            return new IntVector2(0, 0);
        }

        private void PositionGridCellsView()
        {
            foreach (var cell in grid.cells)
            {
                float x = grid.settings.startPos.x + cell.cell.row * grid.settings.cellSpacing.x;
                float y = grid.settings.startPos.y - cell.cell.column * grid.settings.cellSpacing.y;

                if (grid.settings.type.Equals(GridType.ISO))
                {
                    var cartX = x;
                    var cartY = y;
                    x = cartX - cartY;
                    y = cartX + cartY / 2;
                }

                cell.xPosition = x;
                cell.zPosition = y;
            }
        }
    }
}
