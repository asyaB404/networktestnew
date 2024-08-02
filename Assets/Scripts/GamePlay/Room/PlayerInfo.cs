using FishNet.Connection;

namespace GamePlay.Room
{
    public struct PlayerInfo
    {
        /// <summary>
        /// 说是ID其实是连接服务器顺序的下标而已
        /// </summary>
        public int id;
        public string playerName;
        public NetworkConnection connection;
        

        public PlayerInfo(int id, string playerName, NetworkConnection connection)
        {
            this.id = id;
            this.playerName = playerName;
            this.connection = connection;
        }

        public override string ToString()
        {
            return id + "_" + playerName;
        }
    }
}