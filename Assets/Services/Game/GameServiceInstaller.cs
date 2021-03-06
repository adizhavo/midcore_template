﻿using Zenject;
using Services.Core;
using Services.Game.Tiled;
using Services.Game.Grid;
using Services.Game.Factory;
using Services.Game.HUD;
using Services.Game.SceneCamera;
using Services.Game.Tutorial;

namespace Services.Game
{
    public class GameServiceInstaller : Installer<GameServiceInstaller>
    {
        private static GameServiceInstaller instance;

        public static T Resolve<T>()
        {
            return instance.Container.Resolve<T>();
        }

        public GameServiceInstaller()
        {
            instance = this;
        }

        #region Installer implementation

        /// <summary>
        /// Setup game's bindings
        /// </summary>

        public override void InstallBindings()
        {
            Container.Bind<TILED_DataProvider>().AsSingle().NonLazy();
            Container.Bind<TILED_MapReader>().AsSingle().NonLazy();
            Container.Bind<FactoryEntity>().AsSingle().NonLazy();
            Container.Bind<FactoryGUI>().AsSingle().NonLazy();
            Container.Bind<GridService>().AsSingle().NonLazy();
            Container.Bind<HUD_Service>().AsSingle().NonLazy();
            Container.Bind<CameraService>().AsSingle().NonLazy();
            Container.Bind<TutorialService<TutorialStep>>().AsSingle().NonLazy();

            LogWrapper.DebugLog("[{0}] installation of game bindings successfull", GetType());
        }

        #endregion
    }
}