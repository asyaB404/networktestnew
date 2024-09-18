using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Coins
{
    public struct CheckCoinsSystem
    {
        /// <summary>
        /// 初始为int.MaxValue
        /// </summary>
        private int _minY;

        /// <summary>
        /// 初始为int.MaxValue
        /// </summary>
        private int _minX;

        private readonly CoinsPool _coinsPool;
        private readonly CoinsType _coinsType;
        private readonly HashSet<Vector2Int> _hashSet;

        public CheckCoinsSystem(CoinsPool coinsPool, CoinsType coinsType)
        {
            _coinsPool = coinsPool;
            _coinsType = coinsType;
            _minY = int.MaxValue;
            _minX = int.MaxValue;
            _hashSet = new HashSet<Vector2Int>();
        }

        public static void CheckForCoins(CoinsPool coinsPool, CoinsType coinsType, Vector2Int pos)
        {
            CheckCoinsSystem checkCoinsSystem = new CheckCoinsSystem(coinsPool, coinsType);
            checkCoinsSystem.CheckForCoin(pos);
            int count = checkCoinsSystem._hashSet.Count;
            if ((count >= 2 && CoinsFactory.IsTypeNeedTwo(coinsType)) || 
                (count >= 5 && CoinsFactory.IsTypeNeedFive(coinsType)))
            {
                foreach (var key in checkCoinsSystem._hashSet)
                {
                    coinsPool.GetCoin(key).DeSpawnRequest();
                }
            }
            checkCoinsSystem._hashSet.Clear();
        }

        private void CheckForCoin(Vector2Int pos)
        {
            Coin coin = _coinsPool.GetCoin(pos);
            if (coin == null || _hashSet.Contains(pos) || coin.coinsType.Value != _coinsType)
                return;
            _hashSet.Add(pos);
            //记录生成位置
            if (pos.y <= _minY)
            {
                _minY = pos.y;
                _minX = pos.x;
            }

            pos.y -= 1;
            CheckForCoin(pos);
            pos.x -= 1;
            CheckForCoin(pos);
            pos.y += 2;
            CheckForCoin(pos);
            pos.x += 2;
            CheckForCoin(pos);
        }
    }
}