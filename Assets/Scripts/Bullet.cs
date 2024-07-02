using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private Vector2 velocity;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    [ServerRpc]
    private void Damage(PlayerController playerController)
    {
        playerController.Health.Value -= 1;
    }
}