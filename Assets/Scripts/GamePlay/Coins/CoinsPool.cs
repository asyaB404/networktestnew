using FishNet.Object;
using UnityEngine;

namespace GamePlay.Coins
{
    public class CoinsPool : NetworkBehaviour
    {
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        [SerializeField] private GameObject readySprite;
        [SerializeField] private GameObject playersParent;
        [SerializeField] private GameObject coinsParent;

        public bool IsReady
        {
            get => readySprite.activeSelf;
            set => readySprite.SetActive(value);
        }

        public void GameStart()
        {
            IsReady = false;
        }
    }
}