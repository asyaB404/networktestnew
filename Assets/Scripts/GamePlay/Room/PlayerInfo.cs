using System;
using System.Runtime.CompilerServices;
using FishNet;
using FishNet.Connection;

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
        public static PlayerInfo Default => new(-1, PlayerPrefsMgr.PlayerName, InstanceFinder.ClientManager.Connection);

        /// <summary>
        /// return new(-127, "", null);
        /// </summary>
        public static PlayerInfo Null
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        } = new(-127, "", null);


        public PlayerInfo(int id, string playerName, NetworkConnection connection,
            PlayerStatus status = PlayerStatus.Idle)
        {
            this.id = id;
            this.playerName = playerName;
            this.connection = connection;
            this.status = status;
        }

        public bool Equals(PlayerInfo other)
        {
            return id == other.id && playerName == other.playerName && Equals(connection, other.connection) &&
                   status == other.status;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, playerName, connection, (int)status);
        }

        public static bool operator ==(PlayerInfo left, PlayerInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerInfo left, PlayerInfo right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return id + "_" + status + "_" + playerName + "_" + connection;
        }
    }
}