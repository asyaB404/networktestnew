using System;
using System.Collections.Generic;
using ChatUI;
using FishNet.Connection;
using FishNet.Object;
using UI.InfoPanel;
using UI.Panel;
using UnityEngine;

namespace GamePlay.Room
{
    /// <summary>
    /// 它能被生成也意味着客户端通过了验证。
    /// 每一个客户端仅有一个的RPC实例,与服务端建立连接之前之前Instance为null
    /// </summary>
    public class RPCInstance : NetworkBehaviour
    {
        /// <summary>
        /// 每个客户端的Instance都指向自己的RPCInstance
        /// </summary>
        public static RPCInstance Instance { get; private set; }

        /// <summary>
        /// 客户端断开连接时不需要reset,因为客户端断开连接后会直接销毁RPC实例,服务端状态同理
        /// </summary>
        public static PlayerStatus CurStatus { get; private set; } = PlayerStatus.Idle;

        /// <summary>
        /// 在主机的_playersCon列表的下标
        /// </summary>
        public static int ID { get; private set; } = -1;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                Debug.Log("OwnerRPC");
                ChatPanel.Instance.SendChatMessage("   ", "玩家" + PlayerPrefsMgr.PlayerName + "加入了游戏");
                if (Instance != null)
                {
                    Debug.Log("多余的RPC已经被移除");
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                }

                InitServerPlayerInfo(PlayerInfo.Default);
                UpdateHostPlayerInfosPanel();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (IsOwner)
            {
                GamePanel.Instance.HideMe();
                CurStatus = PlayerStatus.Idle;
            }
        }

        [TargetRpc]
        private void Init(NetworkConnection con, int id)
        {
            if (base.IsOwner)
            {
                ID = id;
            }
        }

        /// <summary>
        /// 客户端建立连接时调用，向服务端发送初始化玩家信息
        /// </summary>
        /// <param name="defaultInfo"></param>
        /// <param name="target"></param>
        [ServerRpc]
        public void InitServerPlayerInfo(PlayerInfo defaultInfo, NetworkConnection target = null)
        {
            var i = 0;
            foreach (var con in RoomMgr.Instance.PlayersCon)
            {
                if (con.CustomData is "init") //con.CustomData is string s && s == "init"
                {
                    Init(target, i);
                    defaultInfo.id = i;
                    con.CustomData = defaultInfo;
                    break;
                }

                i++;
            }
        }

        /// <summary>
        /// 改变客户端本地的status的同时向服务端同步自己的status
        /// </summary>
        /// <param name="newStatus"></param>
        public void ChangeStatusRequest(PlayerStatus newStatus)
        {
            CurStatus = newStatus;
            SyncStatus(ID, newStatus);
        }

        /// <summary>
        /// 同步所有客户端的status,除了旁观者
        /// </summary>
        /// <param name="newStatus"></param>
        [ObserversRpc]
        public void SetAllStatusExceptWatcher(PlayerStatus newStatus)
        {
            if (CurStatus == PlayerStatus.Watch)
                return;
            CurStatus = newStatus;
        }

        /// <summary>
        /// 调用一次所有客户端的UpdateGamingUI
        /// </summary>
        [ObserversRpc]
        public void UpdateGamingUI()
        {
            GamePanel.Instance.UpdateUI();
        }

        /// <summary>
        /// 同步服务端玩家准备状态
        /// </summary>
        [ServerRpc]
        public void SyncStatus(int id, PlayerStatus newStatus)
        {
            if (RoomMgr.Instance.PlayersCon[id].CustomData is not PlayerInfo) return;
            PlayerInfo info = (PlayerInfo)RoomMgr.Instance.PlayersCon[id].CustomData;
            info.status = newStatus;
            RoomMgr.Instance.PlayersCon[id].CustomData = info;
            switch (newStatus)
            {
                case PlayerStatus.Idle:
                    GameManager.Instance.SetReadySprite(id, false);
                    break;
                case PlayerStatus.Ready:
                    GameManager.Instance.SetReadySprite(id, true);
                    break;
                case PlayerStatus.Gaming:
                    GameManager.Instance.SetReadySprite(id, false);
                    break;
                case PlayerStatus.Watch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newStatus), newStatus, null);
            }
        }

        /// <summary>
        /// 只更新主机的玩家显示板
        /// </summary>
        [ServerRpc]
        public void UpdateHostPlayerInfosPanel()
        {
            PlayerInfoPanel.Instance.UpdateInfoPanel(RoomMgr.Instance.PlayerInfos);
        }

        [ServerRpc]
        public void SetOwnerShip(NetworkBehaviour networkBehaviour, NetworkConnection owner = null)
        {
            networkBehaviour.GiveOwnership(owner);
        }

        #region # Debug

        [ContextMenu("test")]
        private void Print()
        {
            Debug.Log(ID + " _ " + CurStatus);
        }

        [SerializeField, ContextMenuItem("test2", nameof(SetStatus))]
        private PlayerStatus status;

        private void SetStatus()
        {
            CurStatus = status;
            Print();
        }

        #endregion
    }
}