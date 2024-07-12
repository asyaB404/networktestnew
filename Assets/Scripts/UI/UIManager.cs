using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private readonly Stack<IBasePanel> _panels = new();

    private void Awake()
    {
        Instance = this;
        IBasePanel[] basePanels = GetComponentsInChildren<IBasePanel>(true);
        foreach (var panel in basePanels)
        {
            panel.Init();
        }
    }

    public void ClearPanels()
    {
        foreach (var panel in _panels)
        {
            PopPanel();
            (panel as MonoBehaviour)?.gameObject.SetActive(false);
        }

        _panels.Clear();
    }

    public void PushPanel(IBasePanel basePanel)
    {
        _panels.Push(basePanel);
    }

    public IBasePanel PopPanel()
    {
        if (_panels.Count <= 0)
        {
            Debug.LogError("栈为空");
            return null;
        }

        return _panels.Pop();
    }

    public IBasePanel Peek()
    {
        if (_panels.Count <= 0)
        {
            Debug.LogError("栈为空");
            return null;
        }

        return _panels.Peek();
    }
}