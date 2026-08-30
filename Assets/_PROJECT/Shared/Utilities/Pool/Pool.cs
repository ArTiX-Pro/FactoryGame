using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArTiX.Utils.Pool
{
    public class Pool<T> where T : Component
    {
        private readonly Stack<T> pool = new Stack<T>();
        private readonly int maxSize;

        private readonly Func<T> populateMethod;

        public Pool(Func<T> populateMethod, int initialSize, int maxSize)
        {
            this.populateMethod = populateMethod;

            T instance;
            for (int i = 0; i < initialSize; i++)
            {
                instance = populateMethod();
                instance.GetComponent<IPooledObject<T>>().Pool = this;
                pool.Push(instance);
            }

            this.maxSize = maxSize;
        }

        public T GetPooledObject()
        {
            if (pool.TryPop(out T objectTaken)) objectTaken.gameObject.SetActive(true);
            else if (pool.Count < maxSize)
            {
                objectTaken = populateMethod();
                objectTaken.GetComponent<IPooledObject<T>>().Pool = this;
            }

            return objectTaken;
        }

        public void ReturnToPool(in T obj)
        {
            if (pool.Contains(obj) || obj == null) return;

            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }
}