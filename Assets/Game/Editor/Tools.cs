using UnityEngine;
using UnityEditor;
using Services.Core;
using Services.Core.Data;
using System.IO;

namespace GramGames.MergeKingdom.Tools
{
    public class Tools
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            var appConfig = Utils.ReadJsonFromResources<ApplicationConfig>(Constants.PATH_APP_CONFIG);
            var databasePath = Path.Combine(Application.persistentDataPath, appConfig.databaseId);
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
            Debug.Log("All data cleared!");
        }
    }
}