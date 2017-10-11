using Entitas;
using Framework.Utils;
using System.Collections.Generic;
using System;
using Framework.Log;

namespace Framework.Data
{
    /// <summary>
    /// Will load application and assets as readonly data and add it to the database
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

            LogWrapper.Log(string.Format("{0} loaded app config succesfully and added to the database with key: {1}", this.GetType(), Constants.APP_CONFIG_ID));
        }

        private void ReadAssetManifest()
        {
            var appConfig = dataBase.Get<ApplicationConfig>(Constants.APP_CONFIG_ID);
            var assetManifest = Util.ReadJsonFromResources<AssetManifest>(appConfig.assetManifestPath);
            foreach (var asset in assetManifest.assets)
            {
                dataBase.AddReadonly(asset.id, asset.path, false);

                LogWrapper.DebugLog(string.Format("{0} add asset to the database with key: {1}, path: {2}", this.GetType(), asset.id, asset.path));
            }

            LogWrapper.Log(string.Format("{0} assets loaded successfully and all paths added to the database", this.GetType()));
        }
    }
}