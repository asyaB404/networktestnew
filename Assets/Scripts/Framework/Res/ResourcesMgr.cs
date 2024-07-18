using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        return (T)_resDict[path];
    }
    
    //异步加载资源
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T:Object
    {
        ScenesMgr.Instance.StartCoroutine(ReallyLoadAsync(path, callback));
    }
    
    private IEnumerator ReallyLoadAsync<T>(string path, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(path);
        yield return r;

        if (r.asset is GameObject)
            callback(Object.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }

    public void Clear()
    {
        Resources.UnloadUnusedAssets();
        _resDict.Clear();
    }
}