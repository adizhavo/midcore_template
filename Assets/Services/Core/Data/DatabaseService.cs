using Entitas;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using Services.Core.DataVersion;

namespace Services.Core.Data
{
    /// <summary>
    /// Will store all runtime data from the game and start up data from the DataBooter
    /// </summary>

    public class DatabaseService : IInitializeSystem
    {
        private Hashtable database = new Hashtable();
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
            var appConfig = Utils.ReadJsonFromResources<ApplicationConfig>(Constants.PATH_APP_CONFIG);
            AddReadonly(Constants.DB_KEY_APP_CONFIG, appConfig, false);
            LogWrapper.Log("[{0}] loaded app config succesfully and added to the database with key: {1}", GetType(), Constants.DB_KEY_APP_CONFIG);
        }

        private void TryLoadDatabase()
        {
            databasePath = Path.Combine(Application.persistentDataPath, Get<ApplicationConfig>(Constants.DB_KEY_APP_CONFIG).databaseId);
            if (File.Exists(databasePath))
            {
                var readData = Utils.ReadBinary<Hashtable>(databasePath);
                foreach(DictionaryEntry data in readData)
                    if (!HasKey(data.Key as string))
                        database.Add(data.Key as string, data.Value as DBData);
            }
        }

        public void AddReadonly(string key, object value, bool persist, bool flush = false)
        {
            if (!HasKey(key))
            {
                var data = new DBData();
                data.value = value;
                data.persist = persist;
                data.isReadonly = true;
                database.Add(key, data);
                if (flush) Flush();
            }
            else
            {
                LogWrapper.DebugWarning("[{0}] Key already in the database {1}", GetType(), key);
            }
        }

        public void Set(string key, object value, bool persist, bool flush = false)
        {
            if (!HasKey(key))
            {
                var data = new DBData();
                data.value = value;
                data.persist = persist;
                database.Add(key, data);
            }
            else
            {
                var data = database[key] as DBData;

                if (data.isReadonly)
                {
                    LogWrapper.Warning("[{0}] Cannot override read-only data: {1}", GetType(), key);
                }
                else
                {
                    data.value = value;
                    data.persist = persist;
                }
            }

            if (flush) Flush();
        }

        public void Clear(string key, bool flush = false)
        {
            database.Remove(key);
            if (flush) Flush();
        }

        public void ClearAll(bool flush = false)
        {
            database.Clear();
            if (flush) Flush();
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            return HasKey(key) ? (T)(database[key] as DBData).value : defaultValue;
        }

        public T Get<T>(Predicate<T> predicate, T defaultValue = default(T))
        {
            foreach(DictionaryEntry data in database)
            {
                var value = (data.Value as DBData).value;
                if (value is T)
                {
                    var casted = (T)value;
                    if (predicate(casted))
                    {
                        return casted;
                    }
                }
            }

            return defaultValue;
        }

        public bool HasKey(string key)
        {
            return database.Contains(key);
        }

        public bool IsReadonly(string key)
        {
            return HasKey(key) ? (database[key] as DBData).isReadonly : false;
        }

        public void Flush()
        { 
            var save = new Hashtable();
            foreach(DictionaryEntry databaseEntry in database)
            {
                var data = (databaseEntry.Value as DBData);
                if (data.persist)
                    save.Add(databaseEntry.Key as string, data);
            }
            
            Utils.WriteBinary(save, databasePath);
        }

        public void Backup()
        {
            var backupDatabasePath = databasePath += "_" + DataVersionService.APP_VERSION;
            Utils.WriteBinary(database, backupDatabasePath);
        }

        public void RestoreBackup(string version)
        {
            var backupDatabasePath = databasePath += "_" + version;
            if (File.Exists(backupDatabasePath))
            {
                database = Utils.ReadBinary<Hashtable>(backupDatabasePath);
            }
            else
            {
                LogWrapper.Error("[{0}] Could not restore backup for version {1} and databse path {2}", GetType(), version, backupDatabasePath);
            }
        }
    }

    [System.Serializable]
    public class DBData
    {
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