using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfos
{
}

public class EventInfos<T, T1> : IEventInfos
{
    public UnityAction<T, T1> UnityActions;

    public EventInfos()
    {
    }

    public EventInfos(UnityAction<T, T1> unityActions)
    {
        this.UnityActions = unityActions;
    }
}

public class EventInfos<T> : IEventInfos
{
    public UnityAction<T> UnityActions;

    public EventInfos()
    {
    }

    public EventInfos(UnityAction<T> unityActions)
    {
        this.UnityActions = unityActions;
    }
}

public class EventInfos : IEventInfos
{
    public UnityAction UnityActions;

    public EventInfos()
    {
    }

    public EventInfos(UnityAction unityActions)
    {
        this.UnityActions = unityActions;
    }
}

/// <summary>
/// 事件中心
/// </summary>
public class MyEventSystem
{
    private static MyEventSystem _instance;

    public static MyEventSystem Instance
    {
        get
        {
            _instance ??= new MyEventSystem();
            return _instance;
        }
    }

    private readonly Dictionary<string, IEventInfos> _eventDict = new();

    public void AddEventListener(string eventName, UnityAction action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).UnityActions += action;
        }
        else
        {
            EventInfos eventInfos = new(action);
            _eventDict.Add(eventName, eventInfos);
        }
    }

    public void AddEventListener<T>(string eventName, UnityAction<T> action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).UnityActions += action;
        }
        else
        {
            EventInfos<T> eventInfos = new(action);
            _eventDict.Add(eventName, eventInfos);
        }
    }

    public void AddEventListener<T, T1>(string eventName, UnityAction<T, T1> action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).UnityActions += action;
        }
        else
        {
            EventInfos<T, T1> eventInfos = new(action);
            _eventDict.Add(eventName, eventInfos);
        }
    }

    public void RemoveEventListener(string eventName, UnityAction action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).UnityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void RemoveEventListener<T>(string eventName, UnityAction<T> action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).UnityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void RemoveEventListener<T, T1>(string eventName, UnityAction<T, T1> action)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).UnityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void EventTrigger(string eventName)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).UnityActions?.Invoke();
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void EventTrigger<T>(string eventName, T eventData)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).UnityActions?.Invoke(eventData);
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void EventTrigger<T, T1>(string eventName, T eventData, T1 eventData1)
    {
        if (_eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).UnityActions?.Invoke(eventData, eventData1);
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void Clear(string eventName)
    {
        _eventDict.Remove(eventName);
    }

    public void Clear()
    {
        _eventDict.Clear();
    }
}