using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Room : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public readonly SyncVar<int> ID = new();

    public void Close()
    {
        Despawn();
    }
    
    // public void 
}