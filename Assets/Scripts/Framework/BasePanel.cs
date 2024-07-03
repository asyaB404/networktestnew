using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类 
/// 帮助我门通过代码快速的找到所有的子控件
/// 方便我们在子类中处理逻辑 
/// 节约找控件的工作量
/// </summary>
public class BasePanel<T> : MonoBehaviour where T : class
{
    public static T Instance { get; private set; }
    public bool IsShowing { get; private set; }
    private readonly Dictionary<string, List<UIBehaviour>> _controlDic = new();

    protected virtual void Awake()
    {
        Instance = this as T;
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }

    public virtual void ShowMe()
    {
        IsShowing = true;
    }

    public virtual void HideMe()
    {
        IsShowing = false;
    }

    public virtual void Change()
    {
        if (IsShowing)
            HideMe();
        else
            ShowMe();
    }

    //可以在这里选择添加音效等
    protected virtual void OnClick(string btnName)
    {
    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {
    }
    
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (!_controlDic.ContainsKey(controlName)) return null;
        for (int i = 0; i < _controlDic[controlName].Count; ++i)
        {
            if (_controlDic[controlName][i] is T)
            {
                return _controlDic[controlName][i] as T;
            }
        }

        return null;
    }
    
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].gameObject.name;
            if (_controlDic.TryGetValue(objName, value: out var value1))
            {
                value1.Add(controls[i]);
            }
            else
            {
                _controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            }
            
            if (controls[i] is Button)
            {
                (controls[i] as Button)?.onClick.AddListener(() => { OnClick(objName); });
            }
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle)?.onValueChanged.AddListener((value) => { OnValueChanged(objName, value); });
            }
        }
    }
}