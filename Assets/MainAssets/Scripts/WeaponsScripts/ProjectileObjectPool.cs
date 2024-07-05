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
    private Dictionary<GameObject, GameObject> instanceToPrefabMap;

    void Awake()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        instanceToPrefabMap = new Dictionary<GameObject, GameObject>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                instanceToPrefabMap[obj] = pool.prefab;
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

        Queue<GameObject> objectPool = poolDictionary[prefab];

        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Se nessun oggetto ? disponibile, creiamo un nuovo oggetto
        GameObject newObj = Instantiate(prefab);
        newObj.SetActive(true);
        objectPool.Enqueue(newObj);
        instanceToPrefabMap[newObj] = prefab;
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        if (instanceToPrefabMap.ContainsKey(obj))
        {
            GameObject prefab = instanceToPrefabMap[obj];
            if (!poolDictionary[prefab].Contains(obj))
            {
                poolDictionary[prefab].Enqueue(obj);
            }
        }
        else
        {
            Debug.LogError("Returned object does not belong to any pool: " + obj.name);
        }
    }
}
