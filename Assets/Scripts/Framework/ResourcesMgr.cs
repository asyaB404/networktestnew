using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesMgr
{
    private readonly Dictionary<string, Object> _resDict = new();
    public static ResourcesMgr Instance { get; } = new();


    //同步加载资源，加载完成后将会
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
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T:Object
    {
        ScenesMgr.Instance.StartCoroutine(ReallyLoadAsync(name, callback));
    }
    
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
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