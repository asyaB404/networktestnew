
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace GamePlay.Coins
{
    public enum CoinsType
    {
        C1,
        C5,
        C10,
        C50,
        C100,
        C500
    }

    public class CoinFactory : MonoBehaviour
    {
        public GameObject coinPrefab;
        public Sprite[] coinSprites;
        public static CoinFactory Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// 在服务端创建硬币，一般为服务端调用，同时也会给客户端同步生成
        /// </summary>
        /// <param name="coinsType"></param>
        /// <param name="pos"></param>
        /// <param name="coinsPool"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        [Server]
        public Coin GenerateCoin(CoinsType coinsType, Vector2 pos, CoinsPool coinsPool,
            NetworkConnection owner = null)
        {
            var coinObj = Instantiate(coinPrefab, coinsPool.coinsParent.transform, false);
            var coin = coinObj.GetComponent<Coin>();
            coin.transform.localPosition = pos;
            coin.coinsType.Value = coinsType;
            coin.sr.sprite = coinSprites[(int)coinsType];
            coin.coinsPool.Value = coinsPool;
            InstanceFinder.ServerManager.Spawn(coinObj, owner);
            return coin;
        }
    }
}