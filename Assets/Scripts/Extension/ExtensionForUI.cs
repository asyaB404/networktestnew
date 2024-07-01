using TMPro;
using UnityEngine;

namespace Extension
{
    public static class ExtensionForUI
    {
        public static void SetHeightFromText(this TextMeshProUGUI textMeshProUGUI)
        {
            int line = textMeshProUGUI.GetTextInfo(textMeshProUGUI.text).lineCount;
            float height = textMeshProUGUI.rectTransform.rect.height;
            Vector2 sizeDelta = textMeshProUGUI.rectTransform.sizeDelta;
            sizeDelta.y = line * height;
            textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
        }

        public static void SetHeightFromText(this TextMeshProUGUI textMeshProUGUI, float height)
        {
            int line = textMeshProUGUI.GetTextInfo(textMeshProUGUI.text).lineCount;
            Vector2 sizeDelta = textMeshProUGUI.rectTransform.sizeDelta;
            sizeDelta.y = line * height;
            textMeshProUGUI.rectTransform.sizeDelta = sizeDelta;
        }
    }
}