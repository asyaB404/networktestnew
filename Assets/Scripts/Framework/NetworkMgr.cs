using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using GamePlay.Room;
using UnityEngine;


public class NetworkMgr : MonoBehaviour
{
    public Tugboat tugboat;
    public NetworkManager networkManager;
    public LocalConnectionState ClientState { get; private set; }
    public LocalConnectionState ServerState { get; private set; }
    public static NetworkMgr Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        networkManager.ServerManager.OnServerConnectionState += OnServerConnection;
        networkManager.ClientManager.OnClientConnectionState += OnClientConnection;
    }

    public bool CreateOrCloseRoom()
    {
        if (networkManager == null)
            return false;
        return ServerState != LocalConnectionState.Stopped
            ? networkManager.ServerManager.StopConnection(true)
            : networkManager.ServerManager.StartConnection();
    }

    public bool CloseRoom()
    {
        if (networkManager == null)
            return false;
        return networkManager.ServerManager.StopConnection(true);
    }

    public bool CreateRoom(RoomType roomType, string roomName)
    {
        if (networkManager == null)
            return false;
        bool flag = networkManager.ServerManager.StartConnection();
        if (flag)
        {
            RoomMgr.Instance.Create(roomType, roomName);
        }

        return flag;
    }

    public bool JoinRoom()
    {
        if (networkManager == null)
            return false;
        return ClientState != LocalConnectionState.Stopped
            ? networkManager.ClientManager.StopConnection()
            : networkManager.ClientManager.StartConnection();
    }

    public bool JoinOrExitRoom(bool open)
    {
        if (networkManager == null)
            return false;
        return open
            ? networkManager.ClientManager.StartConnection()
            : networkManager.ClientManager.StopConnection();
    }

    private void OnServerConnection(ServerConnectionStateArgs obj)
    {
        ServerState = obj.ConnectionState;
    }

    private void OnClientConnection(ClientConnectionStateArgs obj)
    {
        ClientState = obj.ConnectionState;
    }
}