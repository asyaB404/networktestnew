using DG.Tweening;
using FishNet.Transporting;
using GamePlay.Room;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    /// 游戏开始时将会打开的Panel
    /// </summary>
    public class GamePanel : BasePanel<GamePanel>
    {
        private int _playerCount;
        [SerializeField] private GameObject menuPanel;

        public override void Init()
        {
            base.Init();
            GetControl<Button>("menu").onClick.AddListener(() =>
            {
                HideMe(false);
                // NetworkMgr.Instance.CloseRoom();
                // NetworkMgr.Instance.ExitRoom();
            });
        }

        public override void HideMe(bool isPressedEsc = false)
        {
            base.HideMe(isPressedEsc);
        }

        private void OnEnable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnUpdateJoin;
        }

        private void OnDisable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnUpdateJoin;
        }

        private void OnUpdateJoin(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                Debug.Log("生成一个玩家");
            }

            if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                HideMe();
            }
        }
        public override void CallBack(bool flag)
        {
            transform.DOKill(true);
            if (flag)
            {
                MyCanvasGroup.interactable = true;
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                transform.DOScale(1, UIConst.UIDuration);
            }
            else
            {
                MyCanvasGroup.interactable = false;
                transform.DOScale(0, UIConst.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
            }
        }
    }
}