using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloorDoor : MonoBehaviour, IInteractable
{
    public Collider2D blockCollider;
    //public Animator anim;

  


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



    //ã©è„óàã©â∫ãé
    public ElevatorController elevator;
    public bool isTopDoor;

    public void TriggerAction()
    {

        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Button);

        if (isTopDoor)
            elevator.CallToTop();
        else
            elevator.CallToBottom();
    }

}