﻿using Zenject;
using Entitas;
using UnityEngine;
using Services.Core.Data;
using Services.Core.GUI;
using Services.Game.SceneCamera;
using Services.Game.Factory;
using System;
using System.Linq;
using System.Collections.Generic;
using Services.Core;

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
        [Inject] CameraService cameraService;

        private GameObject HUDParent;

        private Dictionary<GameEntity, HUD_Object> activeHUDs = new Dictionary<GameEntity, HUD_Object>();

        #region IInitializeSystem implementation

        public void Initialize()
        {
            HUDParent = new GameObject("_HUDParent");
            HUDParent.AddComponent<RectTransform>();
            HUDParent.transform.SetParent(guiService.Canvas.transform, false);
            HUDParent.transform.SetSiblingIndex(0);
        }

        #endregion

        #region IExecuteSystem implementation

        public void Execute()
        {
            if (cameraService.activeCamera != null)
            {
                var removeList = new List<GameEntity>();
                var entities = new List<GameEntity>(activeHUDs.Keys);

                foreach (var entity in entities)
                {
                    if (!entity.hasView || !entity.viewObject.activeSelf)
                    {
                        removeList.Add(entity);
                    }
                    else
                    {
                        var onScreen = Utils.IsVisible(entity.HUDPivot, cameraService.activeCamera);

                        if (activeHUDs[entity].Container.activeSelf != onScreen)
                            activeHUDs[entity].Container.SetActive(onScreen);

                        if (onScreen)
                        {
                            RepositionHUD(entity, activeHUDs[entity]);
                            activeHUDs[entity].UpdateHUD();
                        }
                    }
                }

                foreach (var remove in removeList)
                {
                    RemoveHUD(remove);
                }
            }
        }

        private void RepositionHUD(GameEntity entity, HUD_Object hudObject)
        {
            if (cameraService.activeCamera != null)
            {
                var hudPosition = cameraService.activeCamera.WorldToScreenPoint(entity.HUDPivot);
                hudObject.transform.position = hudPosition;
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

                if (hud != null)
                {
                    hud.Setup(hudId, entity);
                }
                else
                {
                    throw new NullReferenceException("Prefab " + prefabPath + " and id " + hudId + " doesn't have a HUD_Object component");
                }

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
            return activeHUDs.ContainsKey(entity) ? activeHUDs[entity] : null;
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