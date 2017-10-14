using Zenject;
using Entitas;
using System;
using System.Collections.Generic;

namespace Services.Core.Data
{
    /// <summary>
    /// Will load application and assets as readonly data and add it to the database
    /// </summary>

    public class DataBootService : IInitializeSystem
    {
        [Inject] private DatabaseService dataBase;

        private string appConfigPath;

        public DataBootService(string appConfigPath)
        {
            this.appConfigPath = appConfigPath;
        }

        #region IInitializeSystem implementation

        public virtual void Initialize()
        {
            ReadAssetManifest();
        }

        #endregion

        private void ReadAssetManifest()
        {
            var appConfig = dataBase.Get<ApplicationConfig>(Constants.APP_CONFIG_ID);
            var assetManifest = Utils.ReadJsonFromResources<AssetManifest>(appConfig.assetManifestPath);
            foreach (var asset in assetManifest.root)
            {
                dataBase.AddReadonly(asset.id, asset.path, false);

                LogWrapper.DebugLog(string.Format("[{0}] add asset to the database with key: {1}, path: {2}", GetType(), asset.id, asset.path));
            }

            LogWrapper.Log(string.Format("[{0}] assets loaded successfully and all paths added to the database", GetType()));
        }
    }

    public class AssetManifest
    {
        public List<Asset> root;
    }

    public class Asset
    {
        public string id;
        public string path;
    }
}