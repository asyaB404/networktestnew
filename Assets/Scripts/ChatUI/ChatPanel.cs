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
        [SerializeField] private GameObject content;
        private Button[] _buttons;
        [FormerlySerializedAs("messageText")] [SerializeField] private TextMeshProUGUI messageInput;
        public int totalLineCount = 0;

        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private int lineHeight = 40;


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
            // SendChatMessage(Owner.ClientId.ToString(), messageText.text);
            GameObject newMessage = Instantiate(messagePrefab, content.transform, false);
            Vector2 pos = newMessage.transform.position;
            pos.y = -totalLineCount * lineHeight;
            newMessage.transform.position = pos;
            totalLineCount += newMessage.GetComponent<Message>().Init("233",messageInput.text);
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