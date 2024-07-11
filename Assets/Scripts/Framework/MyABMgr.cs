using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyABMgr : MonoBehaviour
{
    private static MyABMgr instance;
    public static MyABMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gobj = new(typeof(MyABMgr).ToString());
                instance = gobj.AddComponent<MyABMgr>();
            }
            return instance;
        }
    }
    private AssetBundle mainAB;
    private AssetBundleManifest manifest;
    private readonly Dictionary<string, AssetBundle> abDic = new();

    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

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

    public void LoadAB(string ab)
    {
        if (!mainAB)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABname);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        string[] strs = manifest.GetAllDependencies(ab);
        //临时变量 
        AssetBundle assetBundle;
        foreach (string str in strs)
        {
            if (!abDic.ContainsKey(str))
            {
                assetBundle = AssetBundle.LoadFromFile(PathUrl + str);
                abDic.Add(str, assetBundle);
            }
        }
        if (!abDic.ContainsKey(ab))
        {
            assetBundle = AssetBundle.LoadFromFile(PathUrl + ab);
            abDic.Add(ab, assetBundle);
        }
    }

    public Object LoadRes(string ab, string name)
    {
        LoadAB(ab);
        Object obj = abDic[ab].LoadAsset(name);
        if (obj is GameObject)
            return Instantiate(obj);
        return obj;
    }

    public Object LoadRes(string ab, string name, System.Type type)
    {
        LoadAB(ab);
        Object obj = abDic[ab].LoadAsset(name, type);
        if (obj is GameObject)
            return Instantiate(obj);
        return obj;
    }

    public T LoadRes<T>(string ab, string name) where T : Object
    {
        LoadAB(ab);
        T obj = abDic[ab].LoadAsset<T>(name);
        if (obj is GameObject)
            return Instantiate(obj);
        return obj;
    }

    public void LoadResAsync(string ab, string name, UnityAction<Object> callback)
    {
        StartCoroutine(Enumerator(ab, name, callback));
    }
    private IEnumerator Enumerator(string ab, string name, UnityAction<Object> callback)
    {
        LoadAB(ab);
        AssetBundleRequest assetBundleRequest = abDic[ab].LoadAssetAsync(name);
        yield return assetBundleRequest;
        if (assetBundleRequest.asset is GameObject)
            callback(Instantiate(assetBundleRequest.asset));
        else
            callback(assetBundleRequest.asset);
    }

    public void LoadResAsync(string ab, string name, System.Type type, UnityAction<Object> callback)
    {
        StartCoroutine(Enumerator(ab, name, type, callback));
    }
    private IEnumerator Enumerator(string ab, string name, System.Type type, UnityAction<Object> callback)
    {
        LoadAB(ab);
        AssetBundleRequest assetBundleRequest = abDic[ab].LoadAssetAsync(name, type);
        yield return assetBundleRequest;
        if (assetBundleRequest.asset is GameObject)
            callback(Instantiate(assetBundleRequest.asset));
        else
            callback(assetBundleRequest.asset);
    }

    public void LoadResAsync<T>(string ab, string name, UnityAction<T> callback) where T : Object
    {
        StartCoroutine(Enumerator<T>(ab, name, callback));
    }
    private IEnumerator Enumerator<T>(string ab, string name, UnityAction<T> callback) where T : Object
    {
        LoadAB(ab);
        AssetBundleRequest assetBundleRequest = abDic[ab].LoadAssetAsync<T>(name);
        yield return assetBundleRequest;
        if (assetBundleRequest.asset is GameObject)
            callback(Instantiate(assetBundleRequest.asset as T));
        else
            callback(assetBundleRequest.asset as T);
    }


    public void UnLoad(string ab, bool flag = false)
    {
        if (abDic.TryGetValue(ab, out AssetBundle assetBundle))
        {
            assetBundle.Unload(flag);
            abDic.Remove(ab);
        }
    }

    public void ClearAB(bool flag = false)
    {
        AssetBundle.UnloadAllAssetBundles(flag);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
