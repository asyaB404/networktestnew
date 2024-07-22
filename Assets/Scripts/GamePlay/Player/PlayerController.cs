using DG.Tweening;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player player;
        private float _moveTimer;
        private float MoveDuration => 1 / player.Speed;

        private Vector3 Pos => transform.localPosition;
        private float X => transform.localPosition.x;
        private float Y => transform.localPosition.y;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Update()
        {
            if (_moveTimer <= 0 && X > 0 & X < player.CoinsPool.Weight - 1)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    transform.DOLocalMoveX(X - 1,MoveDuration);
                    _moveTimer = MoveDuration;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    transform.DOLocalMoveX(X + 1,MoveDuration);
                    _moveTimer = MoveDuration;
                }
            }
        }

        private void FixedUpdate()
        {
        }
    }
}