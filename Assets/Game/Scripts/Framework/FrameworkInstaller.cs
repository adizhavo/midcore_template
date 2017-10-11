using Zenject;
using Entitas;
using Framework.Data;

namespace Framework.DIInstaller
{
    public class FrameworkInstaller : Installer<FrameworkInstaller>
    {
        #region Installer implementation

        /// <summary>
        /// Setup framework's bindings here
        /// </summary>

        public override void InstallBindings()
        {
            var dataBase = new Database(Constants.DATABASE_ID);
            Container.BindInstance(dataBase);
            Container.QueueForInject(dataBase);

            var dataBooter = new DataBooter(Constants.APP_CONFIG_PATH);
            Container.BindInstance(dataBooter);
            Container.QueueForInject(dataBooter);
        }

        #endregion
    }
}