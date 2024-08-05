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
        ///     说是ID其实是连接服务器顺序的下标而已
        /// </summary>
        public int id;

        public string playerName;
        public NetworkConnection connection;
        public PlayerStatus status;


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