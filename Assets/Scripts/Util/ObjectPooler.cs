using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPooler
{
    [SerializeField] GameObject _parent;
    [SerializeField] GameObject _objectToPool;
    [SerializeField] int _initialPoolAmount = 5;

    [SerializeField] List<GameObject> _pool = new List<GameObject>();
    
    public ObjectPooler(GameObject prefab, GameObject parent, int poolAmount = 5)
    {
        _parent = parent;
        _initialPoolAmount = poolAmount;
        _pool = new List<GameObject>();
        _objectToPool = prefab;
        for (int i = 0; i < _initialPoolAmount; i++)
        {
            AddObject();
        }
    }

    private void AddObject()
    {
        GameObject go = GameObject.Instantiate(_objectToPool, _parent.transform);
        go.SetActive(false);
        _pool.Add(go);
    }


    public GameObject GetObject()
    {
        foreach(GameObject go in _pool)
        {
            if(!go.activeInHierarchy)
            {
                go.SetActive(true);
                return go;
            }
        }

        AddObject();
        return GetObject();
    }

}
