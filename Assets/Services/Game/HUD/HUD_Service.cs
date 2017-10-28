using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using Services.Game.Factory;
using System.Linq;
using System.Collections.Generic;
using Services.Core.GUI;

namespace Services.Game.HUD
{
    /// <summary>
    /// Maps HUDs with a single entity in the scene
    /// Updates HUDs positions
    /// </summary>

    public class HUD_Service : IInitializeSystem, IExecuteSystem
    {
        [Inject] DatabaseService database;
        [Inject] GUIService guiService;

        private GameObject HUDParent;

        private Dictionary<GameEntity, HUD_Object> activeHUDs = new Dictionary<GameEntity, HUD_Object>();

        #region IInitializeSystem implementation

        public void Initialize()
        {
            HUDParent = new GameObject("_HUDParent");
            HUDParent.AddComponent<RectTransform>();
            HUDParent.transform.SetParent(guiService.Canvas.transform, false);
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            for (int i = activeHUDs.Count - 1; i >= 0; i --)
            {
                var activeHUD = activeHUDs.ElementAt(i);

                if (!activeHUD.Key.hasView || !activeHUD.Key.viewObject.activeSelf)
                {
                    RemoveHUD(activeHUD.Key);
                    break;
                }
                else
                {
                    var hudPosition = Camera.main.WorldToScreenPoint(activeHUD.Key.HUDPivot);
                    activeHUD.Value.transform.position = hudPosition;
                }
            }
        }

        #endregion

        public T AssignHUD<T>(string hudId, GameEntity entity) where T : HUD_Object
        {
            return (T)AssignHUD(hudId, entity);
        }

        public HUD_Object AssignHUD(string hudId, GameEntity entity)
        {
            var hud = GetHUD(entity);
            if (hud != null && hud.id.Equals(hudId))
            {
                return hud;
            }
            else
            {
                if (hud != null)
                {
                    RemoveHUD(entity);
                }

                var prefabPath = database.Get<string>(hudId);
                var hudClone = FactoryPool.GetPooled(prefabPath);
                hudClone.transform.SetParent(HUDParent.transform, false);
                hud = hudClone.GetComponent<HUD_Object>();
                activeHUDs.Add(entity, hud);
                return hud;
            }
        }

        public T GetHUD<T>(GameEntity entity) where T : HUD_Object
        {
            return (T)GetHUD(entity);
        }

        public HUD_Object GetHUD(GameEntity entity)
        {
            foreach(var activeHUD in activeHUDs)
            {
                if (activeHUD.Key.Equals(entity))
                    return activeHUD.Value;
            }
            return null;
        }

        public bool HasHUD(GameEntity entity)
        {
            return GetHUD(entity) != null;
        }

        public string GetHUDID(GameEntity entity)
        {
            var hud = GetHUD(entity);
            return hud != null ? hud.id : string.Empty;
        }

        public void RemoveHUD(GameEntity entity)
        {
            var hud = GetHUD(entity);
            if (hud != null)
            {
                hud.Disable();
                activeHUDs.Remove(entity);
            }
        }
    }
}