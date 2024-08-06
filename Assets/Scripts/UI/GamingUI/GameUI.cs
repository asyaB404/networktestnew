using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GamingUI
{
    /// <summary>
    ///     BasePanel的轻量版,一般作为Panel内部的能够独立分开进行打开和关闭操作的轻量级元素控件的基类
    /// </summary>
    /// <example>比如菜单栏，任务栏，小地图等等</example>
    public class GameUI : MonoBehaviour
    {
        /// <summary>
        /// 按父>子,上>下的顺序的按钮组
        /// </summary>
        protected Button[] buttons;

        protected virtual void Awake()
        {
            buttons = GetComponentsInChildren<Button>(true);
        }

        public virtual void ShowMe()
        {
            gameObject.SetActive(true);
        }

        public virtual void HideMe()
        {
            gameObject.SetActive(false);
        }

        public virtual void ChangeMe()
        {
            if (gameObject.activeSelf)
                HideMe();
            else
                ShowMe();
        }
    }
}