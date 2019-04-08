using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Reset();
}

public class Pool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable  
{
    LinkedList<T> pool = new LinkedList<T>();
    public GameObject poolObjectPrefab;
    public int size = 20;

    private void Awake()
    {
        if(!poolObjectPrefab.GetComponent<T>())
        {
            Debug.LogWarning("No matching component on pool object prefab !!");
            poolObjectPrefab.AddComponent<T>();
        }
    }

    private void Start()
    {
        for (int i = 0; i < size; i++)
        {
            T newPoolItem = Instantiate(poolObjectPrefab).GetComponent<T>();
            newPoolItem.gameObject.SetActive(false);
            pool.AddLast(newPoolItem);
        }
    }

    // <summary>
    // Create new Item from pool
    // </summary>
    T Create()
    {
        T ret;
        if(pool.Count > 0)
        {
            ret = pool.Last.Value;
            pool.RemoveLast();
            return ret;
        }
        return Instantiate(poolObjectPrefab).GetComponent<T>();
    }

    // <summary>
    // Consume Item back into pool
    // </summary>
    void Consume(T poolItem)
    {
        poolItem.Reset();
        poolItem.gameObject.SetActive(false);
        pool.AddLast(poolItem);
    }
}
