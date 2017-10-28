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
                foreach(var cell in grid.cells)
                {
                    if (cell.cell.occupant != null)
                    {
                        DeAttach(cell.cell.occupant);
                        cell.cell.occupant.Destroy();
                    }

                    cell.Destroy();
                }

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
            return cell.hasCell && cell.cell.occupant != null;
        }

        public GameEntity GetCell(int row, int column)
        {
            return grid.cells.Find(c => c.cell.row == row && c.cell.column == column);
        }

        public List<GameEntity> GetCells(Entity occupant)
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
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && c.typeId.Equals(typeId)); 
        }

        public List<GameEntity> GetAllCellsWithObjectId(string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(objectId) && c.objectId.Equals(objectId)); 
        }

        public List<GameEntity> GetAllCellsWith(string typeId, string objectId)
        {
            return grid.cells.FindAll(c => !string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(objectId) && c.typeId.Equals(typeId) && c.objectId.Equals(objectId)); 
        }

        public GameEntity GetClosestCell(int row, int column, bool empty = false, List<Entity> ignore = null)
        {
            return GetClosestCell(GetCell(row, column), empty, ignore != null ? ignore : new List<Entity>());
        }

        public GameEntity GetClosestCell(GameEntity from, bool empty = false, List<Entity> ignore = null)
        {
            if (ignore == null)
                ignore = new List<Entity>();

            GameEntity selected = null;
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if (cell != from
                    && (!empty || (empty && !IsOccupied(cell)))
                    && !ignore.Contains(cell.cell.occupant))
                {
                    if (selected == null)
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

        public bool DoesFit(GameEntity entity, GameEntity pivot)
        {
            if (!entity.hasGrid)
                return false;
                
            return GetOccupants(pivot, entity.grid.footprint).Count == 0 && !IsThereAnyNullCell(pivot, entity.grid.footprint);
        }

        public List<GameEntity> GetOccupants(GameEntity pivot, Footprint footprint)
        {
            var entities = new List<GameEntity>();

            for (int i = 0; i < footprint.data.GetLength(0); i++)
            {
                var f_row = footprint.data[i, 0] + pivot.row;
                var f_column = footprint.data[i, 1] + pivot.column;
                var f_cell = GetCell(f_row, f_column);

                if (f_cell != null
                    && IsOccupied(f_cell) 
                    && !entities.Contains((GameEntity)f_cell.cell.occupant))
                    entities.Add((GameEntity)f_cell.cell.occupant);
            }

            return entities;
        }

        public bool IsThereAnyNullCell(GameEntity pivot, Footprint footprint)
        {
            for (int i = 0; i < footprint.data.GetLength(0); i++)
            {
                var f_row = footprint.data[i, 0] + pivot.row;
                var f_column = footprint.data[i, 1] + pivot.column;
                var f_cell = GetCell(f_row, f_column);

                if (f_cell == null) return true;
            }
            return false;
        }

        public GameEntity GetFitPivotCell(GameEntity entity)
        {
            return grid.cells.Find(c => DoesFit(entity, c));
        }

        public GameEntity GetClosestFitPivotCell(GameEntity entity, int row, int column)
        {
            return GetClosestFitPivotCell(entity, GetCell(row, column));
        }

        public GameEntity GetClosestFitPivotCell(GameEntity entity, GameEntity from)
        {
            GameEntity selected = null;
            float minSqrDistance = float.MaxValue;

            foreach (var cell in grid.cells)
            {
                if (cell != from
                    && DoesFit(entity, cell))
                {
                    if (selected == null)
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

        public void SetEntityOn(GameEntity entity, GameEntity pivot)
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
                    if (fit == null)
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
                    if (fit == null)
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

                        GetCell(f_row, f_column).cell.occupant = null;
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

                for (int i = 0; i < entity.grid.footprint.data.GetLength(0); i++)
                {
                    var f_row = entity.grid.footprint.data[i, 0] + entity.grid.pivot.row;
                    var f_column = entity.grid.footprint.data[i, 1] + entity.grid.pivot.column;
                    var f_cell = GetCell(f_row, f_column);
                    f_cell.cell.occupant = entity;
                    entity.grid.cells.Add(f_cell);
                }
            }
        }

        private void PositionGridCellsView()
        {
            foreach (var cell in grid.cells)
            {
                cell.position = grid.settings.startPos.ToVector3();
                cell.xPosition += cell.cell.row * grid.settings.cellSpacing.x;
                cell.zPosition -= cell.cell.column * grid.settings.cellSpacing.y;
                #if UNITY_EDITOR
                cell.viewObject.name = string.Format("cell_{0}_{1}_{2}_{3}_{4}", cell.objectId, cell.typeId, cell.row, cell.column, cell.uniqueId);
                #endif
            }
        }
    }
}