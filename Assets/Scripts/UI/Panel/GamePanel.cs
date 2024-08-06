using System;
using DG.Tweening;
using FishNet;
using FishNet.Transporting;
using GamePlay.Room;
using TMPro;
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
        private float _btnCdTimer;
        private Image _btnMask;
        private TextMeshProUGUI _btnText;

        private void Update()
        {
            if (_btnCdTimer >= 0)
            {
                _btnCdTimer -= Time.deltaTime;
                switch (RPCInstance.Status)
                {
                    case PlayerStatus.Idle:
                        _btnMask.fillAmount = _btnCdTimer / UIConst.BtnClickCoolDown;
                        break;
                    case PlayerStatus.Ready:
                        _btnMask.fillAmount = (UIConst.BtnClickCoolDown - _btnCdTimer) / UIConst.BtnClickCoolDown;
                        break;
                    case PlayerStatus.Gaming:
                    case PlayerStatus.Watch:
                    default:
                        break;
                }
            }
        }

        private void OnEnable()
        {
            if (InstanceFinder.NetworkManager.IsServerStarted)
            {
                _btnText.text = "开始";
            }
            else if (InstanceFinder.NetworkManager.IsClientStarted)
            {
                _btnText.text = "准备";
            }
        }

        private void OnDisable()
        {
            menuPanel.gameObject.SetActive(false);
        }

        public override void Init()
        {
            base.Init();
            InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState += OnUpdateJoin;
            GetComponentInChildren<PlayerInfoPanel>(true).Init();
            _btnText = GetControl<TextMeshProUGUI>("btnText");
            _btnMask = GetControl<Image>("btnMask");
            GetControl<Button>("menu").onClick.AddListener(() => { menuPanel.ChangeMe(); });
            GetControl<Button>("readyOrStart").onClick.AddListener(() =>
            {
                if (_btnCdTimer > 0)
                    return;
                if (InstanceFinder.IsServerStarted)
                {
                }
                else if (InstanceFinder.IsClientStarted)
                {
                    switch (RPCInstance.Status)
                    {
                        case PlayerStatus.Idle:
                            RPCInstance.Instance.ChangeStatus(PlayerStatus.Ready);
                            break;
                        case PlayerStatus.Ready:
                            RPCInstance.Instance.ChangeStatus(PlayerStatus.Idle);
                            break;
                        case PlayerStatus.Gaming:
                            break;
                        case PlayerStatus.Watch:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    Debug.LogWarning("请先建立连接");
                }

                _btnCdTimer = UIConst.BtnClickCoolDown;
            });
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