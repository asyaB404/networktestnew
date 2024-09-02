using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Coins;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] [AllowMutableSyncType]
        public SyncVar<CoinsPool> coinsPool = new();

        [SerializeField] private PlayerController playerController;

        [FormerlySerializedAs("speed")] [SerializeField]
        private float moveSpeed = 15;

        public float MoveSpeed => moveSpeed;
        public float Health { get; } = 100;

        #region Coins

        [AllowMutableSyncType] [SerializeField]
        private SyncList<Coin> catchingCoins = new SyncList<Coin>(new SyncTypeSettings(
            WritePermission.ClientUnsynchronized,
            ReadPermission.ExcludeOwner));

        public IList<Coin> CatchingCoins => catchingCoins;

        // 创建一个ServerRpc，以允许所有者在服务器上更新该值。
        [ServerRpc(RunLocally = true)]
        private void SetCatchingCoin(int index, Coin coin, NetworkConnection owner)
        {
            // GetComponent<NetworkObject>().SetLocalOwnership(owner);
            catchingCoins[index] = coin;
        }

        #endregion

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