using Entitas;
using UnityEngine;

namespace Services.Game.Misc
{
    public class AutoDestroySystem : IExecuteSystem
    {
        #region IExecuteSystem implementation

        public void Execute()
        {
            var entites = Contexts.sharedInstance.game.GetEntities(GameMatcher.AutoDestroy);
            foreach(var entity in entites)
            {
                entity.autoDestroy.remainingTime -= entity.autoDestroy.ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
                if (entity.autoDestroy.remainingTime < 0f)
                {
                    entity.Destroy();
                }
            }
        }

        #endregion
        
    }
}