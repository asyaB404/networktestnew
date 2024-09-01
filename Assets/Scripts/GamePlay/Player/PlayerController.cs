using DG.Tweening;
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

        private float _moveTimer;
        private Tween _moveTween;
        private float MoveDuration => 1 / player.Speed;

        #endregion

        private float X => player.X;

        private void Awake()
        {
            player ??= GetComponent<Player>();
        }

        [SerializeField] private Ease ease = Ease.InQuad;

        private void Update()
        {
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

            if (_catchTimer <= 0)
            {
            }
            else
            {
            }

            if (_shootTimer <= 0)
            {
            }
            else
            {
            }
        }
    }
}