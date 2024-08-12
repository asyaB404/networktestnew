using FishNet.Object;
using UnityEngine;

namespace Test
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private Vector2 velocity;
        private float _flyTime;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }


        private void Update()
        {
            if (!IsServerStarted) return;
            if (_flyTime <= 5)
                _flyTime += Time.deltaTime;
            else
                Despawn();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            // if (IsServerStarted) Despawn();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            rb.velocity = velocity;
        }

        public void Initialize(Vector2 initialVelocity)
        {
            velocity = initialVelocity;
            rb.velocity = velocity;
        }

        [ServerRpc]
        private void Damage(PlayerController playerController)
        {
            playerController.Health.Value -= 1;
        }

        // [ServerRpc]
        // private void Despawn()
        // {
        //     ServerManager.Despawn(gameObject);
        // }
    }
}