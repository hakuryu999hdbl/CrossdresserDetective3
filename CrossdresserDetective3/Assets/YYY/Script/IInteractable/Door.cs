using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("对应出口位置")]
    [SerializeField] private Transform target;

    [Header("对应出口门")]
    [SerializeField] private Door targetDoor;

    [Header("玩家")]
    [SerializeField] private PlayerController player;

    [Header("时间")]
    [SerializeField] private float enterTime = 0.5f;
    [SerializeField] private float exitTime = 0.5f;

    [Header("提示")]
    public GameObject Effect;

    private Animator anim;
    private bool isTeleporting;//是否传送

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();

        }
    }

    public void TriggerAction()
    {
        if (isTeleporting) return;//位于传送中禁止传送
        if (target == null || player == null) return;

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        isTeleporting = true;

        // 1. 锁玩家控制
        player.isTeleporting=true;

        // 2. 当前门开 + 玩家进门淡出
        PlayOpen();
        //player.PlayFadeOut();
        player.playerAnimation.anim.SetTrigger("Teleport");

        yield return new WaitForSeconds(enterTime);

        // 3. 真正传送
        player.transform.position = target.position;

        // 4. 目标门开 + 玩家出门淡入
        if (targetDoor != null)
        {
            targetDoor.PlayOpen();
        }

        //player.PlayFadeIn();

        yield return new WaitForSeconds(exitTime);

        // 5. 解锁玩家控制
        player.isTeleporting = false;

        isTeleporting = false;
    }

    public void PlayOpen()
    {
        if (anim != null)
        {
            anim.SetTrigger("Open");
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            Effect.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            Effect.SetActive(false);
        }
    }















    public void OpenDoor()
    {
        //anim.Play("Exit_Open");
        //coll.enabled = true;
    }//GameManager 调用


   

}
