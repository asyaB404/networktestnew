using System.Runtime.CompilerServices;
using FishNet.Object;
using GamePlay.Coins;
using UnityEngine;

namespace GamePlay.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private CoinsPool coinsPool;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float speed = 4;
        public CoinsPool CoinsPool
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => coinsPool;
        }

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

        public float Health { get; } = 100;
        public float Speed => speed;

        private void Awake()
        {
            playerController ??= GetComponent<PlayerController>();
        }
    }
}