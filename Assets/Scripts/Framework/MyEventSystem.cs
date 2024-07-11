using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfos { }

public class EventInfos<T, T1> : IEventInfos
{
    public UnityAction<T, T1> unityActions;

    public EventInfos() { }

    public EventInfos(UnityAction<T, T1> unityActions)
    {
        this.unityActions = unityActions;
    }
}

public class EventInfos<T> : IEventInfos
{
    public UnityAction<T> unityActions;

    public EventInfos() { }

    public EventInfos(UnityAction<T> unityActions)
    {
        this.unityActions = unityActions;
    }
}

public class EventInfos : IEventInfos
{
    public UnityAction unityActions;

    public EventInfos() { }

    public EventInfos(UnityAction unityActions)
    {
        this.unityActions = unityActions;
    }
}

/// <summary>
/// 事件中心
/// </summary>
public class MyEventSystem
{
    private static MyEventSystem instance;
    public static MyEventSystem Instance
    {
        get
        {
            instance ??= new();
            return instance;
        }
    }
    private readonly Dictionary<string, IEventInfos> eventDict = new();

    public void AddEventListener(string eventName, UnityAction action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).unityActions += action;
        }
        else
        {
            EventInfos eventInfos = new(action);
            eventDict.Add(eventName, eventInfos);
        }
    }

    public void AddEventListener<T>(string eventName, UnityAction<T> action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).unityActions += action;
        }
        else
        {
            EventInfos<T> eventInfos = new(action);
            eventDict.Add(eventName, eventInfos);
        }
    }

    public void AddEventListener<T, T1>(string eventName, UnityAction<T, T1> action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).unityActions += action;
        }
        else
        {
            EventInfos<T, T1> eventInfos = new(action);
            eventDict.Add(eventName, eventInfos);
        }
    }

    public void RemoveEventListener(string eventName, UnityAction action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).unityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void RemoveEventListener<T>(string eventName, UnityAction<T> action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).unityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void RemoveEventListener<T, T1>(string eventName, UnityAction<T, T1> action)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).unityActions -= action;
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,无法被移除");
        }
    }

    public void EventTrigger(string eventName)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos).unityActions?.Invoke();
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void EventTrigger<T>(string eventName, T eventData)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T>).unityActions?.Invoke(eventData);
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void EventTrigger<T, T1>(string eventName, T eventData, T1 eventData1)
    {
        if (eventDict.TryGetValue(eventName, out IEventInfos existingAction))
        {
            (existingAction as EventInfos<T, T1>).unityActions?.Invoke(eventData, eventData1);
        }
        else
        {
            Debug.LogWarning("-------->   " + eventName + "事件为空,不能被触发");
        }
    }

    public void Clear(string eventName)
    {
        eventDict.Remove(eventName);
    }

    public void Clear()
    {
        eventDict.Clear();
    }
}
