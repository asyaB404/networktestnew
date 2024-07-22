using GamePlay.Coins;
using UnityEngine;

namespace GamePlay.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CoinsPool coinsPool;
        public CoinsPool CoinsPool => coinsPool;

        [SerializeField] private PlayerController playerController;

        public float Health { get; private set; }
        public float Speed { get; private set; }

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
        }
    }
}