using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对象池
///当你尝试访问Instance单例时将会自动创建唯一一个名为Pool的gameobject作为对象池的父物体
/// 记得在过场景的时候手动clear一下
/// </summary>
public class PoolManager
{
    private static PoolManager _instance;

    public static PoolManager Instance
    {
        get
        {
            _instance ??= new PoolManager();
            return _instance;
        }
    }

    private readonly Dictionary<string, Stack<GameObject>> _poolDict = new();
    private readonly Dictionary<string, Transform> _parentsSet = new();
    private GameObject _pool;

    public GameObject Pool
    {
        get
        {
            if (!_pool)
            {
                _pool = new GameObject("Pool");
            }

            return _pool;
        }
    }

    public void Push(GameObject obj, string pushId)
    {
        if (_parentsSet.TryGetValue(pushId, out Transform parent))
        {
            obj.transform.SetParent(parent, false);
        }
        else
        {
            GameObject parentObj = new(pushId);
            parentObj.transform.SetParent(Pool.transform, false);
            _parentsSet.Add(pushId, parentObj.transform);
            obj.transform.SetParent(parentObj.transform, false);
        }

        obj.SetActive(false);
        if (_poolDict.TryGetValue(pushId, out Stack<GameObject> stack))
        {
            stack.Push(obj);
        }
        else
        {
            stack = new Stack<GameObject>();
            stack.Push(obj);
            _poolDict.Add(pushId, stack);
        }
    }

    public void Push(GameObject obj)
    {
        Push(obj, obj.name);
    }

    public GameObject GetFromAb(
        string ab,
        string name,
        Transform parent = null,
        UnityAction<GameObject> initAction = null
    )
    {
        GameObject obj;
        if (_poolDict.TryGetValue(name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = _poolDict[name].Pop();
            obj.transform.SetParent(parent, false);
            initAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }

        obj = MyABMgr.Instance.LoadRes<GameObject>(ab, name);
        obj.name = name;
        return obj;
    }

    public GameObject GetFromPrefab(
        GameObject gobjRes,
        Transform parent = null,
        UnityAction<GameObject> initAction = null
    )
    {
        GameObject obj;
        if (_poolDict.TryGetValue(gobjRes.name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = _poolDict[gobjRes.name].Pop();
            obj.transform.SetParent(parent, false);
            initAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }

        obj = Object.Instantiate(gobjRes, parent);
        obj.name = gobjRes.name;
        return obj;
    }

    public GameObject Get(string Id)
    {
        GameObject obj;
        if (_poolDict.TryGetValue(Id, out Stack<GameObject> stack) && stack.Count > 0)
        {
            obj = _poolDict[Id].Pop();
            while (obj == null && _poolDict.Count > 0)
            {
                obj = _poolDict[Id].Pop();
            }

            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        obj = Object.Instantiate(Resources.Load<GameObject>(Id));
        obj.name = Id;

        return obj;
    }

    public GameObject Get(
        string id,
        string path = "",
        UnityAction<GameObject> initAction = null
    )
    {
        GameObject obj;
        if (_poolDict.TryGetValue(id, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = _poolDict[id].Pop();
            initAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }

        obj = Object.Instantiate(Resources.Load<GameObject>(path + id));
        obj.name = id;
        return obj;
    }

    public void Clear()
    {
        foreach (var stack in _poolDict)
        {
            foreach (var gobj in stack.Value)
            {
                Object.Destroy(gobj);
            }
        }

        _poolDict.Clear();
        _instance = null;
    }
}