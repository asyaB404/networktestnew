using System.Collections.Generic;
using DG.Tweening;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Room;
using UnityEngine;

namespace GamePlay.Coins
{
    /// <summary>
    /// 硬币池的Owner是房主
    /// </summary>
    public class CoinsPool : NetworkBehaviour
    {
        public static bool IsSynced { get; private set; }
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        #region SyncVar

        private readonly SyncVar<bool> _isReady = new(false);

        [ServerRpc(RequireOwnership = false)]
        public void SetIsReadySprite(bool value)
        {
            _isReady.Value = value;
        }

        private void OnReadyChanged(bool prev, bool next, bool asServer)
        {
            readySprite.SetActive(next);
        }

        #endregion

        private Dictionary<Vector2Int, Coin> _coinsDict;

        public IReadOnlyDictionary<Vector2Int, Coin> CoinsDict => _coinsDict;

        [SerializeField] private GameObject readySprite;

        public GameObject playersParent;

        public GameObject coinsParent;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (InstanceFinder.IsClientOnlyStarted)
            {
                var gameManager = GameManager.Instance;
                if (!IsSynced && gameManager.coinsPools.Count == 0)
                {
                    Debug.Log("从服务端获得硬币池列表并同步");
                    SyncCoinsPoolsRequest();
                    SpawnPlayer();
                    IsSynced = true;
                }
            }

            _isReady.OnChange += OnReadyChanged;
        }


        public override void OnStopClient()
        {
            base.OnStopClient();
            IsSynced = false;
            _isReady.OnChange -= OnReadyChanged;
        }

        #region SyncCoinsPools

        [ServerRpc(RequireOwnership = false)]
        public void SyncCoinsPoolsRequest(NetworkConnection target = null)
        {
            SyncCoinsPoolsToClient(target, GameManager.Instance.coinsPools, (int)RoomMgr.Instance.CurRoomType);
        }

        [TargetRpc]
        public void SyncCoinsPoolsToClient(NetworkConnection conn, List<CoinsPool> coinsPools, int roomType)
        {
            GameManager.Instance.coinsPools = coinsPools;
            SetCoinsPoolFromMode(roomType);
        }

        private void SetCoinsPoolFromMode(int i)
        {
            Transform mode = GameManager.Instance.transform.GetChild(i);
            for (int j = 0; j < mode.childCount; j++)
            {
                Transform p = mode.GetChild(j);
                GameManager.Instance.coinsPools[j].transform.SetParent(p, false);
            }
        }

        #endregion

        #region spawnPlayer

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayer(int id = -1, NetworkConnection owner = null)
        {
            if (id == -1)
            {
                id = GetNextSpawnIndex();
            }

            var gameManager = GameManager.Instance;
            var coinsPool = gameManager.coinsPools[id];
            var playerObj = Instantiate(gameManager.playerPrefab, coinsPool.playersParent.transform, false);
            playerObj.GetComponent<Player.Player>().coinsPool.Value = coinsPool;
            InstanceFinder.ServerManager.Spawn(playerObj, owner);
        }

        [Server]
        private int GetNextSpawnIndex()
        {
            switch (RoomMgr.Instance.CurRoomType)
            {
                case RoomType.T1V1:
                case RoomType.T2V2:
                case RoomType.T4VS:
                    for (var i = 0; i < GameManager.Instance.coinsPools.Count; i++)
                    {
                        var coinsPool = GameManager.Instance.coinsPools[i];
                        if (coinsPool.playersParent.transform.childCount == 0)
                            return i;
                    }

                    break;
                default:
                    Debug.LogError("错误的enum");
                    break;
            }

            Debug.LogError("怎么回事呢。。？GetNextSpawnIndex 返回了-1");
            return -1;
        }

        #endregion

        [Server]
        private void InitCoinsPool()
        {
        }

        [Server]
        public Coin SpawnCoin(CoinsType coinsType, Vector2 pos,
            NetworkConnection owner = null)
        {
            Coin newCoin = CoinsFactory.Instance.GenerateCoin(coinsType, owner);
            newCoin.transform.SetParent(coinsParent.transform, false);
            newCoin.transform.localPosition = pos;
            newCoin.coinsPool.Value = this;
            Vector2Int key = pos.ToVectorInt();
            if (_coinsDict.ContainsKey(key))
            {
                Debug.LogWarning("将" + key + "上的值覆盖了");
            }

            _coinsDict[key] = newCoin;
            return newCoin;
        }

        [Server]
        private void SpawnRowCoins(int count = 1)
        {
            if (count >= Height)
                count = Height;
            for (int i = 0; i < Weight; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Coin coin = SpawnCoin(CoinsType.C1 + Random.Range(0, 6), new Vector2(i, 1));
                    coin.transform.DOLocalMoveY(-j, 10).SetSpeedBased();
                }
            }
        }

        /// <summary>
        /// 服务端上调用,开始游戏时,每个硬币池都会调用一次
        /// </summary>
        [Server]
        public void StartGame()
        {
            SetIsReadySprite(false);
            Debug.Log("当前房间模式为:" + RoomMgr.Instance.CurRoomType);
            switch (RoomMgr.Instance.CurRoomType)
            {
                case RoomType.T1V1:
                case RoomType.T2V2:
                case RoomType.T4VS:
                    SpawnRowCoins(5);
                    break;
                default:
                    Debug.LogError("enum");
                    break;
            }
        }
    }
}