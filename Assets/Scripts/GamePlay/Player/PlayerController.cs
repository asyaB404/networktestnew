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

        #region Catch

        private void CatchCoin(Vector2Int key, Coin coin)
        {
            // Transform parent = player.CatchingCoinsParent;
            // coin.transform.SetParent(parent);
            // if (coin.shootTween != null && coin.shootTween.IsActive())
            // {
            //     coin.shootTween.onComplete();
            //     coin.transform.DOKill();
            // }

            var coinsPool = player.coinsPool.Value;
            coinsPool.SetCoinsDict(key, null);
            coin.SetCoinStatus(CoinStatus.Catching);
            // SyncCoinsParentRequest(coin);
            coin.transform.SetParent(player.CatchingCoinsParent);
            // coin.catchTween = coin.transform.DOLocalMove(Vector3.zero, player.MoveSpeed * 8f).SetSpeedBased();
            coin.movingController.MoveTo(Vector2.zero, player.MoveSpeed * 8f, 1);
            player.AddCatchingCoin(coin);
        }

        private void CatchCoins()
        {
            CoinsPool coinsPool = player.coinsPool.Value;
            Vector2Int key = new Vector2Int(XInt, 0);
            var minY = coinsPool.FindCoinMinY(XInt);
            key.y = minY;
            Coin coin = coinsPool.GetCoin(key);
            while (key.y <= 0 && coin.coinStatus.Value != CoinStatus.Transforming)
            {
                if (player.CatchingCoins.Count <= 0)
                {
                    CatchCoin(key, coin);
                }
                else
                {
                    if (coin.coinsType.Value == player.CatchingCoins[0].coinsType.Value)
                    {
                        CatchCoin(key, coin);
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

        #endregion

        private void ShootCoin(Coin coin, Vector2Int key)
        {
            // if (coin.shootTween != null && coin.shootTween.IsActive())
            // {
            //     coin.shootTween.onComplete();
            //     coin.transform.DOKill();
            // }

            var coinsPool = player.coinsPool.Value;
            coinsPool.SetCoinsDict(key, coin);
            coin.SetCoinStatus(CoinStatus.Moving);
            // SyncCoinsParentRequest(coin, false);
            coin.transform.SetParent(coin.coinsPool.Value.coinsParent.transform);
            // coin.shootTween = coin.transform.DOLocalMove(new Vector3(key.x, key.y), player.MoveSpeed * 8f)
            //     .SetSpeedBased().OnComplete(
            //         () => { coin.SetCoinStatus(CoinStatus.Idle); });
            coin.movingController.MoveTo(Vector2.zero, player.MoveSpeed * 8f, 1,
                () => { coin.SetCoinStatus(CoinStatus.Idle); });
        }

        private void ShootCoins()
        {
            if (player.CatchingCoins.Count > 0)
            {
                CoinsPool coinsPool = player.coinsPool.Value;
                Vector2Int key = new(XInt, 0);
                var minY = coinsPool.FindCoinMinY(XInt) - 1;
                key.y = minY;
                foreach (var coin in player.CatchingCoins)
                {
                    ShootCoin(coin, key);
                    key.y -= 1;
                }

                player.CatchingCoins.Clear();
            }
        }

        #region SyncCoinsParent

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

        #endregion

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

            if (_catchTimer <= 0 && Input.GetKeyDown(KeyCode.K))
            {
                CatchCoins();
                _catchTimer = catchDuration;
            }
            else
            {
                _catchTimer -= Time.deltaTime;
            }


            if (_shootTimer <= 0 && Input.GetKeyDown(KeyCode.I))
            {
                ShootCoins();
                _shootTimer = shootDuration;
            }
            else
            {
                _shootTimer -= Time.deltaTime;
            }
        }
    }
}