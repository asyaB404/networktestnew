using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;


namespace GamePlay.Room
{
    public enum RoomType
    {
        T1V1,
        T2V2,
        T4VS
    }

    public struct PlayerInfo
    {
        public int id;
        public string playerName;

        public PlayerInfo(int id, string playerName)
        {
            this.id = id;
            this.playerName = playerName;
        }

        public override string ToString()
        {
            return id+"_"+playerName;
        }
    }

    public class RoomMgr : MonoBehaviour
    {
        public static RoomMgr Instance { get; private set; }
        public static int PlayerCount => InstanceFinder.ServerManager.Clients.Count;
        private readonly List<NetworkConnection> _players = Enumerable.Repeat<NetworkConnection>(null, 4).ToList();
        public IReadOnlyList<NetworkConnection> Players => _players;
        public RoomType CurType { get; private set; }

        public int MaxPlayerCount
        {
            get
            {
                if (CurType == RoomType.T1V1)
                {
                    return 2;
                }
                else
                {
                    return 4;
                }
            }
        }

        public string RoomName { get; private set; }

        [ContextMenu("debuglist")]
        public void Test()
        {
            foreach (var player in _players)
            {
                Debug.Log(player);
                if (player != null)
                {
                    Debug.Log(player.CustomData);
                }
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
            networkManager.ServerManager.OnRemoteConnectionState += (connection, obj) =>
            {
                if (obj.ConnectionState == RemoteConnectionState.Started)
                {
                    Debug.Log("收到来自远端的连接" + connection + "\n目前有:" + PlayerCount);
                    _players[PlayerCount - 1] = connection;
                }
                else if (obj.ConnectionState == RemoteConnectionState.Stopped)
                {
                    Debug.Log("来自远端的连接已断开" + connection + "\n目前有:" + (PlayerCount - 1));
                    _players[PlayerCount - 1] = null;
                }
            };
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}