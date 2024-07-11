using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private Vector2 velocity;
    private float _flyTime = 0;

    private Rigidbody2D rb;


    private void Update()
    {
        if (!base.IsServerStarted) return;
        if (_flyTime <= 5)
        {
            _flyTime += Time.deltaTime;
        }
        else
        {
            Despawn();
        }
    }

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
        if (IsServerStarted)
        {
            Despawn();
        }
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