
using DG.Tweening;
using FishNet.Object;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private Player player;
        private float _moveTimer;

        private Tween _moveTween;
        private float MoveDuration => 1 / player.Speed;

        private float X => player.X;

        private void Awake()
        {
            player ??= GetComponent<Player>();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!base.IsOwner)
            {
                this.enabled = false;
            }
        }

        private void Update()
        {
            if (_moveTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.A) && X > 0)
                {
                    _moveTween.Kill(true);
                    _moveTween = transform.DOLocalMoveX(X - 1, MoveDuration).SetEase(Ease.Linear);
                    _moveTimer = MoveDuration;
                }
                else if (Input.GetKeyDown(KeyCode.D) && X < player.CoinsPool.Weight - 1)
                {
                    _moveTween.Kill(true);
                    _moveTween = transform.DOLocalMoveX(X + 1, MoveDuration).SetEase(Ease.Linear);
                    _moveTimer = MoveDuration;
                }
            }
            else
            {
                _moveTimer -= Time.deltaTime;
            }
        }
    }
}