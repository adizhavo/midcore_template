using Entitas;
using UnityEngine;
using Framework.Utils;
using System;
using System.IO;
using System.Collections.Generic;

namespace Framework.Data
{
    public class DataBase : IInitializeSystem
    {
        private Dictionary<string, object> data;
        private string databasePath;

        public DataBase()
        {
            data = new Dictionary<string, object>();
        }

        #region IInitializeSystem implementation

        public void Initialize()
        {
            // TODO : get this value from the application config
            databasePath = Path.Combine(Application.persistentDataPath, "data_base");

            if (File.Exists(databasePath))
            {
                data = Util.ReadBinary<Dictionary<string, object>>(databasePath);
            }
            else
            {
                Flush();
            }
        }

        #endregion

        public void Set<T>(string key, object value, bool flush = true)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }

            if (flush) Flush();
        }

        public T Get<T>(string key, T defaultValue = default(T))
        {
            return data.ContainsKey(key) ? (T)data[key] : defaultValue;
        }

        public void Flush()
        {
            Util.WriteBinary(data, databasePath);
        }
    }
}