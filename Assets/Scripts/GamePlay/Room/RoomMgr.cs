using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
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

    /// <summary>
    /// 作为服务端时生效，管理玩家连接的顺序以及主机房间的基本配置
    /// </summary>
    public class RoomMgr : MonoBehaviour
    {
        /// <summary>
        /// 最大连接数
        /// </summary>
        public const int MaxCons = 16;

        /// <summary>
        /// 旁观玩家数
        /// </summary>
        public int watchersCount;

        /// <summary>
        /// 连接列表，用来管理玩家的连接顺序
        /// </summary>
        private readonly List<NetworkConnection> _playersCon =
            Enumerable.Repeat<NetworkConnection>(null, MaxCons).ToList();

        /// <summary>
        ///     玩家连接管理，玩家连接时优先占用下标比较小的位置
        /// </summary>
        /// <example>例如[c0,null,c2,null,...]时有玩家连接则会变成[c0,c1,c2,null,...]</example>
        public IReadOnlyList<NetworkConnection> PlayersCon => _playersCon;

        public static RoomMgr Instance { get; private set; }

        public static int PlayerCount => InstanceFinder.ServerManager.Clients.Count;

        /// <summary>
        /// 遍历连接列表得到第一个空位置的下标
        /// </summary>
        public int FirstIndex
        {
            get
            {
                for (var i = 0; i < PlayersCon.Count; i++)
                    if (PlayersCon[i] == null)
                        return i;

                return -1;
            }
        }

        /// <summary>
        ///     仅仅用来得到连接的附带信息,但返回的是Linq表达式,对其修改不会影响_playersCon
        /// </summary>
        public List<PlayerInfo> PlayerInfos =>
            PlayersCon.Select(con =>
            {
                if (con != null && con.CustomData is PlayerInfo info)
                    return info;
                return new PlayerInfo(-1, "NULL", null);
            }).ToList();

        /// <summary>
        /// 当前房间类型
        /// </summary>
        public RoomType CurType { get; private set; }

        /// <summary>
        /// 最大玩家数
        /// </summary>
        public int MaxPlayerCount
        {
            get
            {
                var count = 0;
                switch (CurType)
                {
                    case RoomType.T1V1:
                        count = 2;
                        break;
                    case RoomType.T2V2:
                    case RoomType.T4VS:
                        count = 4;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return count + watchersCount;
            }
        }

        public string RoomName { get; private set; }

        private void Awake()
        {
            Instance = this;
            Debug.Log("RoomManager正确初始化");
            var networkManager = InstanceFinder.NetworkManager;
            networkManager.ServerManager.OnServerConnectionState += obj =>
            {
                var state = obj.ConnectionState;
                if (state == LocalConnectionState.Stopped)
                    for (var i = 0; i < PlayersCon.Count; i++)
                        _playersCon[i] = null;
            };
            networkManager.ServerManager.OnRemoteConnectionState += (connection, obj) =>
            {
                if (obj.ConnectionState == RemoteConnectionState.Started)
                {
                    Debug.Log("收到来自远端的连接" + connection + "\n目前有:" + PlayerCount);
                    _playersCon[FirstIndex] = connection;
                    connection.CustomData = "init";
                    // PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);  不在这里更新的原因是需要客户端的RPC为服务端赋值完才能更新
                }
                else if (obj.ConnectionState == RemoteConnectionState.Stopped)
                {
                    Debug.Log("来自远端的连接已断开" + connection + "\n目前有:" + (PlayerCount - 1));
                    var i = ((PlayerInfo)connection.CustomData).id;
                    connection.CustomData = null;
                    _playersCon[i] = null;
                    PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);
                }
            };
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }


        public void SetRoomConfig(RoomType roomType, string roomName)
        {
            CurType = roomType;
            RoomName = roomName;
            NetworkMgr.Instance.tugboat.SetMaximumClients(MaxPlayerCount);
        }

        [ContextMenu("debuglist")]
        public void Test()
        {
            foreach (var player in PlayersCon)
            {
                Debug.Log(player);
                if (player != null) Debug.Log(player.CustomData);
            }
        }

        [ContextMenu("debuglist1")]
        public void Test1()
        {
            foreach (var item in InstanceFinder.ServerManager.Clients) Debug.Log(item.Key + "___" + item.Value);
        }
    }
}