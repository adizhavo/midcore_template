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
using Services.Game.HUD;
using Services.Game.Factory;
using Services.Game.SceneCamera;
using Services.Game.Misc;
using MergeWar.Game;
using MergeWar.Game.Systems;

namespace MergeWar
{
    public class Main : MonoBehaviour
    {
        private Systems gameSystems;
        private DiContainer container;
        private CampSystem camp;

        private void Awake()
        {
            #if ADHOC
            SRDebug.Init();
            #endif

            InstallDIContainer();
            InitialiseGesture();
            InitializeGameSystem();

            container.Resolve<CampSystem>().LoadCamp();
        }

        private void Update()
        {
            gameSystems.Execute();
        }

        private void InstallDIContainer()
        {
            // initialise the di container and installers
            container = new DiContainer();
            CoreServicesInstaller.Install(container);
            GameServiceInstaller.Install(container);
            GameSystemInstaller.Install(container);
        }

        private void InitializeGameSystem()
        {
            gameSystems = new Systems()
                // Services
                .Add(container.Resolve<DatabaseService>())
                .Add(container.Resolve<DataVersionService>())
                .Add(container.Resolve<GestureService>())
                .Add(container.Resolve<AssetManifestReader>())
                .Add(container.Resolve<CameraService>())
                .Add(container.Resolve<GUIService>())
                .Add(container.Resolve<HUD_Service>())
                .Add(container.Resolve<FactoryGUI>())
                .Add(container.Resolve<DataProviderSystem>())
                .Add(new AutoDestroySystem())

                // Gameplay
                .Add(container.Resolve<TimedCommandSystem>())
                .Add(container.Resolve<DragSystem>())
                .Add(container.Resolve<PinchSystem>());

            gameSystems.Initialize();
        }

        private void InitialiseGesture()
        {
            var orderSystem = container.Resolve<OrderListSystem>();
            var mergeSystem = container.Resolve<MergeSystem>();
            var tapCommandSystem = container.Resolve<TapCommandSystem>();
            var dragSystem = container.Resolve<DragSystem>();
            var pinchSystem = container.Resolve<PinchSystem>();

            var gestureService = container.Resolve<GestureService>();
            gestureService.AddPinchHandler(pinchSystem);

            gestureService.AddDragHandler(orderSystem);
            gestureService.AddDragHandler(mergeSystem);
            gestureService.AddDragHandler(dragSystem);

            gestureService.AddTouchHandler(dragSystem);
            gestureService.AddTouchHandler(tapCommandSystem);
        }
    }
}