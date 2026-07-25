using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{

    public bool canExit = true;

    private Animator anim;

    [Header("玩家")]
    [SerializeField] private PlayerController player;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        GameManager.instance.IsExit(this);

        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();

        }
    }

    public void TriggerAction()
    {
        if (!canExit) return;
        if (GameManager.instance.PlayerWin) return;


      

        GameManager.instance.PlayerEscapeWin();

        anim.SetTrigger("Open");
        player.playerAnimation.anim.SetBool("TeleportStop", true);//为了只触发一次这个进门动画，不触发离开门
        player.playerAnimation.anim.SetTrigger("Teleport");




        //防止敌人碰到玩家
        
        //player.gameObject.SetActive(false);//防止敌人打到玩家

    }


}
