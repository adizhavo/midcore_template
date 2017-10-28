using Zenject;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Tiled;
using Services.Game.Grid;
using Services.Game.Factory;

namespace Template.Sample
{
    public class SampleCamp 
    {
        [Inject] DatabaseService database;
        [Inject] TILED_MapReader mapReader;
        [Inject] GridService gridService;
        [Inject] FactoryEntity factoryEntity;

        public void LoadCamp()
        {
            // Load grid
            var sampleMap = database.Get<string>("player_map");
            var grid = mapReader.TILED_ReadGrid(sampleMap, new GridSettings());
            gridService.Load(grid);

            // Load grid objects
            var gridObjects = mapReader.TILED_ReadObjects(sampleMap);

            foreach(var gridObject in gridObjects)
            {
                var gridEntity = factoryEntity.CreateGridObject(gridObject.Value);
                gridService.SetEntityOn(gridEntity, gridObject.Key.x, gridObject.Key.y);
                gridEntity.PositionOnCell();
            }
        }
    }
}