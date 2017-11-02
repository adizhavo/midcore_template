using Zenject;
using Services.Core;
using MergeWar.Game.Systems;

namespace MergeWar.Game
{
    public class GameInstaller : Installer<GameInstaller>
    {
        #region implemented abstract members of InstallerBase

        public override void InstallBindings()
        {
            Container.Bind<DataProviderSystem>().AsSingle().NonLazy();
            Container.Bind<DragSystem>().AsSingle().NonLazy();
            Container.Bind<PinchSystem>().AsSingle().NonLazy();
            Container.Bind<CampSystem>().AsSingle().NonLazy();

            LogWrapper.DebugLog("[{0}] installation of sample bindings successfull", GetType());
        }

        #endregion
    }
}