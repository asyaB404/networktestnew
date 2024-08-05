using DG.Tweening;
using FishNet.Transporting;
using UI.GamingUI;
using UI.InfoPanel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    ///     游戏开始时将会打开的Panel
    /// </summary>
    public class GamePanel : BasePanel<GamePanel>
    {
        [SerializeField] private MenuPanel menuPanel;
        private int _playerCount;

        private void OnEnable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnUpdateJoin;
        }

        private void OnDisable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnUpdateJoin;
            menuPanel.gameObject.SetActive(false);
        }

        public override void Init()
        {
            base.Init();
            GetComponentInChildren<PlayerInfoPanel>(true).Init();
            GetControl<Button>("menu").onClick.AddListener(() => { menuPanel.ChangeMe(); });
        }


        public override void OnPressedEsc()
        {
            menuPanel.ChangeMe();
        }

        private void OnUpdateJoin(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started) Debug.Log("生成一个玩家");

            if (obj.ConnectionState == LocalConnectionState.Stopped) HideMe();
        }

        public override void CallBack(bool flag)
        {
            transform.DOKill(true);
            if (flag)
            {
                CanvasGroupInstance.interactable = true;
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                transform.DOScale(1, UIConst.UIDuration);
            }
            else
            {
                CanvasGroupInstance.interactable = false;
                transform.DOScale(0, UIConst.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
            }
        }
    }
}