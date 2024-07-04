using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int initialSize;
    }

    public List<Pool> pools;
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool with prefab " + prefab.name + " doesn't exist.");
            return null;
        }

        if (poolDictionary[prefab].Count > 0 && !poolDictionary[prefab].Peek().activeInHierarchy)
        {
            GameObject objectToSpawn = poolDictionary[prefab].Dequeue();
            objectToSpawn.SetActive(true);
            poolDictionary[prefab].Enqueue(objectToSpawn);
            return objectToSpawn;
        }
        else
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            poolDictionary[prefab].Enqueue(newObj);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
