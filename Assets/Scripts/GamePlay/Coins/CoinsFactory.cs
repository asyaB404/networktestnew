using System;
using UnityEngine;

namespace GamePlay.Coins
{
    public enum CoinsType
    {
        Null = -1,
        C1,
        C5,
        C10,
        C50,
        C100,
        C500
    }

    public class CoinsFactory : MonoBehaviour
    {
        public GameObject coinPrefab;
        public Sprite[] coinSprites;
        public static CoinsFactory Instance { get; private set; }

        public static bool IsTypeNeedTwo(CoinsType coinsType)
        {
            return coinsType switch
            {
                CoinsType.C5 or CoinsType.C50 or CoinsType.C500 => true,
                _ => false
            };
        }
        
        public static bool IsTypeNeedFive(CoinsType coinsType)
        {
            return coinsType switch
            {
                CoinsType.C1 or CoinsType.C10 or CoinsType.C100 => true,
                _ => false
            };
        }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 在服务端创建硬币，一般为服务端调用，同时也会给客户端同步生成
        /// </summary>
        /// <param name="coinsType"></param>
        /// <param name="pos"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public Coin GenerateCoin(CoinsType coinsType)
        {
            var coinObj = Instantiate(coinPrefab);
            var coin = coinObj.GetComponent<Coin>();
            coin.coinsType.Value = coinsType;
            coin.sr.sprite = coinSprites[(int)coinsType];
            return coin;
        }
    }
}