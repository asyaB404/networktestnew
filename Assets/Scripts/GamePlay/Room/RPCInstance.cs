using System;
using System.Collections.Generic;
using FishNet.Object;
using GamePlay.Coins;
using UI.InfoPanel;
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
                if (Instance != null)
                {
                    Debug.Log("多余的RPC已经被移除");
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                }

                SendPlayerInfo(PlayerInfo.Default);
                UpdateHostPlayerInfosPanel();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        [ObserversRpc]
        private void Init(int id)
        {
            if (base.IsOwner)
            {
                ID = id;
            }
        }

        /// <summary>
        /// 客户端建立连接时调用，向服务端发送玩家信息
        /// </summary>
        /// <param name="defaultInfo"></param>
        [ServerRpc]
        public void SendPlayerInfo(PlayerInfo defaultInfo)
        {
            var i = 0;
            foreach (var con in RoomMgr.Instance.PlayersCon)
            {
                if (con.CustomData is "init") //con.CustomData is string s && s == "init"
                {
                    Init(i);
                    defaultInfo.id = i;
                    con.CustomData = defaultInfo;
                    break;
                }

                i++;
            }
        }

        [Client]
        public void ChangeStatus(PlayerStatus status)
        {
            CurStatus = status;
            SyncStatus(ID, status);
        }

        [ObserversRpc]
        public void ChangeStatusFromServer(PlayerStatus status)
        {
            CurStatus = status;
        }

        /// <summary>
        /// 同步服务端玩家准备状态
        /// </summary>
        [ServerRpc]
        private void SyncStatus(int id, PlayerStatus status)
        {
            if (RoomMgr.Instance.PlayersCon[id].CustomData is not PlayerInfo) return;
            PlayerInfo info = PlayerInfo.Default;
            info.id = id;
            info.status = status;
            RoomMgr.Instance.PlayersCon[id].CustomData = info;
            UpdateIsReady(id, status);
        }

        /// <summary>
        /// 向所有客户端发送更新玩家准备状态的请求
        /// </summary>
        [ObserversRpc]
        private void UpdateIsReady(int id, PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Idle:
                    GameManager.Instance.SetReady(id, false);
                    break;
                case PlayerStatus.Ready:
                    GameManager.Instance.SetReady(id, true);
                    break;
                case PlayerStatus.Gaming:
                    break;
                case PlayerStatus.Watch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
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

        // [ServerRpc]
        // public void SyncCoinsPoolsRequest()
        // {
        // }
        //
        // [ObserversRpc]
        // public void SyncCoinsPoolsToClient(List<CoinsPool> coinsPools)
        // {
        //     if (base.IsOwner)
        //     {
        //         GameManager.Instance.coinsPools = coinsPools;
        //     }
        // }

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