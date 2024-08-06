using FishNet;
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

        public static PlayerStatus Status { get; set; }

        /// <summary>
        /// 在主机的_playersCon列表的下标
        /// </summary>
        public static int ID { get; private set; } = -1;

        public static PlayerInfo DefaultInfo =>
            new PlayerInfo(-1, PlayerPrefsMgr.PlayerName, InstanceFinder.ClientManager.Connection);

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

                SendPlayerInfo(DefaultInfo);
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

        [ServerRpc]
        public void SendPlayerInfo(PlayerInfo defaultInfo)
        {
            var i = 0;
            foreach (var con in RoomMgr.Instance.PlayersCon)
            {
                if (con.CustomData is string s && s == "init")
                {
                    Init(i);
                    defaultInfo.id = i;
                    con.CustomData = defaultInfo;
                    break;
                }

                i++;
            }
        }

        [ServerRpc]
        public void ChangeStatus(int id, PlayerStatus status)
        {
            if (RoomMgr.Instance.PlayersCon[id].CustomData is PlayerInfo)
            {
                PlayerInfo info = DefaultInfo;
                info.id = id;
                info.status = status;
                RoomMgr.Instance.PlayersCon[id].CustomData = info;
            }
        }

        [ObserversRpc]
        private void UpdateStatusUI()
        {
            
        }

        /// <summary>
        /// 只更新主机的玩家显示板
        /// </summary>
        [ServerRpc]
        public void UpdateHostPlayerInfosPanel()
        {
            PlayerInfoPanel.Instance.UpdateInfoPanel(RoomMgr.Instance.PlayerInfos);
        }
    }
}