using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.Coins
{
    public class CoinsPool : MonoBehaviour
    {
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        [SerializeField] private GameObject readySprite;

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