using Zenject;
using Entitas;
using UnityEngine;
using Services.Core;
using Services.Core.Data;
using MergeWar.Data;
using System.Collections.Generic;

namespace MergeWar.Game.Systems
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
            var gameConfig = LoadFile<GameConfig>(Constants.GAME_CONFIG_DATA_ID);
            database.AddReadonly(gameConfig.id, gameConfig, false);

            foreach(var vfx in LoadFile<VFXDataRoot>(Constants.VFX_DATA_ID).root)
                database.AddReadonly(vfx.id, vfx, false);

            foreach(var objectData in LoadFile<ObjectDataRoot>(Constants.TILE_DATA_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);

            foreach(var objectData in LoadFile<GameGridObjecDataRoot>(Constants.OBJECT_DATA_DATA_ID).root)
                database.AddReadonly(objectData.objectId, objectData, false);

            foreach(var commandData in LoadFile<CommandDataRoot>(Constants.COMMAND_DATA_ID).root)
                database.AddReadonly(commandData.id, commandData, false);

            var mergeComboDataRoot = LoadFile<MergeComboDataRoot>(Constants.MERGE_COMBO_DATA_ID);
            database.AddReadonly(Constants.MERGE_COMBO_DATA_ROOT_KEY, mergeComboDataRoot, false);
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

        public MergeComboData GetMergeComboDataForInput(string input)
        {
            return database.Get<MergeComboDataRoot>(Constants.MERGE_COMBO_DATA_ROOT_KEY).root.Find(c => !string.IsNullOrEmpty(input) && c.input.Equals(input));
        }

        public List<MergeComboData> GetMergeComboDataForOutput(string output)
        {
            return database.Get<MergeComboDataRoot>(Constants.MERGE_COMBO_DATA_ROOT_KEY).root.FindAll(c => !string.IsNullOrEmpty(output) && c.output.Equals(output));
        }

        public bool CanMerge(string input)
        {
            var mergeComboData = GetMergeComboDataForInput(input);
            return mergeComboData != null && !string.IsNullOrEmpty(mergeComboData.output);
        }

        public string GetNextObjectId(string typeId, int level)
        {
            var objectData = database.Get<GameGridObjectData>((GameGridObjectData ggod) => 
                !string.IsNullOrEmpty(typeId) 
                && typeId.Equals(ggod.typeId)
                && ggod.level == level + 1);
            return objectData != null ? objectData.objectId : string.Empty;
        }
    }
}