using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingOnBlast : MonoBehaviour
{
    public float maxAngle = 12f;
    public float swingSpeed = 8f;
    public float damping = 1.2f;
    public float swingDuration = 3f;

    bool swinging;
    float timer;
    float dir = 1f;

    public void OnBlastHit(Vector3 blastPos)
    {
        timer = 0f;
        swinging = true;

        dir = transform.position.x >= blastPos.x ? 1f : -1f;
    }

    void Update()
    {
        if (!swinging) return;

        timer += Time.deltaTime;

        float angle =
            Mathf.Sin(timer * swingSpeed) *
            maxAngle *
            Mathf.Exp(-timer * damping) *
            dir;

        transform.localRotation = Quaternion.Euler(0, 0, angle);

        if (timer >= swingDuration)
        {
            transform.localRotation = Quaternion.identity;
            swinging = false;
        }
    }
}
