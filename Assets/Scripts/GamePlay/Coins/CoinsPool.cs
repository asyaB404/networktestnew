using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GamePlay.Room;
using UnityEngine;

namespace GamePlay.Coins
{
    public class CoinsPool : NetworkBehaviour
    {
        public static bool IsSynced;
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        #region SyncVar

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
            //只会同步一次
            if (!IsSynced && InstanceFinder.IsClientOnlyStarted && GameManager.Instance.coinsPools.Count == 0)
            {
                Debug.Log("从服务端获得硬币池列表并同步");
                SyncCoinsPoolsRequest();
                IsSynced = true;
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
        public void SyncCoinsPoolsRequest()
        {
            SyncCoinsPoolsToClient(GameManager.Instance.coinsPools, (int)RoomMgr.Instance.CurRoomType);
        }

        [ObserversRpc]
        public void SyncCoinsPoolsToClient(List<CoinsPool> coinsPools, int i)
        {
            Debug.Log(i);
            GameManager.Instance.coinsPools = coinsPools;
            SetCoinsPoolFromMode(i);
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

        /// <summary>
        /// 服务端调用
        /// </summary>
        public void StartGame()
        {
            SetIsReadySprite(false);
        }
    }
}