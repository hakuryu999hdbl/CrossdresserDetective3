using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spine_FrameEvents : MonoBehaviour
{
    public Animator Introduce;
    public Animator CG_Photo;
    public Animator CG_Clock;


    //当前播放的动画器
    private Animator currentAnimator;

    public DialogSystem dialogSystem;

    public void SetCurrentAnimator()
    {
       

        switch (GameFlowData.nextAreaId)
        {
            case "Introduce":
                dialogSystem.animation_number = 0;
                currentAnimator = Introduce;
                break;
            case "CG_Photo":
                dialogSystem.animation_number = 1;
                currentAnimator = CG_Photo;
                break;
            case "CG_Clock":
                dialogSystem.animation_number = 6;
                currentAnimator = CG_Clock;
                break;
            //case "CG_Bomb":
            //    currentAnimator = CG_Bomb;
            //    break;
            //case "CG_Mirror":
            //    currentAnimator = CG_Mirror;
            //    break;

        }

        currentAnimator.gameObject.SetActive(true);
    }



    public void TriggerNext()
    {
        AudioManager.Instance.fxSource.Stop();//个人感觉声音因该是特效这一栏
        currentAnimator.SetTrigger("Next");
    }
}
