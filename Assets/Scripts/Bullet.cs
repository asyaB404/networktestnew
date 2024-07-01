using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField]
    private Vector2 velocity;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}