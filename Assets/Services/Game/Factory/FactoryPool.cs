using UnityEngine;
using System.Collections.Generic;

namespace Services.Game.Factory
{
    public class FactoryPool 
    {
        private static List<FactoryPool> pools = new List<FactoryPool>();

        public static FactoryPool GetPool(string prefabPath)
        {
            foreach (var p in pools)
                if (string.Equals(p.prefabPath, prefabPath))
                    return p;

            var pool = new FactoryPool(prefabPath);
            pools.Add(pool);
            return pool;
        }
        
        public static FactoryPool GetPool(GameObject prefab)
        {
            foreach (var p in pools)
                if (string.Equals(p.prefab, prefab))
                    return p;

            var pool = new FactoryPool(prefab);
            pools.Add(pool);
            return pool;
        }

        public static GameObject GetPooled(string prefabPath)
        {
            return GetPool(prefabPath).GetObject();
        }

        public static GameObject GetPooled(GameObject prefab)
        {
            return GetPool(prefab).GetObject();
        }

        public static GameObject GetUnPooled(string prefabPath)
        {
            return GetPool(prefabPath).GetObject(false);
        }

        public string prefabPath;
        public GameObject prefab;
        private GameObject poolObject;
        private List<GameObject> objects;

        public FactoryPool(string prefabPath)
        {
            this.prefabPath = prefabPath;
            prefab = Resources.Load<GameObject>(prefabPath);
            objects = new List<GameObject>();
            poolObject = new GameObject("_pool_" + prefab.name);
        }
        
        public FactoryPool(GameObject prefab)
        {
            this.prefab = prefab;
            objects = new List<GameObject>();
            poolObject = new GameObject("_pool_" + prefab.name);
        }

        public GameObject GetObject(bool pooled = true)
        {
            #if UNITY_EDITOR
            ResetPoolHierarchy();
            #endif

            if (pooled)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i] == null)
                    {
                        objects[i] = GameObject.Instantiate<GameObject>(prefab);
                        return objects[i];
                    }
                    else if (!objects[i].activeSelf)
                    {
                        objects[i].SetActive(true);
                        return objects[i];
                    }
                }
            }

            var go = GameObject.Instantiate<GameObject>(prefab);
            objects.Add(go);
            go.transform.SetParent(poolObject.transform);
            return go;
        }

        private void ResetPoolHierarchy()
        {
            foreach(var ob in objects)
            {
                if (!ob.activeSelf)
                {
                    ob.transform.SetParent(poolObject.transform, false);
                }
            }
        }
    }
}