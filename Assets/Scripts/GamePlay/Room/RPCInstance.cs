using System;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UI.InfoPanel;
using UnityEngine;

namespace GamePlay.Room
{
    /// <summary>
    /// 每一个客户端仅有一个的RPC实例,与服务端建立连接之前之前Instance为null
    /// </summary>
    public class RPCInstance : NetworkBehaviour
    {
        public static RPCInstance Instance { get; private set; }

        /// <summary>
        /// 客户端断开连接时不需要reset,因为客户端断开连接后会直接销毁RPC实例,服务端状态同理
        /// </summary>
        public static PlayerStatus Status { get; private set; } = PlayerStatus.Idle;

        /// <summary>
        /// 在主机的_playersCon列表的下标
        /// </summary>
        public static int ID { get; private set; } = -1;

        [ContextMenu("test")]
        private void Test()
        {
            Debug.Log(ID);
        }

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
            Status = status;
            SyncStatus(ID, status);
        }

        [ObserversRpc]
        public void ChangeStatusFromServer(PlayerStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// 同步服务端玩家准备状态
        /// </summary>
        [ServerRpc]
        private static void SyncStatus(int id, PlayerStatus status)
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
        private static void UpdateIsReady(int id, PlayerStatus status)
        {
            switch (status)
            {
                case PlayerStatus.Idle:
                    GameManager.Instance.SetPlayerReady(id, false);
                    break;
                case PlayerStatus.Ready:
                    GameManager.Instance.SetPlayerReady(id, true);
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

        #region # Debug

        [ContextMenu("test")]
        private void Test1()
        {
            Debug.Log(ID + " _ " + Status);
        }

        #endregion
    }
}