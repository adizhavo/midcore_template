using Zenject;
using Entitas;
using Framework.Data;

namespace Framework.DIInstaller
{
    public class CoreServicesInstaller : Installer<CoreServicesInstaller>
    {
        #region Installer implementation

        /// <summary>
        /// Setup framework's bindings here
        /// </summary>

        public override void InstallBindings()
        {
            var dataBase = new DatabaseService(Constants.DATABASE_ID);
            Container.BindInstance(dataBase);
            Container.QueueForInject(dataBase);

            var dataBooter = new DataBootService(Constants.APP_CONFIG_PATH);
            Container.BindInstance(dataBooter);
            Container.QueueForInject(dataBooter);

            LogWrapper.DebugLog(string.Format("[{0}] installation of bindings successfull", GetType()));
        }

        #endregion
    }
}