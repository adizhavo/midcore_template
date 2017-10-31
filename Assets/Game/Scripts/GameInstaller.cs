using Zenject;
using Services.Core;

namespace MergeWar
{
    public class GameInstaller : Installer<GameInstaller>
    {
        #region implemented abstract members of InstallerBase

        public override void InstallBindings()
        {
            Container.Bind<DataProviderSystem>().AsSingle().NonLazy();
            Container.Bind<DragSystem>().AsSingle().NonLazy();
            Container.Bind<CampSystem>().AsSingle().NonLazy();

            LogWrapper.DebugLog("[{0}] installation of sample bindings successfull", GetType());
        }

        #endregion
    }
}