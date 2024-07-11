using System.Collections.Generic;
using UnityEngine;

public class ResourceMgr
{
    private readonly Dictionary<string, Object> _resDict = new();
    public static ResourceMgr Instance { get; } = new();


    public T LoadRes<T>(string path) where T : Object
    {
        if (_resDict.TryGetValue(path, out var res))
        {
            return res as T;
        }

        _resDict[path] = Resources.Load<T>(path);
        return _resDict[path] as T;
    }
}