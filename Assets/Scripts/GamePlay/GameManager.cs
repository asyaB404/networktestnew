using System.Collections.Generic;
using GamePlay.Coins;
using UnityEngine;

namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public List<CoinsPool> coinsPools;
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}