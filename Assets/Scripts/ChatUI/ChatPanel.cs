using FishNet;
using FishNet.Broadcast;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct ChatMessage : IBroadcast
{
    public string Sender;
    public string Message;
}

public class ChatPanel : NetworkBehaviour
{
    [SerializeField] private GameObject content;
    private Button[] _buttons;
    [SerializeField] private TextMeshProUGUI messageText;


    private void Awake()
    {
        _buttons = GetComponentsInChildren<Button>(true);
    }

    private void OnEnable()
    {
        _buttons[0].onClick.AddListener(ButtonOnClick);
    }

    private void OnDisable()
    {
        _buttons[0].onClick.RemoveListener(ButtonOnClick);
    }

    private void ButtonOnClick()
    {
        SendChatMessage(Owner.ClientId.ToString(), messageText.text);
    }

    public void SendChatMessage(string sender, string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            Sender = sender,
            Message = message
        };
        // int i = messageText.GetTextInfo(messageText.text).lineCount();
        

        // 发送广播消息到所有客户端
        InstanceFinder.ServerManager.Broadcast<ChatMessage>(chatMessage);
    }
}