using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    BoxCollider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent <BoxCollider2D>();
        coll.enabled = false;

        GameManager.instance.IsExit(this);
    }
    public void OpenDoor() 
    {
        anim.Play("Exit_Open");
        coll.enabled = true;
    }//GameManager 调用


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.NextScene();
        }
    }
}
