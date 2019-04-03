using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    void Reset();
}

public class Pool<T> : MonoBehaviour where T : MonoBehaviour, IPoolable  
{
    List<T> pool = new List<T>();
    public GameObject poolObjectPrefab;
    public int size = 20;
    private GameObject tempObject;

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
            pool.Add(newPoolItem);
        }
    }

    // <summary>
    // Create new Item from pool
    // </summary>
    T Create()
    {
        for(int i = 0; i < size; i++)
        {
            if(!pool[i].gameObject.activeSelf)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }
        T newPoolItem = Instantiate(poolObjectPrefab).GetComponent<T>();
        pool.Add(newPoolItem);
        return newPoolItem;
    }

    // <summary>
    // Consume Item back into pool
    // </summary>
    void Consume(T poolItem)
    {
        poolItem.Reset();
        poolItem.gameObject.SetActive(false);
    }
}
