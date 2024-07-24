using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;


namespace GamePlay.Room
{
    public enum RoomType
    {
        T1V1,
        T2V2,
        T4VS
    }


    public class RoomMgr : NetworkBehaviour
    {
        public GameObject roomPrefab;
        public static RoomMgr Instance { get; private set; }

        [SerializeField]private int playerCount;

        public int PlayerCount
        {
            get => playerCount;
            private set => playerCount = value;
        }


        [ContextMenu("test")]
        private void Print()
        {
            Debug.Log(PlayerCount);
            Debug.Log(IsSpawned);
        }

        [ContextMenu("test1")]
        private void test1()
        {
            NetworkMgr.Instance.networkManager.ServerManager.Spawn(gameObject);
        }

        public RoomType CurType { get; private set; }

        public int MaxPlayerCount
        {
            get
            {
                if (CurType == RoomType.T1V1)
                {
                    return 2;
                }
                else
                {
                    return 4;
                }
            }
        }

        public string RoomName { get; private set; }


        public void Create(RoomType roomType, string roomName)
        {
            CurType = roomType;
            NetworkMgr.Instance.tugboat.SetMaximumClients(MaxPlayerCount);
        }


        [ServerRpc(RequireOwnership = false)]
        public void Join()
        {
            PlayerCount += 1;
            JoinRpc();
        }

        [ObserversRpc]
        private void JoinRpc()
        {
            
        }

        [ServerRpc(RequireOwnership = false)]
        public void Exit()
        {
            PlayerCount -= 1;
            ExitRpc();
        }

        [ObserversRpc]
        private void ExitRpc()
        {
            
        }


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
        }
    }
}