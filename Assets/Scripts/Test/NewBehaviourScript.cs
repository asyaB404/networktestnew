using System.Collections;
using System.Collections.Generic;
using Extension;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [ContextMenu("Test")]
    private void Test()
    {
        TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
        int i = component.GetTextInfo(component.text).lineCount;
        Debug.Log(i);
    }
    
    [ContextMenu("Test1")]
        private void Test1()
        {
            TextMeshProUGUI component = GetComponent<TextMeshProUGUI>();
            component.ReSetHeightFromText();
        }
}