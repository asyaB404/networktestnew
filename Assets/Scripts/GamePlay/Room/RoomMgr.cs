using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class RoomMgr : NetworkBehaviour
{
    [SerializeField] private List<GamePanel> rooms = new(4);
    public RoomMgr Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}