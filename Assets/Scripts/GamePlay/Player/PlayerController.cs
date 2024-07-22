using System;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player player;
        private float _timer;
        private float MoveDuration => 1 / player.Speed;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}