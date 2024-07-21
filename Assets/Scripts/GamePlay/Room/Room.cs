using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public enum RoomType
{
    T1V1,
    T2V2,
    T4VS
}

namespace GamePlay.Room
{
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
}