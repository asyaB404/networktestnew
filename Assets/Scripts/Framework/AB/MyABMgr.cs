using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public class MyABMgr : MonoBehaviour
{
    private static MyABMgr _instance;
    private readonly Dictionary<string, AssetBundle> _abDic = new();

    private AssetBundle _mainAb;
    private AssetBundleManifest _manifest;

    public static MyABMgr Instance
    {
        get
        {
            if (_instance != null) return _instance;
            GameObject gobj = new(typeof(MyABMgr).ToString());
            DontDestroyOnLoad(gobj);
            _instance = gobj.AddComponent<MyABMgr>();
            return _instance;
        }
    }

    private string PathUrl => Application.streamingAssetsPath + "/";

    private string MainABname
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    private void LoadAB(string ab)
    {
        if (!_mainAb)
        {
            _mainAb = AssetBundle.LoadFromFile(PathUrl + MainABname);
            _manifest = _mainAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        var strs = _manifest.GetAllDependencies(ab);
        //临时变量 
        AssetBundle assetBundle;
        foreach (var str in strs)
        {
            if (_abDic.ContainsKey(str)) continue;
            assetBundle = AssetBundle.LoadFromFile(PathUrl + str);
            _abDic.Add(str, assetBundle);
        }

        if (!_abDic.ContainsKey(ab))
        {
            assetBundle = AssetBundle.LoadFromFile(PathUrl + ab);
            _abDic.Add(ab, assetBundle);
        }
    }

    public Object LoadRes(string ab, string resName)
    {
        LoadAB(ab);
        var obj = _abDic[ab].LoadAsset(resName);
        return obj is GameObject ? Instantiate(obj) : obj;
    }

    public Object LoadRes(string ab, string resName, Type type)
    {
        LoadAB(ab);
        var obj = _abDic[ab].LoadAsset(resName, type);
        return obj is GameObject ? Instantiate(obj) : obj;
    }

    public T LoadRes<T>(string ab, string resName) where T : Object
    {
        LoadAB(ab);
        var obj = _abDic[ab].LoadAsset<T>(resName);
        if (obj is GameObject)
            return Instantiate(obj);
        return obj;
    }

    public void LoadResAsync(string ab, string resName, UnityAction<Object> callback)
    {
        StartCoroutine(Enumerator(ab, resName, callback));
    }

    private IEnumerator Enumerator(string ab, string resName, UnityAction<Object> callback)
    {
        LoadAB(ab);
        var assetBundleRequest = _abDic[ab].LoadAssetAsync(resName);
        yield return assetBundleRequest;
        callback(assetBundleRequest.asset is GameObject
            ? Instantiate(assetBundleRequest.asset)
            : assetBundleRequest.asset);
    }

    public void LoadResAsync(string ab, string resName, Type type, UnityAction<Object> callback)
    {
        StartCoroutine(Enumerator(ab, resName, type, callback));
    }

    private IEnumerator Enumerator(string ab, string resName, Type type, UnityAction<Object> callback)
    {
        LoadAB(ab);
        var assetBundleRequest = _abDic[ab].LoadAssetAsync(resName, type);
        yield return assetBundleRequest;
        callback(assetBundleRequest.asset is GameObject
            ? Instantiate(assetBundleRequest.asset)
            : assetBundleRequest.asset);
    }

    public void LoadResAsync<T>(string ab, string resName, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(Enumerator(ab, resName, callback));
    }

    private IEnumerator Enumerator<T>(string ab, string resName, UnityAction<T> callback) where T : Object
    {
        LoadAB(ab);
        var assetBundleRequest = _abDic[ab].LoadAssetAsync<T>(resName);
        yield return assetBundleRequest;
        if (assetBundleRequest.asset is GameObject)
            callback(Instantiate(assetBundleRequest.asset as T));
        else
            callback(assetBundleRequest.asset as T);
    }


    public void UnLoad(string ab, bool flag = false)
    {
        if (_abDic.TryGetValue(ab, out var assetBundle))
        {
            assetBundle.Unload(flag);
            _abDic.Remove(ab);
        }
    }

    public void Clear(bool flag = false)
    {
        AssetBundle.UnloadAllAssetBundles(flag);
        _abDic.Clear();
        _mainAb = null;
        _manifest = null;
    }
}