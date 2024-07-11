using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesMgr : MonoBehaviour
{
    public static ScenesMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new(nameof(ScenesMgr));
                instance = obj.AddComponent<ScenesMgr>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    private static ScenesMgr instance;

    private void Clear()
    {
        PoolManager.Instance.Clear();
        MainUI.Instance.ClearPanels();
        MyABMgr.Instance.ClearAB();
        UIForMap.Instance.Clear();
    }

    public void LoadScene(string name, UnityAction callback = null)
    {
        Clear();
        SceneManager.LoadScene(name);
        callback?.Invoke();
    }

    public void LoadSceneAsyn(string name, UnityAction callback = null)
    {
        StartCoroutine(LoadSceneCoroutine(name, callback));
    }

    private IEnumerator LoadSceneCoroutine(string name, UnityAction callback)
    {
        Clear();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
        float y = 0;
        while (y < 0.8)
        {
            MyEventSystem.Instance.EventTrigger<float>("update_progress", y);
            y += 0.2f;
            yield return null;
        }
        while (!asyncOperation.isDone)
        {
            yield return asyncOperation.progress;
            MyEventSystem.Instance.EventTrigger<float>("update_progress", asyncOperation.progress);
        }
        yield return asyncOperation;
        yield return new WaitForSeconds(0.5f);
        callback?.Invoke();
    }
}
