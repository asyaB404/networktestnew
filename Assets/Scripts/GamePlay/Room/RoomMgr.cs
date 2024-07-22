using System;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;


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
        public int PlayerCount { get; private set; } = 1;

        [ContextMenu("test")]
        private void Print()
        {
            Debug.Log(PlayerCount);
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

        [ServerRpc]
        public void Join()
        {
            PlayerCount += 1;
        }

        [ServerRpc]
        public void Exit()
        {
            PlayerCount -= 1;
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}