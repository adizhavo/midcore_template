using Zenject;
using Entitas;
using System.Collections.Generic;

namespace Services.Core.Data
{
    /// <summary>
    /// Will load the asset manifest and add them to the database
    /// </summary>

    public class AssetManifestReader : IInitializeSystem
    {
        [Inject] private DatabaseService dataBase;

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

                LogWrapper.DebugLog("[{0}] add asset to the database with key: {1}, path: {2}", GetType(), asset.id, asset.path);
            }

            LogWrapper.Log("[{0}] assets loaded successfully and all paths added to the database", GetType());
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