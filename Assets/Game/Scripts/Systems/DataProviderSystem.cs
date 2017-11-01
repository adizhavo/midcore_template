using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;

namespace MergeWar
{
    /// <summary>
    /// Add here all game specific data
    /// </summary>

    public class DataProviderSystem : IInitializeSystem
    {
        [Inject] DatabaseService database;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            foreach(var objectData in LoadFile<GridObjectDataRoot>(Constants.OBJECT_DATA_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);

            foreach(var objectData in LoadFile<ObjectDataRoot>(Constants.TILE_DATA_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);

            var gameConfig = LoadFile<GameConfig>(Constants.GAME_CONFIG_DATA_ID);
            database.AddReadonly(gameConfig.id, gameConfig, false);
        }

        #endregion

        public T LoadFile<T>(string db_keyId)
        {
            return Utils.ReadJsonFromResources<T>(database.Get<string>(db_keyId));
        }

        public GameConfig GetGameConfig()
        {
            return database.Get<GameConfig>(Constants.DEFAULT_GAME_CONFIG_ID);
        }
    }
}