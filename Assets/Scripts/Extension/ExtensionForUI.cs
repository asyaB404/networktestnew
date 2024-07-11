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
    }
}