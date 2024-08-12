using System;
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
        public List<CoinsPool> coinsPools;
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            if (InstanceFinder.IsServerStarted)
            {
                InitRoom();
            }
            else if (InstanceFinder.IsClientStarted)
            {
                CoinsPool[] pools = GetComponentsInChildren<CoinsPool>(true);
                coinsPools = new List<CoinsPool>(pools);
            }
        }

        private void OnDisable()
        {
            if (InstanceFinder.IsServerStarted)
            {
                coinsPools.Clear();
            }
            else if (InstanceFinder.IsClientStarted)
            {
                coinsPools.Clear();
            }
        }


        private void InitRoom()
        {
            InitForMode((int)RoomMgr.Instance.CurType);
            return;

            void InitForMode(int i)
            {
                Transform mode = transform.GetChild(i);
                for (int j = 0; j < mode.childCount; j++)
                {
                    Transform p = mode.GetChild(j);
                    GameObject coinsPool = Instantiate(coinsPoolsPrefab, p, false);
                    InstanceFinder.ServerManager.Spawn(coinsPool);
                    coinsPools.Add(coinsPool.GetComponent<CoinsPool>());
                }
            }
        }

        public void SetReady(int id, bool flag)
        {
            coinsPools[id].IsReady = flag;
        }
    }
}