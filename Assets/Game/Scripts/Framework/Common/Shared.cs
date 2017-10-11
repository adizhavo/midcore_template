using System.Collections.Generic;

public partial class Constants
{
    // ids
    public const string APP_CONFIG_ID = "app_config";
    public const string DATABASE_ID = "game_database";

    // paths
    public const string APP_CONFIG_PATH = "app_config";
}

namespace Framework
{
    // DataBooter will load this and add it to the database
    public class ApplicationConfig
    {
        public string version;
        public string assetManifestPath;
    }

    // Exported from the sheets
    // DataBooter will loop through the assets and add them to the data base as read-only values
    public class AssetManifest
    {
        public List<Asset> root;
    }

    // DataBooter will load the assets and add it to the database
    public class Asset
    {
        public string id;
        public string path;
    }

    // Used by the database to understand the data and persist it
    [System.Serializable]
    public class MetaData
    {
        public string key;
        public object value;
        public bool isReadonly;
        public bool persist;
    }
}