using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PitfallPlatform : MonoBehaviour
{
    private BoxCollider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryCollapse(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryCollapse(collision);
    }

    private void TryCollapse(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponentInParent<PlayerController>();
        if (player == null || player.isDead)
            return;

        float top = platformCollider.bounds.max.y;
        if (collision.collider.bounds.center.y <= top)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).point.y >= top - 0.15f)
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }
}
