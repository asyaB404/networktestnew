using FishNet;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public static ChatPanel Instance;
        [SerializeField] private RectTransform content;
        private Button[] _buttons;
        [SerializeField] private TMP_InputField messageInput;
        public int totalLineCount = 0;

        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private int lineHeight = 40;


        private void Awake()
        {
            Instance = this;
            _buttons = GetComponentsInChildren<Button>(true);
        }

        private void OnEnable()
        {
            _buttons[0].onClick.AddListener(SendChatMessage);
            messageInput.onSubmit.AddListener(OnInputSubmit);
            messageInput.onValidateInput += ValidateInput;
            InstanceFinder.ClientManager.RegisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
        }

        private char ValidateInput(string text, int charIndex, char addedChar)
        {
            // 忽略首个Enter键的输入
            if (charIndex == 0 && addedChar is '\n' or '\r')
            {
                return '\0'; //返回空字符表示忽略该输入
            }

            return addedChar; //返回输入的字符
        }

        private void OnDisable()
        {
            Clear();
            _buttons[0].onClick.RemoveListener(SendChatMessage);
            messageInput.onSubmit.RemoveListener(OnInputSubmit);
            messageInput.onValidateInput -= ValidateInput;
            InstanceFinder.ClientManager.UnregisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.UnregisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
        }

        private void OnClientChatMessageReceived(ChatMessage chatMessage, Channel channel)
        {
            SpawnMsg(chatMessage);
        }

        private void OnServerChatMessageReceived(
            NetworkConnection networkConnection,
            ChatMessage chatMessage,
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
            if (size.y >= 700)
                content.localPosition = new Vector3(0, size.y - 700);
        }

        private void SendChatMessage()
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = PlayerPrefsMgr.PlayerName,
                Message = messageInput.text
            };
            if (InstanceFinder.IsServerStarted)
            {
                InstanceFinder.ServerManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                InstanceFinder.ClientManager.Broadcast(chatMessage);
                messageInput.text = null;
            }
        }

        private void OnInputSubmit(string message)
        {
            SendChatMessage();
            EventSystem.current.SetSelectedGameObject(messageInput.gameObject, null);
            messageInput.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public void Clear()
        {
            (content as Transform).DestroyAllChildren();
            Vector3 size = content.sizeDelta;
            size.y = 700;
            content.sizeDelta = size;
            content.localPosition = new Vector3(0, 0);
            totalLineCount = 0;
        }
    }
}