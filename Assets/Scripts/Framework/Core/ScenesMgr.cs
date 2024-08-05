using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
///     场景管理器
/// </summary>
public class ScenesMgr : MonoBehaviour
{
    private static ScenesMgr _instance;

    public static ScenesMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new(nameof(ScenesMgr));
                _instance = obj.AddComponent<ScenesMgr>();
                DontDestroyOnLoad(obj);
            }

            return _instance;
        }
    }

    private void Clear()
    {
        GameObjectPoolMgr.Instance.Clear();
        ResourcesMgr.Instance.Clear();
        MyABMgr.Instance.Clear();
    }

    public void LoadScene(string sceneName, UnityAction callback = null)
    {
        Clear();
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    public void LoadSceneAsyn(string sceneName, UnityAction callback = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, callback));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, UnityAction callback)
    {
        Clear();
        var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return asyncOperation.progress;
            //更新加载进度条事件
            MyEventSystem.Instance.EventTrigger(EventEnum.UpdateProgress, asyncOperation.progress);
        }

        yield return asyncOperation;
        callback?.Invoke();
    }
}