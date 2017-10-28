using Zenject;
using Services.Core;

namespace Template.Sample
{
    public class SampleInstaller : Installer<SampleInstaller>
    {
        #region implemented abstract members of InstallerBase

        public override void InstallBindings()
        {
            Container.Bind<SampleDataProvider>().AsSingle().NonLazy();
            Container.Bind<SampleCamp>().AsSingle().NonLazy();

            LogWrapper.DebugLog("[{0}] installation of sample bindings successfull", GetType());
        }

        #endregion
    }
}