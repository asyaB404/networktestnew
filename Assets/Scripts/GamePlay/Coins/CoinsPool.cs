using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using GamePlay.Room;
using UnityEngine;

namespace GamePlay.Coins
{
    public class CoinsPool : NetworkBehaviour
    {
        public static bool IsSynced;
        public int Weight { get; private set; } = 8;
        public int Height { get; private set; } = 16;

        [SerializeField] private GameObject readySprite;
        [SerializeField] private GameObject playersParent;
        [SerializeField] private GameObject coinsParent;

        public bool IsReady
        {
            get => readySprite.activeSelf;
            set => readySprite.SetActive(value);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsSynced && InstanceFinder.IsClientOnlyStarted && GameManager.Instance.coinsPools.Count == 0)
            {
                SyncCoinsPoolsRequest();
                IsSynced = true;
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            IsSynced = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncCoinsPoolsRequest()
        {
            SyncCoinsPoolsToClient(GameManager.Instance.coinsPools, (int)RoomMgr.Instance.CurType);
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

        public void GameStart()
        {
            IsReady = false;
        }
    }
}