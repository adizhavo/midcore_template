using System;
using Zenject;
using Services.Core.Data;

namespace Services.Core.DataVersion
{
    /// <summary>
    /// Inherit to implement a data migrator block
    /// Implement the Execute method to apply changes to data, Execute will be called by the DataVersionService
    /// </summary>

    public abstract class DataMigratorBlock
    {
        [Inject] private DatabaseService database;

        protected Version blockVersion;

        public string version
        {
            get { return blockVersion.ToString(); }
        }

        public DataMigratorBlock SetVersion(string version)
        {
            blockVersion = new Version(version);
            return this;
        }

        public bool IsOfVersion(string version)
        {
            Version appVersion = new Version(version);
            return blockVersion.CompareTo(appVersion) == 0;
        }

        public bool IsOfCurrentVersion()
        {
            return IsOfVersion(DataVersionService.APP_VERSION);
        }

        public bool CanExecute(string fromVersion, string toVersion)
        {
            Version fVersion = new Version(fromVersion);
            Version tVersion = new Version(toVersion);
            return blockVersion.CompareTo(fVersion) > 0 && blockVersion.CompareTo(tVersion) <= 0;
        }

        public abstract void Execute();
    }
}
