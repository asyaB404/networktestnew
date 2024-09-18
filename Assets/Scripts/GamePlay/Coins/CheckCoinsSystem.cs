using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Coins
{
    public struct CheckCoinsSystem
    {
        private int _minY;
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
            if (checkCoinsSystem._hashSet.Count >= 2)
            {
            }
            else if (checkCoinsSystem._hashSet.Count >= 5)
            {
            }

            checkCoinsSystem._hashSet.Clear();
        }

        private void CheckForCoin(Vector2Int pos)
        {
            Coin coin = _coinsPool.GetCoin(pos);
            if (coin == null || _hashSet.Contains(pos) || coin.coinsType.Value != _coinsType)
                return;
            _hashSet.Add(pos);
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