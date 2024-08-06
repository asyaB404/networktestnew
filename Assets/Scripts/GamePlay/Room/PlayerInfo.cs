using System.Runtime.CompilerServices;
using FishNet;
using FishNet.Connection;
using UnityEngine;

namespace GamePlay.Room
{
    /// <summary>
    ///     玩家在房间内的状态
    /// </summary>
    public enum PlayerStatus
    {
        Idle,
        Ready,
        Gaming,
        Watch
    }

    public struct PlayerInfo
    {
        /// <summary>
        ///     其实是连接服务器顺序,对应RoomMgr.PlayersCon列表的下标
        /// </summary>
        public int id;

        public string playerName;
        public NetworkConnection connection;
        public PlayerStatus status;

        /// <summary>
        /// return new(-1, PlayerPrefsMgr.PlayerName, InstanceFinder.ClientManager.Connection);
        /// </summary>
        public static PlayerInfo Default
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        } = new(-1, PlayerPrefsMgr.PlayerName, InstanceFinder.ClientManager.Connection);
        
        /// <summary>
        /// return new(-1, "", null);
        /// </summary>
        public static PlayerInfo Null
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        } = new(-1, "", null);


        public PlayerInfo(int id, string playerName, NetworkConnection connection,
            PlayerStatus status = PlayerStatus.Idle)
        {
            this.id = id;
            this.playerName = playerName;
            this.connection = connection;
            this.status = status;
        }

        public override string ToString()
        {
            return id + "_" + playerName;
        }
    }
}