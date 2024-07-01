using FishNet;
using FishNet.Broadcast;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ChatUI
{
    public struct ChatMessage : IBroadcast
    {
        public string Sender;
        public string Message;
    }

    public class ChatPanel : NetworkBehaviour
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
            _buttons[0].onClick.AddListener(BroadCast);
        }

        private void OnDisable()
        {
            _buttons[0].onClick.RemoveListener(BroadCast);
        }

        private void BroadCast()
        {
            // SendChatMessage(Owner.ClientId.ToString(), messageText.text);
            if (string.IsNullOrEmpty(messageInput.text))
            {
                return;
            }

            GameObject newMessage = Instantiate(messagePrefab, content, false);
            Vector2 pos = newMessage.transform.localPosition;
            pos.y = -(totalLineCount + 1) * lineHeight;
            newMessage.transform.localPosition = pos;
            totalLineCount += newMessage.GetComponent<Message>().Init("233", messageInput.text);
            Vector2 size = content.sizeDelta;
            size.y = lineHeight * (totalLineCount + 2);
            content.sizeDelta = size;
            messageInput.text = null;
            if (size.y >= 700)
                content.localPosition = new(0, size.y - 700);
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
}