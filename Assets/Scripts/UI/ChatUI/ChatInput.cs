using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ChatUI
{
    public class ChatInput : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        private ChatPanel _chatPanel;
        public bool IsSelected { get; private set; }

        private void Awake()
        {
            _chatPanel = GetComponentInParent<ChatPanel>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            _chatPanel.ShowChatPanelCoroutine();
            IsSelected = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _chatPanel.HideChatPanelCoroutine();
            IsSelected = false;
        }
    }
}