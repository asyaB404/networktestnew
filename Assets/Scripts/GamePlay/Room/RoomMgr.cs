using System.Collections.Generic;
using UI.Panel;
using UnityEngine;

namespace GamePlay.Room
{
    public class RoomMgr : MonoBehaviour
    {
        public RoomType CurType { get; private set; }
        [SerializeField] private List<GamePanel> rooms = new(4);
        public RoomMgr Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            Debug.Log("123");
        }
    }
}