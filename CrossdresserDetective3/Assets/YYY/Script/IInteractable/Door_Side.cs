using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Side : MonoBehaviour
{
    private Animator anim;
    public GameObject Ground;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&& anim!=null)
        {
            anim.SetBool("isOpen", true);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && anim != null)
        {
            anim.SetBool("isOpen", false);
        }
    }
    public void Open() 
    {
        Ground.SetActive(false);
    }
    public void Close()
    {
        Ground.SetActive(true);
    }
}
