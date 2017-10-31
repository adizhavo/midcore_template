﻿using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;
using Services.Core.DataVersion;
using Services.Core.Event;
using Services.Core.Gesture;
using Services.Core.GUI;
using Services.Game;
using Services.Game.Tiled;
using Services.Game.Grid;
using Services.Game.HUD;
using Services.Game.Factory;

namespace MergeWar
{
    public class Main : MonoBehaviour
    {
        private Systems gameSystems;
        private DiContainer container;
        private CampSystem camp;

        private void Awake()
        {
            InstallDIContainer();
            InitialiseGesture();
            InitializeGameSystem();

            container.Resolve<CampSystem>().LoadCamp();
        }

        private void Update()
        {
            gameSystems.Execute();

            if ((int)Time.timeSinceLevelLoad % 2 == 0)
            {
                var fromEntity = Contexts.sharedInstance.game.GetEntities(GameMatcher.Grid)[0];
                container.Resolve<FactoryGUI>().AnimateFloatingUIWorldPos("sample_floating_UI", fromEntity, "sample_panel_id", "sample_view_id");
            }
        }

        private void InstallDIContainer()
        {
            // initialise the di container and installers
            container = new DiContainer();
            CoreServicesInstaller.Install(container);
            GameServiceInstaller.Install(container);
            GameInstaller.Install(container);
        }

        private void InitializeGameSystem()
        {
            // add core services
            gameSystems = new Systems()
                .Add(container.Resolve<DatabaseService>())
                .Add(container.Resolve<DataVersionService>())
                .Add(container.Resolve<GestureService>())
                .Add(container.Resolve<AssetManifestReader>())
                .Add(container.Resolve<GUIService>())
                .Add(container.Resolve<HUD_Service>())
                .Add(container.Resolve<FactoryGUI>())
                .Add(container.Resolve<DataProviderSystem>());

            gameSystems.Initialize();
        }

        private void InitialiseGesture()
        {
            var dragSystem = new DragSystem();

            var gestureService = container.Resolve<GestureService>();
            gestureService.AddDragHandler(dragSystem);
        }
    }
}