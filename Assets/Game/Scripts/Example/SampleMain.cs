using Zenject;
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

namespace Template.Sample
{
    public class SampleMain : MonoBehaviour
    {
        private Systems gameSystems;
        private DiContainer container;
        private SampleCamp camp;

        private void Awake()
        {
            // initialise the di container and installers
            container = new DiContainer();
            CoreServicesInstaller.Install(container);
            GameServiceInstaller.Install(container);

            container.Bind<SampleDataProvider>().AsSingle().NonLazy();

            // add core services
            gameSystems = new Systems()
                .Add(container.Resolve<DatabaseService>())
                .Add(container.Resolve<DataVersionService>())
                .Add(container.Resolve<GestureService>())
                .Add(container.Resolve<AssetManifestReader>())
                .Add(container.Resolve<GUIService>())
                .Add(container.Resolve<SampleDataProvider>());

            gameSystems.Initialize();

            var sampleLevel = container.Resolve<DatabaseService>().Get<string>("player_map");
            var grid = container.Resolve<TILED_MapReader>().TILED_ReadGrid(sampleLevel, new GridSettings());
            container.Resolve<GridService>().Load(grid);
        }

        private void Update()
        {
            gameSystems.Execute();
        }
    }
}