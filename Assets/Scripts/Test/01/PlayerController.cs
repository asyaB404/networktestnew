using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class PlayerController : NetworkBehaviour
    {
        public Vector2 vector;
        public GameObject bullet;
        public SyncVar<float> Health { get; } = new(10f);
        [SerializeField] private int score;
        [SerializeField] private Text healthGui;

        private void Update()
        {
            if (IsOwner)
            {
                var x = Input.GetAxisRaw("Horizontal");
                var y = Input.GetAxisRaw("Vertical");
                vector = new Vector2(x, y);
                transform.Translate(vector * (Time.deltaTime * 4f));
                if (Input.GetMouseButtonDown(0))
                {
                    // AddScore();
                    Fire();
                    // DoScale();
                }
            }
        }


        private void Fire()
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPosition - (Vector2)transform.position).normalized;
            FireServerRpc(transform.position, direction);
        }

        [ServerRpc]
        private void FireServerRpc(Vector2 position, Vector2 direction)
        {
            GameObject sbullet = Instantiate(bullet, position + direction * 1.5f, Quaternion.identity);
            sbullet.GetComponent<Bullet>().Initialize(direction * 10);
            ServerManager.Spawn(sbullet);
        }


        [ServerRpc]
        private void Test()
        {
            DoScale();
        }

        [ObserversRpc]
        private void DoScale()
        {
            transform.localScale *= 1.5f;
        }


        [ServerRpc]
        private void AddScore()
        {
            score++;
            Debug.Log(score);
        }
    }
}