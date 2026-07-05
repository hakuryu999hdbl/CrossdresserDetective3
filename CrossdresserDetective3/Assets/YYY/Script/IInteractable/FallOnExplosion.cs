using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOnExplosion : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D col;

    [Header("ù{óéï®óù")]
    public float fallMass = 0.4f;
    public float gravityScale = 1f;
    public float drag = 1.5f;
    public float angularDrag = 2f;

    [Header("îÌ‡yóéèâë¨ìx")]
    public float sideVelocity = 1.5f;
    public float downVelocity = 2.5f;
    public float torqueForce = 8f;

    private bool hasFallen;


    private void Start()
    {
        //rb.gravityScale =0;
        //rb = GetComponent<Rigidbody2D>();
    }

    public void OnBlastHit(Vector2 blastCenter)
    {
        //rb.gravityScale = 1;
        Fall(blastCenter);
    }

    private void Fall(Vector2 blastCenter)
    {
        if (hasFallen) return;
        hasFallen = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        rb.mass = fallMass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        float dirX = transform.position.x >= blastCenter.x ? 1f : -1f;

        rb.velocity = new Vector2(
            dirX * sideVelocity,
            -downVelocity
        );

        rb.AddTorque(
            Random.Range(-torqueForce, torqueForce),
            ForceMode2D.Impulse
        );
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (hasFallen) return;
    //
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
    //    {
    //        Fall(other.transform.position);
    //    }
    //}
}
