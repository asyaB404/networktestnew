using FishNet;
using UI.InfoPanel;
using UI.Panel;
using UnityEngine;

namespace UI.GamingUI
{
    public class MenuPanel : GameUI
    {
        [SerializeField] private PlayerInfoPanel playerInfoPanel;

        protected override void Awake()
        {
            base.Awake();
            buttons[0].onClick.AddListener(HideMe);
            buttons[1].onClick.AddListener(() => { });
            buttons[2].onClick.AddListener(() =>
            {
                HideMe();
                GamePanel.Instance.HideMe();
                if (InstanceFinder.IsServerStarted)
                    NetworkMgr.Instance.CloseRoom();
                NetworkMgr.Instance.ExitRoom();
            });
        }

        private void OnEnable()
        {
            playerInfoPanel.gameObject.SetActive(InstanceFinder.NetworkManager.IsServerStarted);
        }

        private void OnDisable()
        {
            playerInfoPanel.gameObject.SetActive(false);
        }
    }
}