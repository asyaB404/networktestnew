using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// GameObject对象池
///当你尝试访问Instance单例时将会自动创建唯一一个名为Pool的gameobject作为对象池的父物体
/// 记得在过场景的时候手动clear一下
/// </summary>
public class GameObjectPoolMgr
{
    private static GameObjectPoolMgr _instance;

    public static GameObjectPoolMgr Instance
    {
        get
        {
            _instance ??= new GameObjectPoolMgr();
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

    public interface ILoadConfig
    {
    }

    public struct ResLoadConfig : ILoadConfig
    {
        public string ResPath;

        public ResLoadConfig(string resPath)
        {
            ResPath = resPath;
        }
    }

    public struct PrefabsLoadConfig : ILoadConfig
    {
        public GameObject Prefab;

        public PrefabsLoadConfig(GameObject prefab)
        {
            Prefab = prefab;
        }
    }

    public struct ABLoadConfig : ILoadConfig
    {
        public string AbName;
        public string ResName;

        public ABLoadConfig(string abName, string resName)
        {
            AbName = abName;
            ResName = resName;
        }
    }

    /// <summary>
    /// 传入id和配置信息，根据配置信息的具体类型来决定通过那种方法生成对象
    /// </summary>
    /// <example>Get("123",new PrefabsLoadConfig(prefab)); Get("123",new ResLoadConfig("Prefabs/Bullet")).....</example>
    public GameObject Get(string id, ILoadConfig config)
    {
        
        GameObject obj;
        if (_poolDict.TryGetValue(id, out Stack<GameObject> stack) && stack.Count > 0)
        {
            obj = _poolDict[id].Pop();
            while (obj == null && _poolDict.Count > 0)
            {
                obj = _poolDict[id].Pop();
            }

            if (obj != null)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        if (config is ResLoadConfig resConfig)
        {
            obj = Object.Instantiate(Resources.Load<GameObject>(resConfig.ResPath));
        }
        else if (config is PrefabsLoadConfig prefabConfig)
        {
            obj = Object.Instantiate(prefabConfig.Prefab);
        }
        else if (config is ABLoadConfig abConfig)
        {
            obj = MyABMgr.Instance.LoadRes<GameObject>(abConfig.AbName, abConfig.ResName);
        }
        else
        {
            Debug.LogError("未知的加载配置信息，无法正确实例化，返回一个空物体");
            obj = new GameObject();
        }

        obj.name = id;
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