﻿using Zenject;
using UnityEngine;
using Services.Core;
using Services.Core.Data;
using Services.Core.Databinding;
using Services.Game.Tiled;
using Services.Game.Grid;
using Services.Game.Factory;
using Services.Game.HUD;
using Services.Game.SceneCamera;
using MidcoreTemplate.Data;

namespace MidcoreTemplate.Game.Systems
{
    public class CampSystem 
    {
        [Inject] DatabaseService database;
        [Inject] TILED_MapReader mapReader;
        [Inject] GridService gridService;
        [Inject] CameraService cameraService;
        [Inject] DataProviderSystem dataProvider;
        [Inject] CommandSystem commandSystem;

        public void LoadCamp()
        {
            // Load grid
            var sampleMap = database.Get<string>("player_map_data");
            var gridSettings = new GridSettings() { type = GridType.ISO, cellSpacing = new FloatVector2(0.5f, 0.7f) };
            var grid = mapReader.TILED_ReadGrid(sampleMap, gridSettings);
            gridService.Load(grid);

            // Load grid objects
            var gridObjects = mapReader.TILED_ReadObjects(sampleMap);

            foreach(var gridObject in gridObjects)
            {
                var command = new CommandData();
                command.type = Constants.COMMAND_SPAWN_OBJ;
                command.output = gridObject.Value;
                command.count = 1;

                var cell = gridService.GetCell(gridObject.Key.x, gridObject.Key.y);
                commandSystem.Execute(command, cell.position, cell);
            }

            // setup camera
            cameraService.SetBoundary(dataProvider.GetGameConfig().cameraInitPos.ToVector3(), dataProvider.GetGameConfig().cameraBoundaryRadius);
            cameraService.SetZoom(dataProvider.GetGameConfig().cameraInitZoom);
            cameraService.SetPosition(dataProvider.GetGameConfig().cameraInitPos.ToVector3());
        }
    }
}