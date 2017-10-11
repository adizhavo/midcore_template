using System.Collections.Generic;

public partial class Constants
{
    public const string APP_CONFIG_ID = "app_config";
}

namespace Framework
{
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

    [System.Serializable]
    public class MetaData
    {
        public string key;
        public object value;
        public bool isReadonly;
        public bool persist;
    }
}