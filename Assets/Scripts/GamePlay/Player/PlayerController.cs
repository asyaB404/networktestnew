using DG.Tweening;
using GamePlay.Coins;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
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
                CoinsPool coinsPool = player.coinsPool.Value;
                int minY;
                Vector2Int key = new Vector2Int(XInt, 0);
                if (player.CatchingCoins.Count <= 0)
                {
                    minY = coinsPool.FindCoinMinY(XInt);
                    key.y = minY;
                    CoinsType coinsType = coinsPool.GetCoin(key).coinsType.Value;
                }
                else
                {
                }

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