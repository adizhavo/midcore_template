using System;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Game.Tiled;
using System.Collections.Generic;
using Services.Game.Components;

namespace Services.Game.Grid
{
    /// <summary>
    /// Load / Unload a grid 
    /// Provides grid information and some queries on cells and occupants
    /// </summary>

    public class GridService
    {
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

        // WIP
        public void Unload()
        {
            if (grid != null)
            {
                // DeAttach occupants
                // Destroy cells view

                grid.cells.Clear();
                grid = null;
            }
        }

        public bool IsOccupied(int row, int column)
        {
            return GetCell(row, column).occupant != null;
        }

        public bool IsOccupied(Cell cell)
        {
            return cell.occupant != null;
        }

        public bool IsNull(int row, int column)
        {
            return GetCell(row, column) is NullCell;
        }

        public bool IsNull(Cell cell)
        {
            return cell is NullCell;
        }

        public Cell GetCell(int row, int column)
        {
            var cell = grid.cells.Find(c => c.row == row && c.column == column);
            return cell != null ? cell : default(NullCell);
        }

        public List<Cell> GetCells(Entity occupant)
        {
            return grid.cells.FindAll(c => c.occupant == occupant);
        }

        public List<Cell> GetAllOccupiedCells()
        {
            return grid.cells.FindAll(c => c.occupant != null);
        }

        public List<Cell> GetAllEmptyCells()
        {
            return GetCells(null);
        }

        public List<Cell> GetAllCells()
        {
            return grid.cells;
        }

        public List<Cell> GetAllCellsWithTypeId(string typeId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && c.typeId.Equals(typeId)); 
        }

        public List<Cell> GetAllCellsWithObjectId(string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(objectId) && c.objectId.Equals(objectId)); 
        }

        public List<Cell> GetAllCellsWith(string typeId, string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(objectId) && c.typeId.Equals(typeId) && c.objectId.Equals(objectId)); 
        }

        public Cell GetClosestCell(int row, int column, bool empty = false, List<Entity> ignore = null)
        {
            return GetClosestCell(GetCell(row, column), empty, ignore != null ? ignore : new List<Entity>());
        }

        public Cell GetClosestCell(Cell from, bool empty = false, List<Entity> ignore = null)
        {
            if (ignore == null)
                ignore = new List<Entity>();

            Cell selected = default(NullCell);
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if (cell != from
                    && !IsNull(cell)
                    && (!empty || (empty && !IsOccupied(cell)))
                    && !ignore.Contains(cell.occupant))
                {
                    if (IsNull(selected))
                    {
                        selected = cell;
                    }
                    else
                    {
                        float sqrDistance = Mathf.Pow(selected.row - cell.row, 2) + Mathf.Pow(selected.column - cell.column, 2);
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

        public bool DoesFit(GameEntity entity, int row, int column)
        {
            return DoesFit(entity, GetCell(row, column));
        }

        public bool DoesFit(GameEntity entity, Cell pivot)
        {
            if (!entity.hasGrid)
                return false;
                
            return GetOccupants(pivot, entity.grid.footprint).Count == 0 && !IsThereAnyNullCell(pivot, entity.grid.footprint);
        }

        public List<GameEntity> GetOccupants(Cell pivot, Footprint footprint)
        {
            var entities = new List<GameEntity>();

            for (int i = 0; i < footprint.data.GetLength(0); i++)
            {
                var f_row = footprint.data[i, 0] + pivot.row;
                var f_column = footprint.data[i, 1] + pivot.column;
                var f_cell = GetCell(f_row, f_column);

                if (!IsNull(f_cell) 
                    && IsOccupied(f_cell) 
                    && !entities.Contains((GameEntity)f_cell.occupant))
                    entities.Add((GameEntity)f_cell.occupant);
            }

            return entities;
        }

        public bool IsThereAnyNullCell(Cell pivot, Footprint footprint)
        {
            for (int i = 0; i < footprint.data.GetLength(0); i++)
            {
                var f_row = footprint.data[i, 0] + pivot.row;
                var f_column = footprint.data[i, 1] + pivot.column;
                var f_cell = GetCell(f_row, f_column);

                if (IsNull(f_cell))
                    return true;
            }
            return false;
        }

        public Cell GetFitPivotCell(GameEntity entity)
        {
            var cell = grid.cells.Find(c => DoesFit(entity, c));
            return cell != null ? cell : default(NullCell);
        }

        public Cell GetClosestFitPivotCell(GameEntity entity, int row, int column)
        {
            return GetClosestFitPivotCell(entity, GetCell(row, column));
        }

        public Cell GetClosestFitPivotCell(GameEntity entity, Cell from)
        {
            Cell selected = default(NullCell);
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if (cell != from
                    && !IsNull(cell)
                    && DoesFit(entity, cell))
                {
                    if (IsNull(selected))
                    {
                        selected = cell;
                    }
                    else
                    {
                        float sqrDistance = Mathf.Pow(selected.row - cell.row, 2) + Mathf.Pow(selected.column - cell.column, 2);
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

        public void SetEntityOn(GameEntity entity, Cell pivot)
        {
            if (DoesFit(entity, pivot))
            {
                Attach(entity, pivot);
            }
            else
            {
                var occupants = GetOccupants(pivot, entity.grid.footprint);
                if (occupants.Count > 1)
                {
                    var fit = GetClosestFitPivotCell(entity, pivot);
                    if (IsNull(fit))
                    {
                        throw new NullReferenceException("There is not enough space to position the entity");
                    }
                    else
                    {
                        Attach(entity, fit);
                    }
                }
                else if (occupants.Count == 1)
                {
                    var occupant = occupants[0];
                    var fit = GetClosestFitPivotCell(occupant, pivot); 
                    if (IsNull(fit))
                    {
                        throw new NullReferenceException("There is not enough space to position the entity");
                    }
                    else
                    {
                        Attach(occupant, fit);
                        Attach(entity, pivot);
                    }
                }
            }
        }

        public void DeAttach(GameEntity entity)
        {
            if (entity.hasGrid)
            {
                if (entity.grid.pivot != null && entity.grid.cells.Count > 0)
                {
                    for (int i = 0; i < entity.grid.footprint.data.GetLength(0); i++)
                    {
                        var f_row = entity.grid.footprint.data[i, 0] + entity.grid.pivot.row;
                        var f_column = entity.grid.footprint.data[i, 1] + entity.grid.pivot.column;

                        GetCell(f_row, f_column).occupant = null;
                    }

                    entity.grid.pivot = null;
                    entity.grid.cells.Clear();
                }
            }
        }

        // Will force position the entity to the cell, ingoring if its occupied or not
        // Suggested to use rarely if you know what you're doing
        // Use 'SetEntityOn' instead. It will resolve all the positioning
        public void Attach(GameEntity entity, Cell cell)
        {
            if (entity.hasGrid && DoesFit(entity, cell))
            {
                DeAttach(entity);

                for (int i = 0; i < entity.grid.footprint.data.GetLength(0); i++)
                {
                    var f_row = entity.grid.footprint.data[i, 0] + entity.grid.pivot.row;
                    var f_column = entity.grid.footprint.data[i, 1] + entity.grid.pivot.column;
                    var f_cell = GetCell(f_row, f_column);
                    f_cell.occupant = entity;
                    entity.grid.cells.Add(f_cell);
                }

                entity.grid.pivot = cell;
            }
        }

        // WIP
        private void PositionGridCellsView()
        {
            foreach (var cell in grid.cells)
            {
                // destroy the cell current view
                // create cell GameObject
                // position cell GamObjects based on settings
            }
        }
    }
}