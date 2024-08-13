using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Example.Authenticating;
using FishNet.Transporting;
using UI.InfoPanel;
using UnityEngine;
using UnityEngine.Serialization;

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
        public PasswordAuthenticator authenticator;

        /// <summary>
        /// 旁观玩家数
        /// </summary>
        [FormerlySerializedAs("watchersCount")]
        public int MaxWatchersCount;

        /// <summary>
        /// 连接列表的大小
        /// </summary>
        public const int MaxConsListSize = 8;

        /// <summary>
        /// 连接列表，用来管理玩家的连接顺序
        /// </summary>
        private readonly List<NetworkConnection> _playersCon =
            Enumerable.Repeat<NetworkConnection>(null, MaxConsListSize).ToList();

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
                return PlayerInfo.Null;
            }).ToList();

        /// <summary>
        /// 当前房间类型
        /// </summary>
        public RoomType CurRoomType { get; private set; }

        /// <summary>
        /// 最大玩家数
        /// </summary>
        public int MaxPlayerCount
        {
            get
            {
                var count = CurRoomType switch
                {
                    RoomType.T1V1 => 2,
                    RoomType.T2V2 or RoomType.T4VS => 4,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return count;
            }
        }

        public string RoomName { get; private set; }

        private void Awake()
        {
            Instance = this;
            var networkManager = InstanceFinder.NetworkManager;
            networkManager.ServerManager.SetAuthenticator(authenticator);
            networkManager.ServerManager.OnServerConnectionState += obj =>
            {
                var state = obj.ConnectionState;
                switch (state)
                {
                    case LocalConnectionState.Stopped:
                        for (var i = 0; i < PlayersCon.Count; i++)
                            _playersCon[i] = null;
                        break;
                    case LocalConnectionState.Starting:
                        break;
                    case LocalConnectionState.Started:
                        break;
                    case LocalConnectionState.Stopping:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            networkManager.ServerManager.OnRemoteConnectionState += (connection, obj) =>
            {
                switch (obj.ConnectionState)
                {
                    case RemoteConnectionState.Started:
                        Debug.Log("收到来自远端的认证连接" + connection);
                        break;
                    case RemoteConnectionState.Stopped:
                    {
                        if (connection.IsAuthenticated)
                        {
                            Debug.Log("来自远端的连接已断开" + connection + "\n目前有:" + (PlayerCount - 1));
                            var i = ((PlayerInfo)connection.CustomData).id;
                            _playersCon[i] = null;
                            GameManager.Instance.SetReadySprite(i, false);
                            PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);
                        }
                        else
                        {
                            Debug.Log("来自远端的认证连接已断开" + connection + "\n目前有:" + (PlayerCount - 1));
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            networkManager.ServerManager.OnAuthenticationResult += (connection, flag) =>
            {
                if (flag)
                {
                    Debug.Log(connection + "通过认证");
                    _playersCon[FirstIndex] = connection;
                    //给这个连接标记需要初始化
                    connection.CustomData = "init";
                    // PlayerInfoPanel.Instance.UpdateInfoPanel(PlayerInfos);  不在这里更新的原因是需要客户端的RPC为服务端赋值完才能更新
                }
                else
                {
                    Debug.Log(connection + "认证失败");
                }
            };
            networkManager.ClientManager.OnClientConnectionState += obj =>
            {
                switch (obj.ConnectionState)
                {
                    case LocalConnectionState.Stopped:
                        Debug.Log("客户端关闭");
                        GameManager.Instance.gameObject.SetActive(false);
                        break;
                    case LocalConnectionState.Starting:
                        break;
                    case LocalConnectionState.Started:
                        Debug.Log("客户端启动成功");
                        break;
                    case LocalConnectionState.Stopping:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
            networkManager.ClientManager.OnAuthenticated += () =>
            {
                GameManager.Instance.gameObject.SetActive(true);
                Debug.Log("客户端通过验证");
            };
        }

        public void SetRoomConfig(RoomType roomType, string roomName)
        {
            CurRoomType = roomType;
            RoomName = roomName;
            NetworkMgr.Instance.tugboat.SetMaximumClients(MaxPlayerCount + MaxWatchersCount);
        }

        public bool TryStartGame()
        {
            IList<PlayerInfo> playerInfos = PlayerInfos;
            int idleCount = 0;
            int readyCount = 0;
            int watcherCount = 0;
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                //对于房主来说 idle状态也是准备状态，除非房主观战
                if (i == 0)
                {
                    if (playerInfos[0].status == PlayerStatus.Watch)
                        watcherCount = 1;
                    else
                        readyCount = 1;
                    continue;
                }

                //when i>0
                if (playerInfos[i] == PlayerInfo.Null)
                    continue;
                switch (playerInfos[i].status)
                {
                    case PlayerStatus.Ready:
                        readyCount++;
                        break;
                    case PlayerStatus.Watch:
                        watcherCount++;
                        break;
                    case PlayerStatus.Idle:
                        idleCount++;
                        break;
                    case PlayerStatus.Gaming:
                        Debug.LogWarning("怎么还有游戏中的状态？");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            switch (CurRoomType)
            {
                case RoomType.T1V1:
                case RoomType.T2V2:
                    if (readyCount < MaxPlayerCount)
                        return false;
                    break;
                case RoomType.T4VS:
                    if (readyCount < 2 && idleCount > 0)
                        return false;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log(idleCount + "_" + readyCount + "_" + watcherCount);

            //同步服务端
            for (int i = 0; i < PlayersCon.Count; i++)
            {
                if (PlayersCon[i] == null || playerInfos[i].status == PlayerStatus.Watch)
                {
                    continue;
                }

                var playerInfo = PlayerInfos[i];
                playerInfo.status = PlayerStatus.Gaming;
                PlayersCon[i].CustomData = playerInfo;
            }

            //同步客户端
            RPCInstance.Instance.SetAllStatusExceptWatcher(PlayerStatus.Gaming);
            RPCInstance.Instance.UpdateGamingUI();
            
            GameManager.Instance.StartGame();
            return true;
        }

        #region #DebugFunction

        [ContextMenu("debuglist")]
        private void Test()
        {
            foreach (var player in PlayersCon)
            {
                Debug.Log(player);
                if (player != null) Debug.Log(player.CustomData);
            }
        }

        [ContextMenu("debuglist1")]
        private void Test1()
        {
            foreach (var item in InstanceFinder.ServerManager.Clients) Debug.Log(item.Key + "___" + item.Value);
        }

        [ContextMenu("dedbug2")]
        private void Test2()
        {
            Debug.Log(InstanceFinder.IsServerStarted);
            Debug.Log(InstanceFinder.IsClientStarted);
            Debug.Log(InstanceFinder.IsClientOnlyStarted);
        }

        [ContextMenu("debug3")]
        private void Test3()
        {
            NetworkMgr.Instance.tugboat.CanBeConnected = !NetworkMgr.Instance.tugboat.CanBeConnected;
            Debug.Log(NetworkMgr.Instance.tugboat.CanBeConnected);
        }

        #endregion
    }
}