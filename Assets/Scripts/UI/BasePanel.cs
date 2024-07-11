using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//需要优化同名问题
public class BasePanel<T1> : MonoBehaviour where T1 : class
{
    public static T1 Instance { get; private set; }
    public bool IsActive { get; private set; }
    private readonly Dictionary<string, List<UIBehaviour>> _controlDic = new();

    protected virtual void Awake()
    {
        Instance = this as T1;
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<TMP_InputField>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }

    public virtual void ShowMe()
    {
        IsActive = true;
    }

    public virtual void HideMe()
    {
        IsActive = false;
    }

    public virtual void Change()
    {
        if (IsActive)
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

    protected T1 GetControl<T1>(string controlName) where T1 : UIBehaviour
    {
        if (!_controlDic.ContainsKey(controlName)) return null;
        for (int i = 0; i < _controlDic[controlName].Count; ++i)
        {
            if (_controlDic[controlName][i] is T1)
            {
                return _controlDic[controlName][i] as T1;
            }
        }

        return null;
    }

    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = GetComponentsInChildren<T>(true);
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