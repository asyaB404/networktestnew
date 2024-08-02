using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UI.InfoPanel;
using UnityEngine;


namespace GamePlay.Room
{
    public enum RoomType
    {
        T1V1,
        T2V2,
        T4VS
    }

    public class RoomMgr : MonoBehaviour
    {
        public static RoomMgr Instance { get; private set; }
        public static int PlayerCount => InstanceFinder.ServerManager.Clients.Count;

        public int LastIndex
        {
            get
            {
                for (int i = 0; i < playersCon.Count; i++)
                {
                    if (playersCon[i] == null)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// 对外公开是方便为其CustomData赋值，原则上不允许从外部修改其元素
        /// </summary>
        public readonly List<NetworkConnection> playersCon = Enumerable.Repeat<NetworkConnection>(null, 4).ToList();

        /// <summary>
        /// 仅仅用来得到，PlayerInfo是结构体.对该容器内的元素的修改不起效果
        /// </summary>
        public List<PlayerInfo> PlayerInfos =>
            playersCon.Select(con =>
            {
                if (con != null && con.CustomData is PlayerInfo info)
                    return info;
                return new PlayerInfo(-1, "NULL", null);
            }).ToList();

        public RoomType CurType { get; private set; }
        public int watchersCount;

        public int MaxPlayerCount
        {
            get
            {
                int count = 0;
                if (CurType == RoomType.T1V1)
                {
                    count = 2;
                }
                else
                {
                    count = 4;
                }

                return count + watchersCount;
            }
        }

        public string RoomName { get; private set; }

        [ContextMenu("debuglist")]
        public void Test()
        {
            foreach (var player in playersCon)
            {
                Debug.Log(player);
                if (player != null)
                {
                    Debug.Log(player.CustomData);
                }
            }
        }

        [ContextMenu("debuglist1")]
        public void Test1()
        {
            foreach (var item in InstanceFinder.ServerManager.Clients)
            {
                Debug.Log(item.Key + "___" + item.Value);
            }
        }

        public void Create(RoomType roomType, string roomName)
        {
            CurType = roomType;
            NetworkMgr.Instance.tugboat.SetMaximumClients(MaxPlayerCount);
        }

        private void Awake()
        {
            Instance = this;
            Debug.Log("RoomManager正确初始化");
            NetworkManager networkManager = InstanceFinder.NetworkManager;
            networkManager.ServerManager.OnServerConnectionState += obj =>
            {
                LocalConnectionState state = obj.ConnectionState;
                if (state == LocalConnectionState.Stopped)
                {
                    for (int i = 0; i < playersCon.Count; i++)
                    {
                        playersCon[i] = null;
                    }
                }
            };
            networkManager.ServerManager.OnRemoteConnectionState += (connection, obj) =>
            {
                if (obj.ConnectionState == RemoteConnectionState.Started)
                {
                    Debug.Log("收到来自远端的连接" + connection + "\n目前有:" + PlayerCount);
                    playersCon[LastIndex] = connection;
                    connection.CustomData = "init";
                    // PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);  不在这里更新的原因是需要客户端的RPC为服务端赋值完才能更新
                }
                else if (obj.ConnectionState == RemoteConnectionState.Stopped)
                {
                    Debug.Log("来自远端的连接已断开" + connection + "\n目前有:" + (PlayerCount - 1));
                    int i = ((PlayerInfo)connection.CustomData).id;
                    playersCon[i] = null;
                    PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);
                }
            };
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}