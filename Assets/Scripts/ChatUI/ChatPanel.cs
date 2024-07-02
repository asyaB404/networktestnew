using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChatUI
{
    public struct ChatMessage : IBroadcast
    {
        public string Sender;
        public string Message;
    }

    public class ChatPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        private Button[] _buttons;
        [SerializeField] private TMP_InputField messageInput;
        public int totalLineCount = 0;

        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private int lineHeight = 40;


        private void Awake()
        {
            _buttons = GetComponentsInChildren<Button>(true);
        }

        private void OnEnable()
        {
            _buttons[0].onClick.AddListener(SendChatMessage);
            InstanceFinder.ClientManager.RegisterBroadcast<ChatMessage>(OnChatMessageReceived);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
        }

        private void OnDisable()
        {
            _buttons[0].onClick.RemoveListener(SendChatMessage);
            InstanceFinder.ClientManager.UnregisterBroadcast<ChatMessage>(OnChatMessageReceived);
            InstanceFinder.ServerManager.UnregisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
        }

        private void OnChatMessageReceived(ChatMessage chatMessage, Channel channel)
        {
            SpawnMsg(chatMessage);
        }

        private void OnClientChatMessageReceived(NetworkConnection networkConnection, ChatMessage chatMessage,
            Channel channel)
        {
            InstanceFinder.ServerManager.Broadcast(chatMessage);
        }

        private void SpawnMsg(ChatMessage chatMessage)
        {
            if (string.IsNullOrEmpty(chatMessage.Message))
            {
                return;
            }

            GameObject newMessage = Instantiate(messagePrefab, content, false);
            Vector2 pos = newMessage.transform.localPosition;
            pos.y = -totalLineCount * lineHeight;
            newMessage.transform.localPosition = pos;
            totalLineCount += newMessage.GetComponent<Message>().Init(chatMessage.Sender, chatMessage.Message) + 1;
            Vector2 size = content.sizeDelta;
            size.y = lineHeight * (totalLineCount + 2);
            content.sizeDelta = size;
            messageInput.text = null;
            if (size.y >= 700)
                content.localPosition = new Vector3(0, size.y - 700);
        }

        public void SendChatMessage()
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = "233:",
                Message = messageInput.text
            };
            if (InstanceFinder.IsServerStarted)
            {
                InstanceFinder.ServerManager.Broadcast(chatMessage);
            }
            else if (InstanceFinder.IsClientStarted)
            {
                InstanceFinder.ClientManager.Broadcast(chatMessage);
            }
        }
    }
}