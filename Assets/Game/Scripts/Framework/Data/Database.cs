using Entitas;
using UnityEngine;
using Framework.Log;
using Framework.Utils;
using System;
using System.IO;
using System.Collections.Generic;

namespace Framework.Data
{
    /// <summary>
    /// Will store all runtime data from the game and start up data from the DataBooter
    /// </summary>

    public class Database : IInitializeSystem
    {
        private List<MetaData> metaData;
        private string databasePath;

        public Database()
        {
            metaData = new List<MetaData>();
        }

        #region IInitializeSystem implementation

        public void Initialize()
        {
            databasePath = Path.Combine(Application.persistentDataPath, Get<ApplicationConfig>(Constants.APP_CONFIG_ID).databasePersistPath);

            if (File.Exists(databasePath))
            {
                metaData = Util.ReadBinary<List<MetaData>>(databasePath);
            }
        }

        #endregion

        public void AddReadonly(string key, object value, bool persist, bool flush = false)
        {
            var mdata = new MetaData();
            mdata.key = key;
            mdata.value = value;
            mdata.persist = persist;
            mdata.isReadonly = true;
            metaData.Add(mdata);
            if (flush) Flush();
        }

        public void Set(string key, object value, bool flush = false)
        {
            var mdata = metaData.Find(md => string.Equals(md.key, key));

            if (mdata == null)
            {
                mdata = new MetaData();
                mdata.key = key;
                mdata.value = value;
                metaData.Add(mdata);
            }
            else if (mdata.isReadonly)
            {
                LogWrapper.Warning(string.Format("Cannot override read-only data: {0}", key));
            }
            else
            {
                mdata.value = value;
            }

            if (flush) Flush();
        }

        public void Clear(string key, bool flush = false)
        {
            metaData.RemoveAll(md => string.Equals(key, md.key) && !md.isReadonly);

            if (flush) Flush();
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
            Util.WriteBinary(metaData.FindAll(md => md.persist), databasePath);
        }
    }
}