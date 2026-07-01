using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpark : MonoBehaviour
{
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.15f;
    public float lifeTime = 3f;

    private bool landed;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (landed) return;

        bool hitGround = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (hitGround)
        {
            Land();
        }
    }

    void Land()
    {
        landed = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
}
