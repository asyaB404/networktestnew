using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Extension
{
    public static class ExtensionForUI
    {
        /// <summary>
        /// 以text组件的文本内容行数正确设置该组件的大小
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <returns></returns>
        public static int ReSetHeightFromText(this TextMeshProUGUI textMeshProUGUI)
        {
            int line = textMeshProUGUI.GetTextInfo(textMeshProUGUI.text).lineCount;
            float height = textMeshProUGUI.rectTransform.rect.height;
            Vector2 sizeDelta = textMeshProUGUI.rectTransform.sizeDelta;
            sizeDelta.y = line * height;
            textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
            return line;
        }

        /// <summary>
        /// 以text组件的文本内容行数以及自己所给的单行高度来正确设置该组件的大小
        /// </summary>
        /// <param name="textMeshProUGUI"></param>
        /// <param name="height">单行高度</param>
        /// <returns></returns>
        public static int ReSetHeightFromText(this TextMeshProUGUI textMeshProUGUI, float height)
        {
            int line = textMeshProUGUI.GetTextInfo(textMeshProUGUI.text).lineCount;
            Vector2 sizeDelta = textMeshProUGUI.rectTransform.sizeDelta;
            sizeDelta.y = line * height;
            textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
            return line;
        }

        public static RectTransform[] GetAllChildRectTransforms(this RectTransform rectTransform)
        {
            RectTransform[] res = new RectTransform[rectTransform.childCount];
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                res[i] = (RectTransform)rectTransform.GetChild(i);
            }

            return res;
        }

        public static void ResetSizeFromChilds(this RectTransform content, float duration = 0)
        {
            Vector2 size = content.sizeDelta;
            float y = 0;
            var childs = content.GetAllChildRectTransforms();
            foreach (var item in childs)
            {
                y += item.sizeDelta.y;
                y += duration;
            }

            y -= duration;
            size.y = y;
            content.sizeDelta = size;
        }
    }
}