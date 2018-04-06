using UnityEngine;

namespace Services.Game.HUD
{
    public class HUD_Object : MonoBehaviour
    {
        public string id { private set; get; }

        public GameEntity entity { private set; get; }
        
        public bool hasInitialPosition { set; get; }

        public virtual void Setup(string id, GameEntity entity)
        {
            this.id = id;
            this.entity = entity;
        }

        public virtual void Enable()
        {
            gameObject.SetActive(true);
        }

        public virtual void Disable()
        {
            id = string.Empty;
            entity = null;
            gameObject.SetActive(false);
        }
    }
}