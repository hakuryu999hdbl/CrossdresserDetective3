using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableEffectBase : MonoBehaviour
{
    [Header("碰撞地板墙壁弹跳发出声音")]
    public LayerMask groundLayer;
    public FrameEvent_Audio frameEvent_Audio;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            frameEvent_Audio._Attack_bomb_bounce();
        }
    }
}
