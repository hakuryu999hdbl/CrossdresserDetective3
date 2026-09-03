using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour,IInteractable
{
    private SpriteRenderer spriteRenderer;

    public Sprite openSprite;
    //public Sprite closeSprite;
    public List<Sprite> closeSprites = new List<Sprite>();
    private Sprite currentCloseSprite;

    public bool isDone;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始随机选择一个关闭状态外观
        if (closeSprites != null && closeSprites.Count > 0)
        {
            currentCloseSprite = closeSprites[
                Random.Range(0, closeSprites.Count)
            ];
        }

    }

    private void OnEnable()
    {
        //spriteRenderer.sprite = isDone ? openSprite : closeSprite;
        spriteRenderer.sprite = isDone ? openSprite : currentCloseSprite;

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
                                            //Destroy(gameObject);

        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Keyboard);
    }

}
