using DG.Tweening;
using FishNet.Object;
using GamePlay.Coins;
using GamePlay.Room;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private Player player;

        #region CatchAndShootCoin

        private float _catchTimer;
        [SerializeField] private float catchDuration;

        private float _shootTimer;
        [SerializeField] private float shootDuration;

        #endregion

        #region Move

        private float X => player.X;
        private int XInt => Mathf.FloorToInt(player.X);
        private float _moveTimer;
        private Tween _moveTween;
        private float MoveDuration => 1 / player.MoveSpeed;

        #endregion

        private void Awake()
        {
            player ??= GetComponent<Player>();
        }

        [SerializeField] private Ease ease = Ease.InQuad;

        private void CatchCoin(CoinsPool coinsPool, Vector2Int key, Coin coin)
        {
            RPCInstance.Instance.SetOwnerShip(coin, player.Owner);
            // Transform parent = player.CatchingCoinsParent;
            // coin.transform.SetParent(parent);
            coinsPool.SetCoinsDict(key, null);
            coin.SetCoinStatus(CoinStatus.Catching);
            SyncCoinsParentRequest(coin);
            coin.catchTween = coin.transform.DOLocalMove(Vector3.zero, player.MoveSpeed * 8).SetSpeedBased();
            player.AddCatchingCoin(coin);
        }

        [ServerRpc(RunLocally = true, RequireOwnership = false)]
        private void SyncCoinsParentRequest(Coin coin, bool isPlayer = true)
        {
            var parent = isPlayer ? player.CatchingCoinsParent : coin.coinsPool.Value.coinsParent.transform;
            coin.transform.SetParent(parent);
            SyncCoinsParent(coin, isPlayer);
        }

        [ObserversRpc(ExcludeOwner = true, ExcludeServer = true)]
        private void SyncCoinsParent(Coin coin, bool isPlayer = true)
        {
            var parent = isPlayer ? player.CatchingCoinsParent : coin.coinsPool.Value.coinsParent.transform;
            coin.transform.SetParent(parent);
        }

        private void CatchCoins()
        {
            CoinsPool coinsPool = player.coinsPool.Value;
            Vector2Int key = new Vector2Int(XInt, 0);
            var minY = coinsPool.FindCoinMinY(XInt);
            key.y = minY;
            Coin coin = coinsPool.GetCoin(key);
            while (key.y <= 0 && coin.coinStatus.Value == CoinStatus.Idle)
            {
                if (player.CatchingCoins.Count <= 0)
                {
                    CatchCoin(coinsPool, key, coin);
                }
                else if (player.CatchingCoins.Count > 0)
                {
                    if (coin.coinsType.Value == player.CatchingCoins[0].coinsType.Value)
                    {
                        CatchCoin(coinsPool, key, coin);
                    }
                    else
                    {
                        break;
                    }
                }

                key.y++;
                coin = coinsPool.GetCoin(key);
            }
        }

        private void Update()
        {
            #region Move

            if (_moveTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.A) && X > 0)
                {
                    _moveTween.Kill(true);
                    _moveTween = transform.DOLocalMoveX(X - 1, MoveDuration).SetEase(ease);
                    _moveTimer = MoveDuration;
                }
                else if (Input.GetKeyDown(KeyCode.D) && X < player.coinsPool.Value.Weight - 1)
                {
                    _moveTween.Kill(true);
                    _moveTween = transform.DOLocalMoveX(X + 1, MoveDuration).SetEase(ease);
                    _moveTimer = MoveDuration;
                }
            }
            else
            {
                _moveTimer -= Time.deltaTime;
            }

            #endregion

            #region Catch

            if (_catchTimer <= 0 && Input.GetKeyDown(KeyCode.K))
            {
                CatchCoins();
                _catchTimer = catchDuration;
            }
            else
            {
                _catchTimer -= Time.deltaTime;
            }

            #endregion

            #region Shoot

            if (_shootTimer <= 0)
            {
            }
            else
            {
            }

            #endregion
        }
    }
}