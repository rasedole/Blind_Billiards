using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPoolingManager
{
    public static Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>>();

    public static GameObject Pooling(GameObject prefab)
    {
        if (!pool.ContainsKey(prefab))
        {
            pool.Add(prefab, new List<GameObject>());
        }

        GameObject instance;
        if (pool[prefab].Count < 1)
        {
            instance = GameObject.Instantiate(prefab);
            pool[prefab].Add(instance);
            return instance;
        }
        foreach (GameObject obj in pool[prefab])
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        instance = GameObject.Instantiate(prefab);
        pool[prefab].Add(instance);
        return instance;
    }
}
