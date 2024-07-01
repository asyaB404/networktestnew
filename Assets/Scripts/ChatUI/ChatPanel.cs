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
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI messageText;


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
        SendChatMessage(Owner.ClientId.ToString(), messageText.text);
    }

    public void SendChatMessage(string sender, string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            Sender = sender,
            Message = message
        };

        // 发送广播消息到所有客户端
        InstanceFinder.ServerManager.Broadcast<ChatMessage>(chatMessage);
    }
}