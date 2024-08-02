using FishNet.Connection;

namespace GamePlay.Room
{
    public struct PlayerInfo
    {
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