using System.Collections.Generic;
using UI.Panel;
using UnityEngine;

namespace GamePlay.Room
{
    public class RoomMgr : MonoBehaviour
    {
        public GameObject roomPrefab;
        [SerializeField] private List<GamePanel> rooms = new(4);
        public RoomType CurType { get; private set; }
        public static RoomMgr Instance { get; private set; }
        public int PlayerCount { get; private set; }


        public Room Create(RoomType roomType)
        {
            GameObject obj = Instantiate(roomPrefab);
            if (roomType == RoomType.T1V1)
            {
                NetworkMgr.Instance.tugboat.SetMaximumClients(2);
            }
            else
            {
                NetworkMgr.Instance.tugboat.SetMaximumClients(4);
            }

            NetworkMgr.Instance.networkManager.ServerManager.Spawn(obj);
            return obj.GetComponent<Room>();
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}