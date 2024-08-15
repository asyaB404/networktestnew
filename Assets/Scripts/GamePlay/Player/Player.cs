using System.Runtime.CompilerServices;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Coins;
using UnityEngine;

namespace GamePlay.Player
{
    public class Player : NetworkBehaviour
    {
        public readonly SyncVar<CoinsPool> coinsPool = new();
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float speed = 15;

        #region pos

        public Vector3 Pos
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transform.localPosition;
        }

        public float X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transform.localPosition.x;
        }

        public float Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transform.localPosition.y;
        }

        #endregion

        public override void OnStartClient()
        {
            base.OnStartClient();
            playerController.enabled = IsOwner;
            // 服务端的父子关系是正确的,客户端不是,需要单独设置正确的父子关系
            if (IsClientOnlyStarted)
                transform.SetParent(coinsPool.Value.playersParent.transform, false);

            if (base.IsOwner)
            {
            }
            else
            {
            }
        }

        public float Health { get; } = 100;
        public float Speed => speed;

        private void Awake()
        {
            playerController ??= GetComponent<PlayerController>();
        }

        #region Debug

        [ContextMenu("Debug01")]
        private void Fun()
        {
            Debug.Log(base.IsOwner + " __ " + Owner);
            Debug.Log(coinsPool.Value);
        }

        #endregion
    }
}