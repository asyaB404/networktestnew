using System.Collections.Generic;
using FishNet.Object;
using UI.Panel;
using UnityEngine;

public class RoomMgr : NetworkBehaviour
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