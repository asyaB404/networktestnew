using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FishNet;
using FishNet.CodeGenerating;
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
        public int coinsFallSpeed = 10;
        public static bool IsSynced { get; private set; }
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        public GameObject playersParent;

        public GameObject coinsParent;

        private System.Random _random;

        #region IsReady

        [SerializeField] private GameObject readySprite;

        [AllowMutableSyncType] [SerializeField]
        private SyncVar<bool> isReady = new(false);

        [ServerRpc(RequireOwnership = false)]
        public void SetIsReadySprite(bool value)
        {
            isReady.Value = value;
        }

        private void OnReadyChanged(bool prev, bool next, bool asServer)
        {
            readySprite.SetActive(next);
        }

        #endregion

        #region Coins

        [AllowMutableSyncType] [SerializeField]
        private SyncDictionary<Vector2Int, Coin> coinsDict =
            new(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

        [ServerRpc(RunLocally = true)]
        private void SetCoinsDict(Vector2Int key, Coin coin)
        {
            if (coin == null)
                coinsDict.Remove(key);

            coinsDict[key] = coin;
        }

        public IReadOnlyDictionary<Vector2Int, Coin> CoinsDict => coinsDict;


        public Coin GetCoin(Vector2Int pos)
        {
            if (!coinsDict.TryGetValue(pos, out var res)) Debug.LogWarning(pos + " 这个位置不存在一个元素");
            return res;
        }

        #endregion


        [SerializeField] private float spawnDuration = 5;
        private float _spawnTimer;

        private void Update()
        {
            if (IsServerStarted && RPCInstance.CurStatus != PlayerStatus.Gaming)
            {
                if (_spawnTimer > 0)
                {
                    _spawnTimer -= Time.deltaTime;
                }
                else
                {
                    SpawnRowCoins(1);
                    _spawnTimer = spawnDuration;
                }
            }
        }

        private void Awake()
        {
            _random = new System.Random(MyRandom.Seed);
        }

        #region OnClient

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

            isReady.OnChange += OnReadyChanged;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            IsSynced = false;
            isReady.OnChange -= OnReadyChanged;
        }

        #endregion

        #region SyncCoinsPoolsInFirst

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

        #region SpawnPlayerInFirst

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

        private void FallCoin(Coin coin, int fallHeight)
        {
            var pos = ((Vector2)coin.transform.localPosition).ToVectorInt();
            pos.y -= fallHeight;
            coin.transform.DOLocalMoveY(pos.y, coinsFallSpeed).SetSpeedBased();
            coinsDict[pos] = coin;
            // coinsDict.Dirty(pos);
        }

        private int FindCoinMinY(int x)
        {
            int y = 0;
            Vector2Int key;
            while (y >= -Height)
            {
                key = new Vector2Int(x, y);
                if (coinsDict.ContainsKey(key))
                {
                    y--;
                }
                else
                {
                    return y + 1;
                }
            }

            return 1;
        }

        [Server]
        public Coin SpawnCoin(CoinsType coinsType, Vector2 pos,
            NetworkConnection owner = null)
        {
            Coin newCoin = CoinsFactory.Instance.GenerateCoin(coinsType);
            newCoin.transform.SetParent(coinsParent.transform, false);
            newCoin.transform.localPosition = pos;
            newCoin.coinsPool.Value = this;
            Vector2Int key = pos.ToVectorInt();
            coinsDict[key] = newCoin;
            // coinsDict.Dirty(key);
            InstanceFinder.ServerManager.Spawn(newCoin.gameObject, owner);
            return newCoin;
        }

        [Server]
        public void SpawnRowCoins(int count = 1)
        {
            Vector2Int targetPos;
            if (count >= Height)
                count = Height;
            foreach (var item in coinsDict.ToList())
            {
                FallCoin(item.Value, count);
            }

            for (int i = 0; i < Weight; i++)
            {
                // int curY = FindCoinMinY(i);
                // while (curY <= 0)
                // {
                //     FallCoin(coinsDict[new Vector2Int(i, curY)], count);
                //     curY++;
                // }
                for (int j = 0; j < count; j++)
                {
                    targetPos = new Vector2Int(i, -j);
                    Coin coin = SpawnCoin(CoinsType.C1 + _random.Next(6), targetPos);
                    coin.transform.localPosition = new Vector3(i, 1);
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

        #region Debug

        void OnDrawGizmos()
        {
            foreach (var pair in coinsDict)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(coinsParent.transform.position + new Vector3(pair.Key.x, pair.Key.y),
                    new(0.3f, 0.3f, 0));
            }
        }

        [ContextMenu(nameof(Fun1))]
        private void Fun1()
        {
            foreach (var item in coinsDict)
            {
                Debug.Log(item.Key + " : " + item.Value);
            }
        }

        #endregion
    }
}