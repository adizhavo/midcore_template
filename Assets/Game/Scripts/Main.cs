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
using MidcoreTemplate.Game;
using MidcoreTemplate.Game.Systems;
using Services.Game.Tutorial;

namespace MidcoreTemplate
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

            #if ADHOC || UNITY_EDITOR
            SRDebug.Init();
            #endif
            
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
                .Add(container.Resolve<DataBindingInitializerSystem>())
                .Add(container.Resolve<GUIService>())
                .Add(container.Resolve<CameraService>())
                .Add(container.Resolve<HUD_Service>())
                .Add(container.Resolve<FactoryGUI>())
                .Add(container.Resolve<DataProviderSystem>())
                .Add(container.Resolve<TutorialService<TutorialStep>>())
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