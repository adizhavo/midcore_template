using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;

namespace Template.Sample
{
    public class SampleDataProvider : IInitializeSystem
    {
        [Inject] DatabaseService database;

        #region IInitializeSystem implementation

        public void Initialize()
        {
            var objectDataRoot = Utils.ReadJsonFromResources<ObjectDataRoot>(database.Get<string>("game_objects"));
            var tileDataRoot = Utils.ReadJsonFromResources<ObjectDataRoot>(database.Get<string>("tile_objects"));

            foreach(var objectData in objectDataRoot.root)
                database.AddReadonly(objectData.objectId, objectData, false);

            foreach(var tileData in tileDataRoot.root)
                database.AddReadonly(tileData.objectId, tileData, false);
        }

        #endregion
    }
}