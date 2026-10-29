using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;

    [Header("类型：默认是普通箱子")]
    public bool isComputer = false;

    [Header("外观")]
    public Sprite openSprite;
    public List<Sprite> closeSprites = new List<Sprite>();
    private Sprite currentCloseSprite;

    public bool isDone;

    // 防止反复启用时重复登记调查数量
    private bool clueRegistered;

    [Header("箱子奖励：第0个放金币，其余放普通物品")]
    public List<GameObject> dropItemList = new List<GameObject>();

    [Min(1)]
    public int moneyDropMin = 1;

    [Min(1)]
    public int moneyDropMax = 3;

    [Range(0f, 1f)]
    public float itemDropChance = 0.25f;

    [Header("掉落位置与弹出力度")]
    public Vector3 dropOffset = new Vector3(0f, 0.5f, 0f);
    public float dropForceX = 3f;
    public float dropForceY = 5f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentCloseSprite = spriteRenderer.sprite;

        if (closeSprites != null && closeSprites.Count > 0)
        {
            currentCloseSprite =
                closeSprites[Random.Range(0, closeSprites.Count)];
        }
    }

    private void OnEnable()
    {
        spriteRenderer.sprite =
            isDone ? openSprite : currentCloseSprite;

        // 只有电脑参与调查任务统计
        if (isComputer && !clueRegistered)
        {
            GameManager.instance.RegisterClue();
            clueRegistered = true;
        }
    }

    public void TriggerAction()
    {
        if (isDone || GameManager.instance.player.isBondage)
            return;

        OpenChest();
    }

    private void OpenChest()
    {
        // 箱子奖励完全没配置时不允许打开，避免空箱
        if (!isComputer && !DropChestRewards())
            return;

        isDone = true;
        spriteRenderer.sprite = openSprite;
        gameObject.tag = "Untagged";

        if (isComputer)
        {
            GameManager.instance.CompleteClue();

            AudioManager.Instance.PlayFX(
                AudioManager.Instance.SE_Keyboard
            );
        }
    }

    private bool DropChestRewards()
    {
        List<GameObject> normalItems = new List<GameObject>();

        if (dropItemList != null)
        {
            for (int i = 1; i < dropItemList.Count; i++)
            {
                if (dropItemList[i] != null)
                    normalItems.Add(dropItemList[i]);
            }
        }

        GameObject moneyPrefab =
            dropItemList != null && dropItemList.Count > 0
                ? dropItemList[0]
                : null;

        if (moneyPrefab == null && normalItems.Count == 0)
        {
            Debug.LogError("普通箱子没有配置任何奖励", this);
            return false;
        }

        // 金币必掉，数量至少为1
        if (moneyPrefab != null)
        {
            int min = Mathf.Max(1, moneyDropMin);
            int max = Mathf.Max(min, moneyDropMax);
            int count = Random.Range(min, max + 1);

            for (int i = 0; i < count; i++)
                SpawnDropItem(moneyPrefab);
        }

        // 没有金币时保底掉一件；有金币时按概率额外掉落
        if (normalItems.Count > 0 &&
            (moneyPrefab == null || Random.value < itemDropChance))
        {
            SpawnDropItem(
                normalItems[Random.Range(0, normalItems.Count)]
            );
        }

        return true;
    }

    private void SpawnDropItem(GameObject prefab)
    {
        GameObject item = Instantiate(
            prefab,
            transform.position + dropOffset,
            Quaternion.identity
        );

        Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();

        if (itemRb == null)
            return;

        float dir = Random.value < 0.5f ? -1f : 1f;

        itemRb.AddForce(
            new Vector2(
                dir * Random.Range(
                    dropForceX * 0.7f,
                    dropForceX * 1.3f
                ),
                Random.Range(
                    dropForceY * 0.8f,
                    dropForceY * 1.2f
                )
            ),
            ForceMode2D.Impulse
        );
    }
}