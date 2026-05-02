using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("传送目标")]
    [SerializeField] private Transform target;

    [Header("传送对象")]
    [SerializeField] private Transform player;



    Animator anim;
    BoxCollider2D coll;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        coll = GetComponent <BoxCollider2D>();


        //coll.enabled = false;

        //GameManager.instance.IsExit(this);//告诉GameManager自己是出口
    }
    public void OpenDoor() 
    {
        //anim.Play("Exit_Open");
        //coll.enabled = true;
    }//GameManager 调用


    //public void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        GameManager.instance.NextScene();
    //    }
    //}

    public void TriggerAction()
    {
        //传送到对应门

        if (target == null)
        {
            Debug.LogWarning($"{name} 没有设置 Target");
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("没有找到 Player");
            return;
        }

        player.position = target.position;

        if (anim != null)
        {
            anim.SetBool("isOpen",true);
        }
    }
}
