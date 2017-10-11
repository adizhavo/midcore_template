using Entitas;
using Framework.Utils;
using System.Collections.Generic;
using System;

namespace Framework.Data
{
    /// <summary>
    /// Will load application/game readonly data and pass it to the data base
    /// </summary>

    public class ApplicationConfig
    {
        public string version;
        public string assetManifestPath;
        public string databasePersistPath;
    }

    public class AssetManifestRoot
    {
        public List<AssetManifest> root;
    }

    public class AssetManifest
    {
        public string id;
        public string path;
        public bool isReadonly;
        public bool loadAtStart;
        public string systemType; // will reflect the loaded object and pass it to the database
    }

    public class DataLoader : IInitializeSystem
    {
        public DataBase dataBase;

        private string appConfigPath;

        public DataLoader(string appConfigPath)
        {
            this.appConfigPath = appConfigPath;
        }

        #region IInitializeSystem implementation

        public void Initialize()
        {
            ReadApplicationConfig();
            ReadAssetManifest();
        }

        #endregion

        private void ReadApplicationConfig()
        {
            var appConfig = Util.ReadJsonFromResources<ApplicationConfig>(appConfigPath);
            dataBase.AddReadonly(Constants.APP_CONFIG_ID, appConfig, false, false);
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