using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public interface IBasePanel
    {
        /// <summary>
        /// 初始化方法
        /// </summary>
        void Init();

        /// <summary>
        /// 是否存在于栈内
        /// </summary>
        bool IsInStack { get; }

        /// <summary>
        /// showme的同时会将其存入栈内
        /// </summary>
        void ShowMe();

        /// <summary>
        /// hideme的同时会尝试将其弹出，注意不能调用栈顶以外面板的hideme
        /// </summary>
        void HideMe(bool isPressedEsc = false);

        /// <summary>
        /// 根据isactive来决定激活showme还是hideme
        /// </summary>
        void ChangeMe();

        /// <summary>
        /// 回调，当uimanager中的栈顶元素变化自己为调用
        /// </summary>
        void CallBack(bool flag);
    }

    public class BasePanel<T1> : MonoBehaviour, IBasePanel where T1 : class
    {
        public static T1 Instance { get; private set; }
        public bool IsInStack { get; private set; }
        private readonly Dictionary<string, List<UIBehaviour>> _controlDic = new();
        private CanvasGroup _canvasGroup;

        protected CanvasGroup CanvasGroupInstance
        {
            get
            {
                _canvasGroup ??= gameObject.AddComponent<CanvasGroup>();
                return _canvasGroup;
            }
        }

        public virtual void Init()
        {
            Instance = this as T1;
            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<TextMeshProUGUI>();
            FindChildrenControl<TMP_InputField>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<ToggleGroup>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<InputField>();
        }


        public virtual void ShowMe()
        {
            if (IsInStack) return;
            UIManager.Instance.PushPanel(this);
            gameObject.SetActive(true);
            gameObject.transform.SetAsLastSibling();
            IsInStack = true;
        }

        /// <summary>
        /// 一般只对栈顶的元素执行,若不是栈顶元素执行,将会弹出该元素之上的所有元素
        /// </summary>
        /// <param name="isPressedEsc">是否按下Esc按键来触发</param>
        public virtual void HideMe(bool isPressedEsc = false)
        {
            if (!IsInStack) return;
            if (
                ReferenceEquals(UIManager.Instance.Peek(), this)
            )
            {
                UIManager.Instance.PopPanel();
                IsInStack = false;
            }
            else
            {
                // Debug.LogWarning("你不能关闭栈顶以为的面板");
                while (!ReferenceEquals(UIManager.Instance.Peek(), this))
                {
                    UIManager.Instance.Peek().HideMe();
                }
            }
        }

        public virtual void ChangeMe()
        {
            if (IsInStack)
                HideMe(false);
            else
                ShowMe();
        }

        /// <summary>
        /// true表示该面板作为新的元素入栈顶时的标记（可以写渐渐出现的逻辑），false时表示新的元素入栈后原来的栈顶（this）的标记（可以写消失的逻辑）
        /// </summary>
        /// <param name="flag">true表示为栈顶，flag表示有新的元素替代了原来的栈顶</param>
        /// <example>例如栈为[1],新push了一个2变为[1,2],此时1会执行CallBack(false),2会执行CallBack(true)</example>
        public virtual void CallBack(bool flag)
        {
            transform.DOKill(true);
            if (flag)
            {
                CanvasGroupInstance.interactable = true;
                gameObject.SetActive(true);
                transform.localPosition = new Vector3(-1500, 0, 0);
                transform.DOLocalMoveX(0, UIConst.UIDuration);
            }
            else
            {
                CanvasGroupInstance.interactable = false;
                transform.DOLocalMoveX(-1500, UIConst.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
            }
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
}