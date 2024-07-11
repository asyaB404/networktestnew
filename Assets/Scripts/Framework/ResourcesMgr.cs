using System.Collections.Generic;
using UnityEngine;

public class ResourcesMgr
{
    private readonly Dictionary<string, Object> _resDict = new();
    public static ResourcesMgr Instance { get; } = new();


    public T LoadRes<T>(string path) where T : Object
    {
        if (_resDict.TryGetValue(path, out var res))
        {
            return res as T;
        }

        _resDict[path] = Resources.Load<T>(path);
        return _resDict[path] as T;
    }

    public void Clear()
    {
        _resDict.Clear();
    }
}