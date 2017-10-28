using Entitas;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Services.Core.DataVersion;

namespace Services.Core.Data
{
    /// <summary>
    /// Will store all runtime data from the game and start up data from the DataBooter
    /// </summary>

    public class DatabaseService : IInitializeSystem
    {
        private List<MetaData> metaData = new List<MetaData>();
        private string databasePath;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            ReadApplicationConfig();
            TryLoadDatabase();
        }

        #endregion

        private void ReadApplicationConfig()
        {
            var appConfig = Utils.ReadJsonFromResources<ApplicationConfig>(Constants.APP_CONFIG_PATH);
            AddReadonly(Constants.APP_CONFIG_DB_KEY, appConfig, false);
            LogWrapper.Log("[{0}] loaded app config succesfully and added to the database with key: {1}", GetType(), Constants.APP_CONFIG_DB_KEY);
        }

        private void TryLoadDatabase()
        {
            databasePath = Path.Combine(Application.persistentDataPath, Get<ApplicationConfig>(Constants.APP_CONFIG_DB_KEY).databaseId);
            if (File.Exists(databasePath))
            {
                var readData = Utils.ReadBinary<List<MetaData>>(databasePath);
                metaData.AddRange(readData);
            }
        }

        public void AddReadonly(string key, object value, bool persist, bool flush = false)
        {
            var mdata = new MetaData();
            mdata.key = key;
            mdata.value = value;
            mdata.persist = persist;
            mdata.isReadonly = true;
            metaData.Add(mdata);
            if (flush)
                Flush();
        }

        public void Set(string key, object value, bool persist, bool flush = false)
        {
            var mdata = metaData.Find(md => string.Equals(md.key, key));

            if (mdata == null)
            {
                mdata = new MetaData();
                mdata.key = key;
                mdata.value = value;
                mdata.persist = persist;
                metaData.Add(mdata);
            }
            else if (mdata.isReadonly)
            {
                LogWrapper.Warning("Cannot override read-only data: {0}", key);
            }
            else
            {
                mdata.value = value;
                mdata.persist = persist;
            }

            if (flush)
                Flush();
        }

        public void Clear(string key, bool flush = false)
        {
            metaData.RemoveAll(md => string.Equals(key, md.key));

            if (flush)
                Flush();
        }

        public void ClearAll(bool flush = false)
        {
            metaData.Clear();

            if (flush)
                Flush();
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            var mdata = metaData.Find(md => string.Equals(md.key, key));
            return mdata != null ? (T)mdata.value : defaultValue; 
        }

        public bool HasData(string key)
        {
            return metaData.Exists(md => string.Equals(md.key, key));
        }

        public bool IsReadonly(string key)
        {
            bool hasData = HasData(key);
            return hasData ? metaData.Find(md => string.Equals(md.key, key)).isReadonly : false;
        }

        public void Flush()
        { 
            Utils.WriteBinary(metaData.FindAll(md => md.persist), databasePath);
        }

        public void Backup()
        {
            var backupDatabasePath = databasePath += "_" + DataVersionService.APP_VERSION;
            Utils.WriteBinary(metaData, backupDatabasePath);
        }

        public void RestoreBackup(string version)
        {
            var backupDatabasePath = databasePath += "_" + version;
            if (File.Exists(backupDatabasePath))
            {
                metaData = Utils.ReadBinary<List<MetaData>>(backupDatabasePath);
            }
            else
            {
                LogWrapper.Error("[{0}] Could not restore backup for version {1} and databse path {2}", GetType(), version, backupDatabasePath);
            }
        }
    }

    [System.Serializable]
    public class MetaData
    {
        public string key;
        public object value;
        public bool isReadonly;
        public bool persist;
    }

    public class ApplicationConfig
    {
        public string version;
        public float doubleTapElapseTime;
        public float dragMinDistance;
        public float holdMinElapseTime;
        public string databaseId;
        public string assetManifestPath;
    }
}