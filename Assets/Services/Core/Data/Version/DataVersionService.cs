using Zenject;
using Entitas;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Services.Core.Data;

namespace Services.Core.DataVersion
{
    /// <summary>
    /// Will track the app version and call migrator blocks when the version changes
    /// </summary>

    public class DataVersionService : IInitializeSystem
    {
        public static string APP_VERSION
        {
            private set;
            get;
        }

        [Inject] private DatabaseService database;

        private List<DataMigratorBlock> migratorBlocks;
        private List<VersionData> appVersions;
        private string versionsPath;

        public DataVersionService()
        {
            appVersions = new List<VersionData>();
            migratorBlocks = new List<DataMigratorBlock>();
        }

        #region IInitializeSystem implementation

        public void Initialize()
        {
            ReadAppVersion();
            ReadVersionLog();

            if (!HasVersion(APP_VERSION))
            {
                TryMigrateData();
                AddVersion(APP_VERSION);
                Utils.WriteBinary(appVersions, versionsPath);
            }

            LogWrapper.Log(this.ToString());
        }

        #endregion

        public DataVersionService AddMigratorBlock(DataMigratorBlock block)
        {
            migratorBlocks.Add(block);
            return this;
        }

        public bool HasVersion(string version)
        {
            Version currVersion = new Version(version);
            return appVersions.Find( v => new Version(v.version).CompareTo(currVersion) == 0 ) != null;
        }

        public override string ToString()
        {
            var sBuilder = new StringBuilder();
            sBuilder.Append(string.Format("[{0}] has versions:\n", GetType()));

            foreach (var v in appVersions)
            {
                sBuilder.Append(string.Format("{0} on date {1}\n", v.version, v.date.ToString()));
            }

            return sBuilder.ToString();
        }

        private void ReadAppVersion()
        {
            var appConfig = database.Get<ApplicationConfig>(Constants.APP_CONFIG_ID);
            APP_VERSION = appConfig.version;
        }

        private void ReadVersionLog()
        {
            versionsPath = Path.Combine(Application.persistentDataPath, Constants.VERSION_LOG_PATH);

            if (File.Exists(versionsPath))
            {
                appVersions.AddRange(Utils.ReadBinary<List<VersionData>>(versionsPath));
            }
        }

        private void AddVersion(string version)
        {
            var v = new VersionData();
            v.version = version;
            v.date = DateTime.UtcNow;
            appVersions.Add(v);
            LogWrapper.DebugLog("[{0}] added new version {0}", GetType(), version);
        }

        private void TryMigrateData()
        {
            if (appVersions.Count > 0)
            {
                var fromVersion = appVersions[appVersions.Count - 1].version;
                var toVersion = APP_VERSION;

                foreach(var block in migratorBlocks)
                {
                    if (block.CanExecute(fromVersion, toVersion))
                    {
                        block.Execute();
                        LogWrapper.Log("[{0}] executed migrator block for {0}", GetType(), block.version);
                    }
                }
            }
        }
      
        [System.Serializable]
        public class VersionData
        {
            public string version;
            public DateTime date;
        }
    }
}
