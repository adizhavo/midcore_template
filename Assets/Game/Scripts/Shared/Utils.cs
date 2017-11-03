using Zenject;
using UnityEngine;
using System.Collections.Generic;
using MergeWar.Game.Systems;

namespace MergeWar.Game.Utilities
{
    /// <summary>
    /// All Unity/Scene related functionalities
    /// </summary>

    public static class Utils 
    {
        public static GameEntity GetInputTarget(Vector3 screenPos, SceneSystem sceneSystem, Camera camera)
        {
            var gameObject = GetInputTarget(screenPos, camera);
            return gameObject != null ? sceneSystem.GetEntityWithView(gameObject) : null;
        }

        public static GameObject GetInputTarget(Vector3 screenPos, Camera camera)
        {
            var ray = camera.ScreenPointToRay(screenPos);
            var raycastHit = new RaycastHit();

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform != null)
                {
                    return raycastHit.transform.gameObject;
                }
            }

            return null;
        }

        public static Vector3 GetPlaneTouchPos(Vector3 screenPos, Camera camera)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                return ray.GetPoint(rayDistance);
            }

            return Vector3.zero;
        }

        public static void SetSortingLayer(GameEntity entity, string layerName)
        {
            if (entity.hasView)
                SetSortingLayer(entity.viewObject, layerName);
        }

        public static void SetSortingLayer(GameObject gameObject, string layerName)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach(var renderer in renderers)
            {
                renderer.sortingLayerName = layerName;
                renderer.sortingOrder = 0;
            }

            var spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach(var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingLayerName = layerName;
                spriteRenderer.sortingOrder = 0;
            }
        }
    }
}