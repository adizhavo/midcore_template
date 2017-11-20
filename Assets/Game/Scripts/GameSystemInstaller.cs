using Zenject;
using Services.Core;
using MergeWar.Game.Systems;
using MergeWar.Game.Command;

namespace MergeWar.Game
{
    public class GameSystemInstaller : Installer<GameSystemInstaller>
    {
        private static GameSystemInstaller instance;

        public static T Resolve<T>()
        {
            return instance.Container.Resolve<T>();
        }

        public GameSystemInstaller()
        {
            instance = this;
        }

        #region implemented abstract members of InstallerBase

        public override void InstallBindings()
        {
            Container.Bind<DataProviderSystem>().AsSingle().NonLazy();
            Container.Bind<SceneSystem>().AsSingle().NonLazy();
            Container.Bind<DragSystem>().AsSingle().NonLazy();
            Container.Bind<PinchSystem>().AsSingle().NonLazy();
            Container.Bind<TapCommandSystem>().AsSingle().NonLazy();
            Container.Bind<CampSystem>().AsSingle().NonLazy();
            Container.Bind<TimedCommandSystem>().AsSingle().NonLazy();
            Container.Bind<MergeSystem>().AsSingle().NonLazy();
            Container.Bind<OrderListSystem>().AsSingle().NonLazy();

            var commandSystem = new CommandSystem();
            Container.BindInstance(commandSystem);
            Container.QueueForInject(commandSystem);

            // start -- command addition
            Container.Bind<SpawnCommand>().AsSingle().NonLazy();

            commandSystem.AddCommand(Constants.COMMAND_SPAWN_OBJ, Container.Resolve<SpawnCommand>());
            // -- end

            LogWrapper.DebugLog("[{0}] installation of sample bindings successfull", GetType());
        }

        #endregion
    }
}