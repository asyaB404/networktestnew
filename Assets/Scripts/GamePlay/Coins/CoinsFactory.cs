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

    public class CoinsFactory : MonoBehaviour
    {
        public GameObject coinPrefab;
        public Sprite[] coinSprites;
        public static CoinsFactory Instance { get; private set; }

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
        [Server]
        public Coin GenerateCoin(CoinsType coinsType,
            NetworkConnection owner = null)
        {
            var coinObj = Instantiate(coinPrefab);
            var coin = coinObj.GetComponent<Coin>();
            coin.coinsType.Value = coinsType;
            coin.sr.sprite = coinSprites[(int)coinsType];
            InstanceFinder.ServerManager.Spawn(coinObj, owner);
            return coin;
        }
    }
}