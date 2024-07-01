using TMPro;
using UnityEngine;

namespace Extension
{
    public static class ExtensionForUI
    {
        // 定义一个扩展方法来获取行数
        public static void SetTextGUIHeight(this TextMeshProUGUI textMeshProUGUI)
        {
            int line = textMeshProUGUI.GetTextInfo(textMeshProUGUI.text).lineCount;
            float height = textMeshProUGUI.rectTransform.rect.height;
            Vector2 sizeDelta = textMeshProUGUI.rectTransform.sizeDelta;
            sizeDelta.y = line * height;
            textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
        }
    }
}