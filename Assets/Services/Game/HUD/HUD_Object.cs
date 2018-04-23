using UnityEngine;
using Services.Core;
using Services.Game.SceneCamera;

namespace Services.Game.HUD
{
    public class HUD_Object : MonoBehaviour
    {
        [Header("Will be enabled/disabled if the entitty is visible")]
        public GameObject Container;
        
        public string id { private set; get; }

        public GameEntity entity { private set; get; }
        
        private CameraService cameraService;

        protected virtual void Awake()
        {
            cameraService = GameServiceInstaller.Resolve<CameraService>();
        }
        
        public virtual void Setup(string id, GameEntity entity)
        {
            this.id = id;
            this.entity = entity;
            Container.SetActive(false);
        }

        public virtual void Disable()
        {
            id = string.Empty;
            entity = null;
            gameObject.SetActive(false);
        }
        
        protected  virtual void Update()
        {
            if (entity != null && entity.hasView)
            {
                var isVisible = Utils.IsVisible(entity.position, cameraService.activeCamera);
                Container.SetActive(isVisible);
            }
        }
    }
}