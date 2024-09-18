using System.Collections.Generic;
using FishNet;
using FishNet.CodeGenerating;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Room;
using UnityEngine;
using Random = System.Random;


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

        private Random _random;

        public Random RandomInstance
        {
            get => _random ??= new Random(0);
            set => _random = value;
        }

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

        [ServerRpc(RequireOwnership = false, RunLocally = true)]
        public void SetCoinsDict(Vector2Int key, Coin coin)
        {
            if (coin == null)
            {
                coinsDict.Remove(key);
                return;
            }

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
            if (IsServerStarted && RPCInstance.CurStatus == PlayerStatus.Gaming &&
                playersParent.transform.childCount > 0)
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

        private void FallCoin(Vector2Int pos, int fallHeight = 1)
        {
            var coin = coinsDict[pos];
            coinsDict.Remove(pos);
            pos.y -= fallHeight;
            coinsDict[pos] = coin;
            coin.movingController.MoveTo(new Vector2(pos.x, pos.y), coinsFallSpeed, -1);
        }

        public int FindCoinMinY(int x)
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

        [ServerRpc]
        public void SpawnCoinRequest(CoinsType coinsType, Vector2 pos,
            NetworkConnection owner = null)
        {
            SpawnCoin(coinsType, pos, owner);
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
            InstanceFinder.ServerManager.Spawn(newCoin.gameObject, owner);
            return newCoin;
        }

        [Server]
        public void SpawnRowCoins(int count = 1)
        {
            if (count >= Height)
                count = Height;
            for (int i = 0; i < Weight; i++)
            {
                int minY = FindCoinMinY(i);
                Vector2Int pos = new(i, minY);
                while (pos.y <= 0)
                {
                    FallCoin(pos, count);
                    pos.y++;
                }
            }


            for (int i = 0; i < Weight; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    var targetPos = new Vector2Int(i, -j);
                    Coin coin = SpawnCoin(CoinsType.C1 + RandomInstance.Next(6), targetPos);
                    coin.transform.localPosition = new Vector3(i, 1);
                    coin.movingController.MoveTo(new Vector2(i, -j), coinsFallSpeed, -1);
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
            int i = 0;
            foreach (var pair in coinsDict)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.red;
                }
                else if (i == 1)
                {
                    Gizmos.color = Color.blue;
                }
                else if (i == 2)
                {
                    Gizmos.color = Color.green;
                }

                Gizmos.DrawCube(coinsParent.transform.position + new Vector3(pair.Key.x, pair.Key.y),
                    new(0.3f, 0.3f, 0));
                i++;
                i %= 3;
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