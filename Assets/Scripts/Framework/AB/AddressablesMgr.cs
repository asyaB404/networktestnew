using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesInfo
{
    public AsyncOperationHandle Handle;
    public uint Count;

    public AddressablesInfo(AsyncOperationHandle handle)
    {
        this.Handle = handle;
        Count += 1;
    }
}

public class AddressablesMgr
{
    public static AddressablesMgr Instance { get; private set; } = new();

    //有一个容器 帮助我们存储 异步加载的返回值
    private readonly Dictionary<string, AddressablesInfo> _resDic = new();

    //异步加载资源的方法
    public void LoadAssetAsync<T>(string name, Action<AsyncOperationHandle<T>> callBack)
    {
        //由于存在同名 不同类型资源的区分加载
        //所以我们通过名字和类型拼接作为 key
        string keyName = name + "_" + typeof(T).Name;
        AsyncOperationHandle<T> handle;
        //如果已经加载过该资源
        if (_resDic.ContainsKey(keyName))
        {
            //获取异步加载返回的操作内容
            handle = _resDic[keyName].Handle.Convert<T>();
            //要使用资源了 那么引用计数+1
            _resDic[keyName].Count += 1;
            //判断 这个异步加载是否结束
            if (handle.IsDone)
            {
                //如果成功 就不需要异步了 直接相当于同步调用了 这个委托函数 传入对应的返回值
                callBack(handle);
            }
            //还没有加载完成
            else
            {
                //如果这个时候 还没有异步加载完成 那么我们只需要 告诉它 完成时做什么就行了
                handle.Completed += (obj) =>
                {
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                        callBack(obj);
                };
            }

            return;
        }

        //如果没有加载过该资源
        //直接进行异步加载 并且记录
        handle = Addressables.LoadAssetAsync<T>(name);
        handle.Completed += (obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                callBack(obj);
            else
            {
                Debug.LogWarning(keyName + "资源加载失败");
                if (_resDic.ContainsKey(keyName))
                    _resDic.Remove(keyName);
            }
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        _resDic.Add(keyName, info);
    }

    //释放资源的方法 
    public void Release<T>(string name)
    {
        //由于存在同名 不同类型资源的区分加载
        //所以我们通过名字和类型拼接作为 key
        string keyName = name + "_" + typeof(T).Name;
        if (_resDic.ContainsKey(keyName))
        {
            //释放时 引用计数-1
            _resDic[keyName].Count -= 1;
            //如果引用计数为0  才真正的释放
            if (_resDic[keyName].Count == 0)
            {
                //取出对象 移除资源 并且从字典里面移除
                AsyncOperationHandle<T> handle = _resDic[keyName].Handle.Convert<T>();
                Addressables.Release(handle);
                _resDic.Remove(keyName);
            }
        }
    }

    //异步加载多个资源 或者 加载指定资源
    public void LoadAssetAsync<T>(Addressables.MergeMode mode, Action<T> callBack, params string[] keys)
    {
        //1.构建一个keyName  之后用于存入到字典中
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;
        //2.判断是否存在已经加载过的内容 
        //存在做什么
        AsyncOperationHandle<IList<T>> handle;
        if (_resDic.ContainsKey(keyName))
        {
            handle = _resDic[keyName].Handle.Convert<IList<T>>();
            //要使用资源了 那么引用计数+1
            _resDic[keyName].Count += 1;
            //异步加载是否结束
            if (handle.IsDone)
            {
                foreach (T item in handle.Result)
                    callBack(item);
            }
            else
            {
                handle.Completed += (obj) =>
                {
                    //加载成功才调用外部传入的委托函数
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (T item in handle.Result)
                            callBack(item);
                    }
                };
            }

            return;
        }

        //不存在做什么
        handle = Addressables.LoadAssetsAsync<T>(list, callBack, mode);
        handle.Completed += (obj) =>
        {
            if (obj.Status != AsyncOperationStatus.Failed) return;
            Debug.LogError("资源加载失败" + keyName);
            if (_resDic.ContainsKey(keyName))
                _resDic.Remove(keyName);
        };
        AddressablesInfo info = new AddressablesInfo(handle);
        _resDic.Add(keyName, info);
    }

    public void LoadAssetAsync<T>(Addressables.MergeMode mode, Action<AsyncOperationHandle<IList<T>>> callBack,
        params string[] keys)
    {
    }

    public void Release<T>(params string[] keys)
    {
        //1.构建一个keyName  之后用于存入到字典中
        List<string> list = new List<string>(keys);
        string keyName = "";
        foreach (string key in list)
            keyName += key + "_";
        keyName += typeof(T).Name;

        if (_resDic.ContainsKey(keyName))
        {
            _resDic[keyName].Count -= 1;
            if (_resDic[keyName].Count == 0)
            {
                //取出字典里面的对象
                AsyncOperationHandle<IList<T>> handle = _resDic[keyName].Handle.Convert<IList<T>>();
                Addressables.Release(handle);
                _resDic.Remove(keyName);
            }
        }
    }

    //清空资源
    public void Clear()
    {
        foreach (var item in _resDic.Values)
        {
            Addressables.Release(item.Handle);
        }

        _resDic.Clear();
        AssetBundle.UnloadAllAssetBundles(true);
        GC.Collect();
    }
}