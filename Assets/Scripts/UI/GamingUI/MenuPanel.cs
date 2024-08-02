
using FishNet;
using UI.InfoPanel;
using UI.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GamingUI
{
    public class MenuPanel : GameUI
    {
        private Button[] _buttons;
        [SerializeField] private PlayerInfoPanel playerInfoPanel;

        private void Awake()
        {
            _buttons = GetComponentsInChildren<Button>(true);
            _buttons[0].onClick.AddListener(HideMe);
            _buttons[1].onClick.AddListener(() => { });
            _buttons[2].onClick.AddListener(() =>
            {
                HideMe();
                GamePanel.Instance.HideMe();
                NetworkMgr.Instance.CloseRoom();
                NetworkMgr.Instance.ExitRoom();
            });
        }

        public override void ShowMe()
        {
            gameObject.SetActive(true);
            playerInfoPanel.gameObject.SetActive(InstanceFinder.NetworkManager.IsServerStarted);
        }

        public override void HideMe()
        {
            gameObject.SetActive(false);
            playerInfoPanel.gameObject.SetActive(false);
        }
    }
}