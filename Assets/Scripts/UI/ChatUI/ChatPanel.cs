using DG.Tweening;
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

        public ChatMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }

    public class ChatPanel : MonoBehaviour, IPointerEnterHandler
    {
        public static ChatPanel Instance;
        private int _totalLineCount = 0;
        private Button[] _buttons;
        [SerializeField] private RectTransform content;
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private int lineHeight = 40;


        private void Awake()
        {
            Instance = this;
            _buttons = GetComponentsInChildren<Button>(true);
        }


        private void OnEnable()
        {
            ShowChatPanelCoroutine();
            _buttons[0].onClick.AddListener(SendInputField);
            messageInput.onSubmit.AddListener(OnInputSubmit);
            messageInput.onValidateInput += ValidateInput;
            InstanceFinder.ClientManager.RegisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.RegisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
            InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnection;
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
            _buttons[0].onClick.RemoveListener(SendInputField);
            messageInput.onSubmit.RemoveListener(OnInputSubmit);
            messageInput.onValidateInput -= ValidateInput;
            InstanceFinder.ClientManager.UnregisterBroadcast<ChatMessage>(OnClientChatMessageReceived);
            InstanceFinder.ServerManager.UnregisterBroadcast<ChatMessage>(OnServerChatMessageReceived);
            InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnection;
        }

        private void OnRemoteConnection(NetworkConnection connection, RemoteConnectionStateArgs obj)
        {
            if (obj.ConnectionState == RemoteConnectionState.Started)
            {
                InstanceFinder.ServerManager.Broadcast(new ChatMessage("   ", "玩家" + connection + "加入了游戏"));
            }
            else if (obj.ConnectionState == RemoteConnectionState.Stopped)
            {
                InstanceFinder.ServerManager.Broadcast(new ChatMessage("   ", "玩家" + connection + "离开了游戏"));
            }
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

            ShowChatPanelCoroutine();
            GameObject newMessage = Instantiate(messagePrefab, content, false);
            Vector2 pos = newMessage.transform.localPosition;
            pos.y = -_totalLineCount * lineHeight;
            newMessage.transform.localPosition = pos;
            newMessage.transform.localScale = Vector3.zero;
            newMessage.transform.DOScale(1, 0.2f);
            _totalLineCount += newMessage.GetComponent<Message>().Init(chatMessage.Sender, chatMessage.Message) + 1;
            Vector2 size = content.sizeDelta;
            size.y = lineHeight * (_totalLineCount + 2);
            content.sizeDelta = size;
            if (size.y >= 700)
                content.localPosition = new Vector3(0, size.y - 700);
        }

        private void SendInputField()
        {
            SendChatMessage(PlayerPrefsMgr.PlayerName, messageInput.text);
            messageInput.text = null;
        }

        public void SendChatMessage(string sender, string text)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = sender,
                Message = text
            };
            if (InstanceFinder.IsServerStarted)
                InstanceFinder.ServerManager.Broadcast(chatMessage);
            else if (InstanceFinder.IsClientStarted)
                InstanceFinder.ClientManager.Broadcast(chatMessage);
        }

        private void OnInputSubmit(string message)
        {
            SendInputField();
            EventSystem.current.SetSelectedGameObject(messageInput.gameObject, null);
            messageInput.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public void Clear()
        {
            content.DestroyAllChildren();
            Vector3 size = content.sizeDelta;
            size.y = 700;
            content.sizeDelta = size;
            content.localPosition = new Vector3(0, 0);
            _totalLineCount = 0;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowChatPanelCoroutine();
        }

        private void ShowChatPanelCoroutine()
        {
            canvasGroup.DOKill(true);
            canvasGroup.DOFade(1f, 0.35f)
                .OnComplete(() => { DOVirtual.DelayedCall(5f, () => { canvasGroup.DOFade(0f, 0.35f); }); });
        }
    }
}