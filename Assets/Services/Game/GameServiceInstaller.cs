using Zenject;
using Services.Core;
using Services.Game.Tiled;
using Services.Game.Grid;

namespace Services.Game
{
    public class GameServiceInstaller : Installer<GameServiceInstaller>
    {
        #region Installer implementation

        /// <summary>
        /// Setup game's bindings
        /// </summary>

        public override void InstallBindings()
        {
            Container.Bind<TILED_DataProvider>().AsSingle().NonLazy();
            Container.Bind<TILED_MapReader>().AsSingle().NonLazy();
            Container.Bind<CellFactory>().AsSingle().NonLazy();

            LogWrapper.DebugLog("[{0}] installation of game bindings successfull", GetType());
        }

        #endregion
    }
}