using Zenject;
using UnityEngine;
using Services.Game.Grid;
using System.Collections.Generic;
using MidcoreTemplate.Game.Systems;

namespace MidcoreTemplate.Game.Utilities
{
    /// <summary>
    /// All Unity/Scene related functionalities
    /// </summary>

    public static class Utils 
    {
        public static GameEntity GetInputTargetOnGrid(Vector3 screenPos, SceneSystem sceneSystem, Camera camera, GridService gridService)
        {
            var touched = GetInputTarget(screenPos, sceneSystem, camera);
            if (touched == null)
            {
                var worldPos = GetPlaneTouchPos(screenPos, camera);
                var cell = gridService.GetCell(worldPos);
                if (cell != null)
                {
                    touched = cell.cell.occupant;
                }
            }
            return touched;
        }

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
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
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

        public static void SetSpriteSortingOrder(GameEntity entity, int sortingOrder)
        {
            SetSpriteSortingOrder(entity.viewObject, sortingOrder);
        }

        public static void SetSpriteSortingOrder(GameObject gameObject, int sortingOrder)
        {
            var spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
            int index = 0;
            foreach(var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = sortingOrder + index;
                index ++;
            } 
        }

        public static void SetRendererSortingOrder(GameEntity entity, int sortingOrder)
        {
            SetRendererSortingOrder(entity.viewObject, sortingOrder);
        }

        public static void SetRendererSortingOrder(GameObject gameObject, int sortingOrder)
        {
            var spriteRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
            int index = 0;
            foreach(var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder = sortingOrder + index;
                index ++;
            } 
        }
    }
}