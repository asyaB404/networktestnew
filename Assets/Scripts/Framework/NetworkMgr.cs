using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class NetworkMgr : MonoBehaviour
{
    public Tugboat tugboat;
    public NetworkManager networkManager;
    private LocalConnectionState _clientState;
    private LocalConnectionState _serverState;
    public static NetworkMgr Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        tugboat = GetComponent<Tugboat>();
        networkManager = GetComponent<NetworkManager>();
    }

    public void CreateOrCloseRoom()
    {
        if (networkManager == null)
            return;
        if (_serverState != LocalConnectionState.Stopped)
            networkManager.ServerManager.StopConnection(true);
        else
            networkManager.ServerManager.StartConnection();
    }

    public void JoinOrExitRoom()
    {
        if (networkManager == null)
            return;
        if (_clientState != LocalConnectionState.Stopped)
            networkManager.ClientManager.StopConnection();
        else
            networkManager.ClientManager.StartConnection();
    }
}