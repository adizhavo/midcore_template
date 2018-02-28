using System;
using UnityEngine;
using Services.Game.Grid;
using System.Collections;
using MidcoreTemplate.Game.Systems;
using Services.Core;

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
        
        public static void DelayedCall(float time, Action callback, bool ignoreTimeScale = false)
        {
            if (ignoreTimeScale)
            {
                SceneAttachment.AttachCoroutine(WaitForRealSeconds(time, callback));
            }
            else
            {
                SceneAttachment.AttachCoroutine(WaitForUnitySeconds(time, callback));
            }
        }
        
        public static IEnumerator WaitForUnitySeconds(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            if (callback != null)
                callback();
        }
    
        public static IEnumerator WaitForRealSeconds(float time, Action callback)
        {
            float startTime = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - startTime < time)
                yield return 1;
            if (callback != null)
                callback();
        }
        
        public static void SetMaterialTiling(Transform transform, Vector2 tiling, bool includeChild = false)
        {
            if (transform != null)
            {
                var renderer = transform.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.material.SetTextureScale("_MainTex", tiling);
                
                if (includeChild)
                {
                    var renderers = transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshRenderer in renderers)
                        meshRenderer.material.SetTextureScale("_MainTex", tiling);
                }
            }
        }
        
        public static void SetMaterialOffset(Transform transform, Vector2 offset, bool includeChild = false)
        {
            if (transform != null)
            {
                var renderer = transform.GetComponent<MeshRenderer>();
                if (renderer != null)
                    renderer.material.SetTextureOffset("_MainTex", offset);
                
                if (includeChild)
                {
                    var renderers = transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshRenderer in renderers)
                        meshRenderer.material.SetTextureOffset("_MainTex", offset);
                }
            }
        }

        public static void AddOffsetMaterial(Transform transform, Vector2 amount, bool includeChild = false)
        {
            if (transform != null)
            {
                var renderer = transform.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    var offset = renderer.material.GetTextureOffset("_MainTex") + amount;
                    if (offset.x > 1f || offset.x < -1f) offset.x = 0f;
                    if (offset.y > 1f || offset.y < -1f) offset.y = 0f;
                    renderer.material.SetTextureOffset("_MainTex", offset);
                }
                
                if (includeChild)
                {
                    var renderers = transform.GetComponentsInChildren<MeshRenderer>();
                    foreach (var meshRenderer in renderers)
                    {
                        var offset = meshRenderer.material.GetTextureOffset("_MainTex") + amount;
                        if (offset.x > 1f || offset.x < -1f) offset.x = 0f;
                        if (offset.y > 1f || offset.y < -1f) offset.y = 0f;
                        meshRenderer.material.SetTextureOffset("_MainTex", offset);
                    }
                }
            }
        }
        
        public static string GetFormattedTime(float time)
        {
            string timeString;
            var timeSpan = new TimeSpan(0, 0, (int)time);
            
            if (timeSpan.Days > 0)
            {
                timeString = string.Format("{0}d {1}h {2:D2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            }
            else if (timeSpan.Hours > 0)
            {
                timeString = timeSpan.Minutes > 0 ? string.Format("{0}h {1:D2}m", timeSpan.Hours, timeSpan.Minutes) : string.Format("{0}h", timeSpan.Hours);
            }
            else
            {
                timeString = string.Format("{0}m {1:D2}s", timeSpan.Minutes, timeSpan.Seconds); 
            }

            return timeString;
        }
    }
}