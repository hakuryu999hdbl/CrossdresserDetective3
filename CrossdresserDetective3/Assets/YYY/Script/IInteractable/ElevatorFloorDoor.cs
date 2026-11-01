using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloorDoor : MonoBehaviour, IInteractable
{
    public Collider2D blockCollider;
    //public Animator anim;

    [Header("电梯引用")]
    public ElevatorController elevator; //叫上来叫下去
    public bool isTopDoor;

    [Header("电梯方向显示")]
    public GameObject upImage;
    public GameObject downImage;

    public void SetDirection(bool goingUp)
    {
        if (upImage != null)
            upImage.SetActive(goingUp);

        if (downImage != null)
            downImage.SetActive(!goingUp);
    }


    public void Open()
    {
        if (blockCollider != null)
            blockCollider.enabled = false;

        //if (anim != null)
        //    anim.SetBool("Open", true);
    }

    public void Close()
    {
        if (blockCollider != null)
            blockCollider.enabled = true;

        //if (anim != null)
        //    anim.SetBool("Open", false);
    }



    public void TriggerAction()
    {

        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Button);

        if (isTopDoor)
            elevator.CallToTop();
        else
            elevator.CallToBottom();
    }

}