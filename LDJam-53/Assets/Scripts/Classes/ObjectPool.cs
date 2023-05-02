namespace DefaultNamespace
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ObjectPool<T> where T : Component
    {
        private T pooledObjectPrefab;
        private int poolSize;
        private bool expandable;
        private Transform poolParent;
        private List<T> pooledObjects;

        public ObjectPool(T prefab, int size, bool canExpand, Transform parent = null)
        {
            pooledObjectPrefab = prefab;
            poolSize = size;
            expandable = canExpand;
            poolParent = parent;
            InitializePool();
        }

        private void InitializePool()
        {
            pooledObjects = new List<T>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                T obj = Object.Instantiate(pooledObjectPrefab, poolParent);
                obj.gameObject.SetActive(false);
                pooledObjects.Add(obj);
            }
        }

        public T GetPooledObject()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].gameObject.activeInHierarchy)
                {
                    return pooledObjects[i];
                }
            }

            if (expandable)
            {
                T obj = Object.Instantiate(pooledObjectPrefab, poolParent);
                obj.gameObject.SetActive(false);
                pooledObjects.Add(obj);
                return obj;
            }

            return null;
        }

        public void ReturnPooledObject(T obj)
        {
            if (pooledObjects.Contains(obj))
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

}