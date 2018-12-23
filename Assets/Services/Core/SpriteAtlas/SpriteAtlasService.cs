using Entitas;
using Services.Core.Data;
using UnityEngine;
using UnityEngine.U2D;
using Zenject;

namespace Services.Core.Atlas
{
    public class SpriteAtlasService : IInitializeSystem
    {
        [Inject] DatabaseService database;

        private SpriteAtlas[] spriteAtlases;

        public void Initialize()
        {
            var appConfig = database.Get<ApplicationConfig>(Constants.DB_KEY_APP_CONFIG);
            spriteAtlases = Resources.LoadAll<SpriteAtlas>(appConfig.spriteAtlasPath);
        }

        public Sprite GetSprite(string id)
        {
            foreach (var spriteAtlas in spriteAtlases)
            {
                var sprite = spriteAtlas.GetSprite(id);
                if (sprite != null)
                    return sprite;
            }
            LogWrapper.Error("Could not find image with id {0}", id);
            return null;
        }
    }
}
