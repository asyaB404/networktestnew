using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Object;
using GamePlay.Coins;
using GamePlay.Room;
using UnityEngine;
using Random = System.Random;

namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public GameObject coinsPoolsPrefab;
        public GameObject playerPrefab;
        public List<CoinsPool> coinsPools;
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            InstanceFinder.SceneManager.OnClientLoadedStartScenes += (connection, b) =>
            {
                if (b && connection == RoomMgr.Instance.PlayersCon[0])
                {
                    InitRoom();
                }
            };
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            coinsPools.Clear();
        }

        private async UniTask InitRoom()
        {
            await InitForMode((int)RoomMgr.Instance.CurRoomType);
        }

        /// <summary>
        /// 不知道为什么fishnet在同一帧中调用ServerManager.Spawn会出BUG,所以就用了异步编程
        /// </summary>
        /// <param name="roomTypeIndex"></param>
        /// <returns></returns>
        private async UniTask InitForMode(int roomTypeIndex)
        {
            Transform mode = transform.GetChild(roomTypeIndex);
            for (int j = 0; j < mode.childCount; j++)
            {
                Transform p = mode.GetChild(j);
                GameObject coinsPoolGobj = Instantiate(coinsPoolsPrefab, p, false);
                coinsPoolGobj.name = "coinsPool" + j;
                InstanceFinder.ServerManager.Spawn(coinsPoolGobj, RoomMgr.Instance.PlayersCon[0]);
                var coinsPool = coinsPoolGobj.GetComponent<CoinsPool>();
                coinsPools.Add(coinsPool);
        
                // 替代 yield return null
                await UniTask.Yield();

                if (j == 0)
                {
                    var playerObj = Instantiate(playerPrefab, coinsPool.playersParent.transform, false);
                    playerObj.GetComponent<Player.Player>().coinsPool.Value = coinsPool;
                    InstanceFinder.ServerManager.Spawn(playerObj, RoomMgr.Instance.PlayersCon[0]);
            
                    // 替代 yield return null
                    await UniTask.Yield();
                }
            }
        }

        /// <summary>
        /// 改变那个硬币池的准备状态，同时那个状态为SyncVar，自动同步到客户端
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag"></param>
        public void SetReadySprite(int id, bool flag)
        {
            coinsPools[id].SetIsReadySprite(flag);
        }

        /// <summary>
        /// 服务端开始游戏时调用一次
        /// </summary>
        [Server]
        public void StartGame()
        {
            int seed = UnityEngine.Random.Range(-9999999, 99999999);
            foreach (var coinsPool in coinsPools)
            {
                coinsPool.RandomInstance = new Random(seed);
                coinsPool.StartGame();
            }
        }

        #region debug

        [SerializeField, ContextMenuItem("spawnCoin", nameof(SpawnCoinTest))]
        private CoinsType spawnTypeTest;

        public void SpawnCoinTest()
        {
            // CoinFactory.Instance.GenerateCoin(spawnTypeTest,Vector2.zero, coinsPools[0],InstanceFinder.ClientManager.Connection);
        }

        #endregion
    }
}