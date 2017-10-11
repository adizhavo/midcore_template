using Zenject;
using Entitas;
using Framework.Data;

namespace Framework.DIInstaller
{
    public class DIFrameworkInstaller : InstallerBase
    {
        #region InstallerBase implementation

        /// <summary>
        /// Setup Bindings here for the framework
        /// </summary>

        public override void InstallBindings()
        {
            var dataBase = new Database("game_data_base");
            Container.BindInstance(dataBase);
            Container.QueueForInject(dataBase);

            var dataBooter = new DataBooter("app_config");
            Container.BindInstance(dataBooter);
            Container.QueueForInject(dataBooter);
        }

        #endregion

        public void SetSystemParent(Systems parentSystem)
        {
            parentSystem.Add(Container.Resolve<Database>())
                        .Add(Container.Resolve<DataBooter>());
        }
    }
}