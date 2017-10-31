using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;

namespace MergeWar
{
    public class DataProviderSystem : IInitializeSystem
    {
        [Inject] DatabaseService database;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            foreach(var objectData in LoadFile<GridObjectDataRoot>(Constants.OBJECT_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);

            foreach(var objectData in LoadFile<ObjectDataRoot>(Constants.TILE_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);
        }

        #endregion

        public T LoadFile<T>(string db_keyId)
        {
            return Utils.ReadJsonFromResources<T>(database.Get<string>(db_keyId));
        }
    }
}