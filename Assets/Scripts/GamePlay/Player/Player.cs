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
        [SerializeField] private float speed = 8;

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
            if (base.IsOwner)
            {
            }
            else
            {
                transform.SetParent(coinsPool.Value.playersParent.transform, false);
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
            Debug.Log(base.IsOwner);
            Debug.Log(coinsPool.Value);
        }

        #endregion
    }
}