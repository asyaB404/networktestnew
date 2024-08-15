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
                switch (RPCInstance.CurStatus)
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
            _btnText.text = InstanceFinder.NetworkManager.IsServerStarted ? "开始" : "准备";
            UpdateUI();
        }

        private void OnDisable()
        {
            menuPanel.gameObject.SetActive(false);
        }

        public override void Init()
        {
            base.Init();
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
                    RoomMgr.Instance.TryStartGame();
                }
                else if (InstanceFinder.IsClientStarted)
                {
                    switch (RPCInstance.CurStatus)
                    {
                        case PlayerStatus.Idle:
                            RPCInstance.Instance.ChangeStatusRequest(PlayerStatus.Ready);
                            break;
                        case PlayerStatus.Ready:
                            RPCInstance.Instance.ChangeStatusRequest(PlayerStatus.Idle);
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

        public void UpdateUI()
        {
            GameObject btn = GetControl<Button>("readyOrStart").gameObject;
            if (RPCInstance.CurStatus == PlayerStatus.Gaming || RPCInstance.CurStatus == PlayerStatus.Watch)
            {
                btn.SetActive(false);
            }
            else
            {
                btn.SetActive(true);
            }
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

        // private void OnClientConnection(ClientConnectionStateArgs obj)
        // {
        //     switch (obj.ConnectionState)
        //     {
        //         case LocalConnectionState.Started:
        //             _btnText.text = InstanceFinder.NetworkManager.IsServerStarted ? "开始" : "准备";
        //             break;
        //         case LocalConnectionState.Stopped:
        //             HideMe();
        //             break;
        //         case LocalConnectionState.Starting:
        //             break;
        //         case LocalConnectionState.Stopping:
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        // }
    }
}