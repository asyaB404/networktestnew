using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Room;
using Unity.VisualScripting;
using UnityEngine;

namespace GamePlay.Coins
{
    /// <summary>
    /// 硬币池的Owner是房主
    /// </summary>
    public class CoinsPool : NetworkBehaviour
    {
        public static bool IsSynced;
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        #region SyncVar

        public readonly SyncVar<int> id = new();
        private readonly SyncVar<bool> _isReady = new(false);

        [ServerRpc(RequireOwnership = false)]
        public void SetIsReadySprite(bool value)
        {
            _isReady.Value = value;
        }

        private void OnChangeReady(bool prev, bool next, bool asServer)
        {
            readySprite.SetActive(next);
        }

        #endregion

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
                    // if不是旁观的话
                    SpawnPlayer(id.Value);
                    IsSynced = true;
                }
            }

            _isReady.OnChange += OnChangeReady;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            IsSynced = false;
            _isReady.OnChange -= OnChangeReady;
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

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayer(int id, NetworkConnection owner = null)
        {
            var gameManager = GameManager.Instance;
            var coinsPool = gameManager.coinsPools[id];
            var playerObj = Instantiate(gameManager.playerPrefab, coinsPool.playersParent.transform, false);
            playerObj.GetComponent<Player.Player>().coinsPool.Value = coinsPool;
            InstanceFinder.ServerManager.Spawn(playerObj, owner);
        }

        /// <summary>
        /// 服务端调用
        /// </summary>
        [Server]
        public void StartGame()
        {
            SetIsReadySprite(false);
        }
    }
}