using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//对象池
//使用方法:拖进项目Assets内任意位置即可使用
//当你尝试访问Instance单例时将会自动创建唯一一个名为Pool的gameobject作为对象池的父物体
public class PoolManager
{
    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            instance ??= new();
            return instance;
        }
    }
    private readonly Dictionary<string, Stack<GameObject>> poolDict = new();
    private readonly Dictionary<string, Transform> parentsSet = new();
    private GameObject pool;

    public GameObject Pool
    {
        get
        {
            if (!pool)
            {
                pool = new GameObject("Pool");
            }
            return pool;
        }
    }

    public void Push(GameObject obj, string pushId)
    {
        if (parentsSet.TryGetValue(pushId, out Transform parent))
        {
            obj.transform.SetParent(parent, false);
        }
        else
        {
            GameObject parentObj = new(pushId);
            parentObj.transform.SetParent(Pool.transform, false);
            parentsSet.Add(pushId, parentObj.transform);
            obj.transform.SetParent(parentObj.transform, false);
        }
        obj.SetActive(false);
        if (poolDict.TryGetValue(pushId, out Stack<GameObject> stack))
        {
            stack.Push(obj);
        }
        else
        {
            stack = new();
            stack.Push(obj);
            poolDict.Add(pushId, stack);
        }
    }

    public void Push(GameObject obj)
    {
        if (parentsSet.TryGetValue(obj.name, out Transform parent))
        {
            obj.transform.SetParent(parent, false);
        }
        else
        {
            GameObject parentObj = new(obj.name);
            parentObj.transform.SetParent(Pool.transform, false);
            parentsSet.Add(obj.name, parentObj.transform);
            obj.transform.SetParent(parentObj.transform, false);
        }
        obj.SetActive(false);
        if (poolDict.TryGetValue(obj.name, out Stack<GameObject> stack))
        {
            stack.Push(obj);
        }
        else
        {
            stack = new();
            stack.Push(obj);
            poolDict.Add(obj.name, stack);
        }
    }

    public GameObject GetFromAB(
        string ab,
        string name,
        Transform parent = null,
        UnityAction<GameObject> initAction = null
    )
    {
        GameObject obj;
        if (poolDict.TryGetValue(name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = poolDict[name].Pop();
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
        if (poolDict.TryGetValue(gobjRes.name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = poolDict[gobjRes.name].Pop();
            obj.transform.SetParent(parent, false);
            initAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }
        obj = GameObject.Instantiate(gobjRes, parent);
        obj.name = gobjRes.name;
        return obj;
    }

    public GameObject Get(string name)
    {
        // Debug.Log(name);
        GameObject obj;
        if (poolDict.TryGetValue(name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = poolDict[name].Pop();
            while (obj == null && poolDict.Count > 0)
            {
                obj = poolDict[name].Pop();
            }
            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        obj.name = name;

        return obj;
    }

    public GameObject Get(
        string name,
        string path = null,
        UnityAction<GameObject> initAction = null
    )
    {
        GameObject obj;
        if (poolDict.TryGetValue(name, out Stack<GameObject> list) && list.Count > 0)
        {
            obj = poolDict[name].Pop();
            initAction?.Invoke(obj);
            obj.SetActive(true);
            return obj;
        }
        obj = GameObject.Instantiate(Resources.Load<GameObject>(path + name));
        obj.name = name;
        return obj;
    }

    public void Clear()
    {
        foreach (var stack in poolDict)
        {
            foreach (var gobj in stack.Value)
            {
                MonoBehaviour.Destroy(gobj);
            }
        }
        poolDict.Clear();
        instance = null;
    }
}
