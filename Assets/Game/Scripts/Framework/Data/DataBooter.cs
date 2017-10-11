using Entitas;
using Framework.Utils;
using System.Collections.Generic;
using System;

namespace Framework.Data
{
    /// <summary>
    /// Will load application/game readonly data and enter it in the database
    /// </summary>

    public class DataBooter : IInitializeSystem
    {
        public Database dataBase;

        private string appConfigPath;

        public DataBooter(string appConfigPath)
        {
            this.appConfigPath = appConfigPath;
        }

        #region IInitializeSystem implementation

        public virtual void Initialize()
        {
            ReadApplicationConfig();
            ReadAssetManifest();
        }

        #endregion

        private void ReadApplicationConfig()
        {
            var appConfig = Util.ReadJsonFromResources<ApplicationConfig>(appConfigPath);
            dataBase.AddReadonly(Constants.APP_CONFIG_ID, appConfig, false);
        }

        private void ReadAssetManifest()
        {
            var appConfig = dataBase.Get<ApplicationConfig>(Constants.APP_CONFIG_ID);
            var assetManifest = Util.ReadJsonFromResources<AssetManifestRoot>(appConfig.assetManifestPath);
            var assets = assetManifest.root.FindAll(asset => asset.loadAtStart);

            foreach (var asset in assets)
            {
                var type = string.IsNullOrEmpty(asset.systemType) ? typeof(object) : Type.GetType(asset.systemType);
                var data = Util.ReadFromResources(asset.path, type);

                if (asset.isReadonly)
                {
                    // This data will not persist in the database since it will be loaded every time the application boots
                    // Add to the database so its available to the game at runtime
                    dataBase.AddReadonly(asset.id, data, false);
                }
                else
                {
                    dataBase.Set(asset.id, data);
                }
            }

            dataBase.Flush();
        }
    }
}