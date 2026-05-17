using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOnExplosion : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;

    public float fallMass = 0.4f;
    public float upwardForce = 1.5f;
    public float sideForce = 2f;
    public float torqueForce = 8f;

    private bool hasFallen;

    public void OnBlastHit(Vector2 blastCenter)
    {
        if (hasFallen) return;
        hasFallen = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.mass = fallMass;
        rb.drag = 1.5f;
        rb.angularDrag = 2f;

        Vector2 dir = ((Vector2)transform.position - blastCenter).normalized;
        dir.y += upwardForce;

        rb.AddForce(dir * sideForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-torqueForce, torqueForce), ForceMode2D.Impulse);
    }
}
