using Zenject;
using Services.Core;
using Services.Game.Grid;
using Services.Game.Factory;
using System;
using System.Collections.Generic;

namespace Services.Game.Tiled
{
    public class TILED_MapReader
    {
        [Inject] TILED_DataProvider tiledDataProvider;
        [Inject] FactoryEntity factoryEntity;

        public GridData TILED_ReadGrid(string mapPath, GridSettings settings)
        {
            var mapSize = tiledDataProvider.GetMapSize(mapPath);
            var grid = new GridData(mapPath, mapSize, settings);
            var tileLayer = tiledDataProvider.GetMapLayer(mapPath, Constants.TILED_TILE_LAYER);

            if (tileLayer == null)
                throw new NullReferenceException("Could not find layer in the map " + mapPath + " with name " + Constants.TILED_TILE_LAYER + ", please add a layer with that name.");

            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                var gid = tileLayer.data[i];
                if (gid != 0) // 0 is mapped to a void cell
                {
                    var objectId = tiledDataProvider.GetMapTilesetObjectId(mapPath, gid);
                    var cell = factoryEntity.CreateCell(i % mapSize.x, (i / mapSize.x) % mapSize.y, objectId);
                    grid.cells.Add(cell);
                }
            }
            return grid;
        }

        public Dictionary<IntVector2, string> TILED_ReadObjects(string mapPath)
        {
            var gridObjects = new Dictionary<IntVector2, string>();

            var mapSize = tiledDataProvider.GetMapSize(mapPath);
            var objectLayer = tiledDataProvider.GetMapLayer(mapPath, Constants.TILED_OBJECTS_LAYER);

            if (objectLayer == null)
                throw new NullReferenceException("Could not find layer in the map " + mapPath + " with name " + Constants.TILED_OBJECTS_LAYER + ", please add a layer with that name.");

            for (int i = 0; i < objectLayer.data.Length; i++)
            {
                var gid = objectLayer.data[i];
                if (gid != 0) // 0 is empty
                {
                    var objectId = tiledDataProvider.GetMapTilesetObjectId(mapPath, gid);
                    var row = i % mapSize.x;
                    var column = (i / mapSize.x) % mapSize.y;
                    gridObjects.Add(new IntVector2(row, column), objectId);
                }
            }

            return gridObjects;
        }
    }
}