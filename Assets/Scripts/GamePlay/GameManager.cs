using System.Collections;
using System.Collections.Generic;
using FishNet;
using GamePlay.Coins;
using GamePlay.Room;
using UnityEngine;

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
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (InstanceFinder.IsServerStarted)
            {
                InitRoom();
            }
            else if (InstanceFinder.IsClientStarted)
            {
                // CoinsPool[] pools = GetComponentsInChildren<CoinsPool>(true);
                // coinsPools = new List<CoinsPool>(pools);
            }
        }

        private void OnDisable()
        {
            coinsPools.Clear();
        }

        private void InitRoom()
        {
            StartCoroutine(InitForMode((int)RoomMgr.Instance.CurRoomType));
        }

        /// <summary>
        /// 不知道为什么fishnet在同一帧中调用ServerManager.Spawn会出BUG,所以就用了协程
        /// </summary>
        /// <param name="roomTypeIndex"></param>
        /// <returns></returns>
        private IEnumerator InitForMode(int roomTypeIndex)
        {
            Transform mode = transform.GetChild(roomTypeIndex);
            for (int j = 0; j < mode.childCount; j++)
            {
                Transform p = mode.GetChild(j);
                GameObject coinsPoolGobj = Instantiate(coinsPoolsPrefab, p, false);
                coinsPoolGobj.name = "coinsPool" + j;
                InstanceFinder.ServerManager.Spawn(coinsPoolGobj, RoomMgr.Instance.PlayersCon[0]);
                var coinsPool = coinsPoolGobj.GetComponent<CoinsPool>();
                coinsPool.id.Value = j;
                coinsPools.Add(coinsPool);
                yield return null;
                if (j == 0)
                {
                    var playerObj = Instantiate(playerPrefab, coinsPool.playersParent.transform, false);
                    playerObj.GetComponent<Player.Player>().coinsPool.Value = coinsPool;
                    InstanceFinder.ServerManager.Spawn(playerObj, RoomMgr.Instance.PlayersCon[0]);
                    yield return null;
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

        public void StartGame()
        {
            foreach (var coinsPool in coinsPools)
            {
                coinsPool.StartGame();
            }
        }

        #region debug

        [SerializeField, ContextMenuItem("spawnCoin",nameof(SpawnCoinTest))]
        private CoinsType spawnTypeTest;
        public void SpawnCoinTest()
        {
            CoinFactory.Instance.GenerateCoin(spawnTypeTest,Vector2.zero, coinsPools[0],InstanceFinder.ClientManager.Connection);
        }

        #endregion
    }
}