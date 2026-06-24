using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombKicker : MonoBehaviour
{
    float kickForceX = 60f;
    float kickForceY = 100f;
    public bool clearVelocity = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bomb")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float dir = transform.root.localScale.x >= 0 ? 1f : -1f;

        if (clearVelocity)
        {
            rb.velocity = Vector2.zero;
        }

        rb.AddForce(
            new Vector2(dir * kickForceX, kickForceY),
            ForceMode2D.Impulse
        );
    }
}
