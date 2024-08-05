using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private readonly Stack<IBasePanel> _panels = new();
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            var basePanels = GetComponentsInChildren<IBasePanel>(true);
            foreach (var panel in basePanels) panel.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (Peek() != null)
                    Peek().OnPressedEsc();
        }

        public void ClearPanels()
        {
            foreach (var panel in _panels)
            {
                PopPanel(false);
                (panel as MonoBehaviour)?.gameObject.SetActive(false);
            }

            _panels.Clear();
        }


        /// <summary>
        ///     存入栈中
        /// </summary>
        /// <param name="basePanel"></param>
        /// <param name="callback">是否先执行栈顶的渐渐隐藏</param>
        public void PushPanel(IBasePanel basePanel, bool callback = true)
        {
            if (basePanel.IsInStack)
            {
                Debug.LogWarning("已经存在于栈内，无法再将其存入栈内");
                return;
            }

            if (callback && Peek() != null) Peek().CallBack(false);

            _panels.Push(basePanel);
            basePanel.CallBack(true);
        }

        /// <summary>
        ///     弹出栈顶元素
        /// </summary>
        /// <param name="callback">弹出后，是否执行新的栈顶的渐渐显示</param>
        public IBasePanel PopPanel(bool callback = true)
        {
            if (_panels.Count <= 0)
            {
                Debug.LogError("栈为空,不能弹出");
                return null;
            }

            if (Peek() != null) Peek().CallBack(false);

            var res = _panels.Pop();
            if (callback && Peek() != null) Peek().CallBack(true);

            return res;
        }

        public IBasePanel Peek()
        {
            return _panels.Count <= 0 ? null : _panels.Peek();
        }
    }
}