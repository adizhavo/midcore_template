using Zenject;
using Entitas;
using Services.Core.Data;
using Services.Core.Databinding;
using Services.Core.DataVersion;

namespace Services.Core
{
    public class CoreServicesInstaller : Installer<CoreServicesInstaller>
    {
        #region Installer implementation

        /// <summary>
        /// Setup framework's bindings
        /// </summary>

        public override void InstallBindings()
        {
            var DBService = new DatabaseService(Constants.DATABASE_ID);
            Container.BindInstance(DBService);
            Container.QueueForInject(DBService);

            var dataBootService = new DataBootService(Constants.APP_CONFIG_PATH);
            Container.BindInstance(dataBootService);
            Container.QueueForInject(dataBootService);

            Container.Bind<DataBindingService>().AsSingle().NonLazy();

            var dataVersionService = new DataVersionService();

            // --- Example how to add data migrator blocks
            // --- each block should extend the DataMigratorBlock class
            // ---
            // Container.Bind<DataMigratorBlock_1_0_0()>().AsSingle().NonLazy();
            // Container.Bind<DataMigratorBlock_2_0_0()>().AsSingle().NonLazy();
            // dataVersionService.AddMigratorBlock(Container.Resolve<DataMigratorBlock_1_0_0>().SetVersion("1.0.0"));
            // dataVersionService.AddMigratorBlock(Container.Resolve<DataMigratorBlock_2_0_0>().SetVersion("2.0.0"));
            // --- end

            Container.BindInstance(dataVersionService);
            Container.QueueForInject(dataVersionService);

            LogWrapper.DebugLog(string.Format("[{0}] installation of bindings successfull", GetType()));
        }

        #endregion
    }
}