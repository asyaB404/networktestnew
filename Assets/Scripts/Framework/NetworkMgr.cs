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

    [SerializeField] private GameObject roomMgrPrefab;

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
        return networkManager.ClientManager.StartConnection();
    }

    public bool ExitRoom()
    {
        if (networkManager == null)
            return false;
        return networkManager.ClientManager.StopConnection();
    }

    public bool JoinOrExitRoom()
    {
        if (networkManager == null)
            return false;
        return ClientState != LocalConnectionState.Stopped
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
        // if (obj.ConnectionState == LocalConnectionState.Started)
        // {
        //     GameObject gobj = Instantiate(roomMgrPrefab);
        //     networkManager.ServerManager.Spawn(gobj);
        // }
        // else if (obj.ConnectionState == LocalConnectionState.Stopped)
        // {
        //     networkManager.ServerManager.Despawn(RoomMgr.Instance.gameObject);
        // }
    }
}