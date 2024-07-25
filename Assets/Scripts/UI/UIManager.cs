using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        private readonly Stack<IBasePanel> _panels = new();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Peek() != null)
                {
                    Peek().HideMe(true);
                }
            }
        }

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
                PopPanel(false);
                (panel as MonoBehaviour)?.gameObject.SetActive(false);
            }

            _panels.Clear();
        }

        public void PushPanel(IBasePanel basePanel, bool callback = true)
        {
            if (basePanel.IsInStack)
            {
                Debug.LogWarning("已经存在于栈内，无法再将其存入栈内");
                return;
            }

            if (Peek() != null)
            {
                Peek().CallBack(false);
            }

            _panels.Push(basePanel);
            if (callback)
            {
                basePanel.CallBack(true);
            }
        }

        public IBasePanel PopPanel(bool callback = true)
        {
            if (_panels.Count <= 0)
            {
                Debug.LogError("栈为空,不能弹出");
                return null;
            }

            if (Peek() != null)
            {
                Peek().CallBack(false);
            }

            var res = _panels.Pop();
            if (callback && Peek() != null)
            {
                Peek().CallBack(true);
            }

            return res;
        }

        public IBasePanel Peek()
        {
            return _panels.Count <= 0 ? null : _panels.Peek();
        }
    }
}