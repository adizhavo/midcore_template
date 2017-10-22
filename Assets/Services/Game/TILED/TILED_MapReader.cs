using Zenject;
using Services.Core;
using Services.Game.Grid;
using System.Collections.Generic;

namespace Services.Game.Tiled
{
    public class TILED_MapReader
    {
        [Inject] TILED_DataProvider tiledDataProvider;
        [Inject] CellFactory cellFactory;

        public GridData TILED_ReadGrid(string mapPath, GridSettings settings = default(GridSettings))
        {
            var mapSize = tiledDataProvider.GetMapSize(mapPath);
            var grid = new GridData(mapPath, mapSize, settings);
            var tileLayer = tiledDataProvider.GetMapLayer(mapPath, "Tiles");
            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                var gid = tileLayer.data[i];
                if (gid != 0) // 0 is mapped to a void cell
                {
                    var typeId = tiledDataProvider.GetMapTilesetTileTypeId(mapPath, gid);
                    var objectId = tiledDataProvider.GetMapTilesetObjectId(mapPath, gid);
                    var cell = cellFactory.GetCell(typeId);
                    cell.x = i % mapSize.x;
                    cell.y = (i / mapSize.x) % mapSize.y;
                    cell.objectId = objectId;
                    grid.cells.Add(cell);
                }
            }
            return grid;
        }

        public Dictionary<IntVector2, string> TILED_ReadObjects(string mapPath)
        {
            var gridObjects = new Dictionary<IntVector2, string>();

            var mapSize = tiledDataProvider.GetMapSize(mapPath);
            var objectLayer = tiledDataProvider.GetMapLayer(mapPath, "Objects");
            for (int i = 0; i < objectLayer.data.Length; i++)
            {
                var gid = objectLayer.data[i];
                if (gid != 0) // 0 is empty
                {
                    var objectId = tiledDataProvider.GetMapTilesetObjectId(mapPath, gid);
                    var x = i % mapSize.x;
                    var y = (i / mapSize.x) % mapSize.y;
                    gridObjects.Add(new IntVector2(x, y), objectId);
                }
            }

            return gridObjects;
        }
    }
}