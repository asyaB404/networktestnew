using System;
using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

// public struct ChatMessage : IBroadcast
// {
//     public string Sender;
//     public string Message;
// }

public class ChatManager : NetworkBehaviour
{
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

public class ChatListener : MonoBehaviour
{
    private void OnEnable()
    {
        // 注册广播消息的处理方法
        InstanceFinder.ClientManager.RegisterBroadcast<ChatMessage>(OnChatMessageReceived);
    }

    private void OnDisable()
    {
        // 注销广播消息的处理方法
        InstanceFinder.ClientManager.UnregisterBroadcast<ChatMessage>(OnChatMessageReceived);
    }


    private void OnChatMessageReceived(ChatMessage chatMessage, Channel channel)
    {
        // 处理接收到的广播消息
        Debug.Log($"Message from {chatMessage.Sender}: {chatMessage.Message}");
        // 你可以在这里更新 UI 或执行其他逻辑
    }
}