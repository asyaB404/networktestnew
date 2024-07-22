using System.Collections.Generic;
using GamePlay.Coins;
using UnityEngine;


namespace GamePlay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public List<CoinsPool> coinsPools;

        private void Awake()
        {
            Instance = this;
        }
    }
}