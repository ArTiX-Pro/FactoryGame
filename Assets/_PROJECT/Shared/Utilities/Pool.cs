using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : Component
{
    private readonly Stack<T> pool = new Stack<T>();

    public Pool(Func<T> populateMethod, int size)
    {
        for (int i = 0; i < size; i++)
        {
            pool.Push(populateMethod());
        }
    }

    public T GetPooledObject()
    {
        if (pool.TryPop(out T objectTaken))
        {
            objectTaken.gameObject.SetActive(true);
            return objectTaken;
        }
        return null;
    }

    public void ReturnToPool(in T obj)
    {
        if (pool.Contains(obj) || obj == null) return;

        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }
}
