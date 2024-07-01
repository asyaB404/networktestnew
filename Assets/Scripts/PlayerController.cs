using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
 
    public Vector2 vector;
    public GameObject bullet;
    [SerializeField] private int score;

    
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
                AddScore();
                // Fire();
                // DoScale();
            }
        }
    }


    [ServerRpc]
    private void Fire()
    {
        GameObject sbullet = Instantiate(bullet, transform.position, Quaternion.identity);
        transform.position = transform.position + new Vector3(1, 0, 0);
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 velocity = mouseWorldPosition - (Vector2)transform.position;
        sbullet.GetComponent<Bullet>().Initialize(velocity);
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