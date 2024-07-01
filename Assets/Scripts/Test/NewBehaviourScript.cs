using System.Collections;
using System.Collections.Generic;
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
}