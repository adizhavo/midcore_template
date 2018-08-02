using Services.Core;
using Services.Game.SceneCamera;
using UnityEngine;

namespace Services.Game.HUD
{
    public class HUD_Object : MonoBehaviour
    {
        [Header("Will be enabled/disabled if the entitty is visible")]
        public GameObject Container;
        
        public string id { private set; get; }

        public GameEntity entity { private set; get; }
        private Animator animator;
        
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public virtual void Setup(string id, GameEntity entity)
        {
            this.id = id;
            this.entity = entity;
            Container.SetActive(false);
            
            if (animator != null)
            {
                animator.ResetTrigger("setup");
                animator.SetTrigger("setup");
            }
        }

        public virtual void Disable()
        {
            id = string.Empty;
            entity = null;
            gameObject.SetActive(false);
        }

        public virtual void UpdateHUD()
        {
        }
    }
}