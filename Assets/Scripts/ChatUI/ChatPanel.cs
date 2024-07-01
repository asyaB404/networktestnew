using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Broadcast;
using UnityEngine;
using UnityEngine.UI;


public struct ChatMessage : IBroadcast
{
    public string Sender;
    public string Message;
}

public class ChatPanel : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Button[] buttons;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>(true);
    }

    private void OnEnable()
    {
        buttons[0].onClick.AddListener(ButtonOnClick);
    }

    private void OnDisable()
    {
        buttons[0].onClick.RemoveListener(ButtonOnClick);
    }

    private void ButtonOnClick()
    {
    }
}