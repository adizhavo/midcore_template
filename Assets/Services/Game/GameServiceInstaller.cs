using Zenject;
using Services.Core;
using Services.Game.Tiled;

namespace Services.Game
{
	public class GameServiceInstaller : Installer<GameServiceInstaller>
	{
		#region Installer implementation

		/// <summary>
		/// Setup framework's bindings
		/// </summary>

		public override void InstallBindings()
		{
			Container.Bind<TiledAdapter>().AsSingle().NonLazy();

			LogWrapper.DebugLog("[{0}] installation of game bindings successfull", GetType());
		}

		#endregion
	}
}