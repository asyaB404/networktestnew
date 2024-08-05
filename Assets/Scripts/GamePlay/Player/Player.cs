using GamePlay.Coins;
using UnityEngine;

namespace GamePlay.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CoinsPool coinsPool;

        [SerializeField] private PlayerController playerController;
        public CoinsPool CoinsPool => coinsPool;

        public float Health { get; }
        public float Speed { get; }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }
    }
}