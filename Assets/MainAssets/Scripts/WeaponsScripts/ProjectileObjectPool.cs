using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count > 0 && !poolDictionary[tag].Peek().activeInHierarchy)
        {
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.SetActive(true);
            poolDictionary[tag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }
        else
        {
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool == null)
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            GameObject newObj = Instantiate(pool.prefab);
            newObj.SetActive(false);
            poolDictionary[tag].Enqueue(newObj);
            return newObj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
