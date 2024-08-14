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
        [SerializeField] private GameObject coinsPoolsPrefab;
        [SerializeField] private GameObject playerPrefab;
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

        public void InitRoom()
        {
            StartCoroutine(InitForMode((int)RoomMgr.Instance.CurRoomType));
        }

        /// <summary>
        /// 不知道为什么fishnet在同一帧中调用ServerManager.Spawn会出BUG,所以就用了协程
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private IEnumerator InitForMode(int i)
        {
            Transform mode = transform.GetChild(i);
            for (int j = 0; j < mode.childCount; j++)
            {
                Transform p = mode.GetChild(j);
                GameObject coinsPoolGobj = Instantiate(coinsPoolsPrefab, p, false);
                coinsPoolGobj.name = "coinsPool" + j;
                InstanceFinder.ServerManager.Spawn(coinsPoolGobj);
                var coinsPool = coinsPoolGobj.GetComponent<CoinsPool>();
                coinsPools.Add(coinsPool);
                yield return null;
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
    }
}