using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Room : NetworkBehaviour
{
    public readonly SyncVar<int> ID = new();

    public void Close()
    {
        Despawn();
    }
}