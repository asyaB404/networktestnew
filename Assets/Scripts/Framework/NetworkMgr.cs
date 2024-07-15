using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class NetworkMgr : MonoBehaviour
{
    public Tugboat tugboat;
    public NetworkManager networkManager;
    public LocalConnectionState clientState;
    public LocalConnectionState serverState;
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
        if (serverState != LocalConnectionState.Stopped)
            networkManager.ServerManager.StopConnection(true);
        else
            networkManager.ServerManager.StartConnection();
    }

    public void CreateOrCloseRoom(bool open)
    {
        if (networkManager == null)
            return;
        if (open)
            networkManager.ServerManager.StartConnection();
        else
            networkManager.ServerManager.StopConnection(true);
    }

    public void JoinOrExitRoom()
    {
        if (networkManager == null)
            return;
        if (clientState != LocalConnectionState.Stopped)
            networkManager.ClientManager.StopConnection();
        else
            networkManager.ClientManager.StartConnection();
    }

    public void JoinOrExitRoom(bool open)
    {
        if (networkManager == null)
            return;
        if (open)
            networkManager.ClientManager.StartConnection();
        else
            networkManager.ClientManager.StopConnection();
    }
}