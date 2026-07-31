using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable
{
    private SpriteRenderer spriteRenderer;

    public Sprite openSprite;
    public Sprite closeSprite;

    public bool isDone;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;

        GameManager.instance.RegisterClue();//登记数量
    }

    public void TriggerAction()
    {
        //Debug.Log("Open Chest!");
        if (isDone) return;
        OpenChest();
    }

    private void OpenChest()
    {
        isDone = true;
        spriteRenderer.sprite = openSprite;

        // TODO: 掉落物品 / 加金币 / 播放音效

        this.gameObject.tag = "Untagged";//防止再跳出提示



        GameManager.instance.CompleteClue();//增加数量
        Destroy(gameObject);
    }

}
