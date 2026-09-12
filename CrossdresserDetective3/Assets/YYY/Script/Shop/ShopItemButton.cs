using UnityEngine;
using UnityEngine.UI;
using static GameFlowData;

public class ShopItemButton : MonoBehaviour
{
    [Header("商品信息")]
    [Tooltip("例如：melee_dagger")]
    public string itemId;

    [Tooltip("商品价格")]
    public int price;

    [Header("价格UI")]
    public GameObject priceRoot;
    public Text priceText;

    [Header("购买状态UI")]
    [Tooltip("可选。购买后显示，例如“已购买”图标")]
    public GameObject purchasedRoot;

    [Header("解锁条件")]
    [Tooltip("到达第几章后显示，填写1～6")]
    [Min(1)]
    public int requiredChapter = 1;

    [Tooltip("到达该章第几关后显示，填写1～10")]
    [Min(1)]
    public int requiredStage = 1;


    private void Start()
    {
        RefreshUI();
    }


    /// <summary>
    /// 刷新商品的显示、价格和购买状态
    /// </summary>
    public void RefreshUI()
    {
        if (string.IsNullOrEmpty(GameFlowData.CurrentPlayer))
        {
            Debug.LogWarning($"{name}：CurrentPlayer为空，无法读取商店存档");
            return;
        }

        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        //检查是否已经推进到商品要求的关卡
        bool unlocked = IsStageUnlocked(data);

        gameObject.SetActive(unlocked);

        if (!unlocked)
            return;

        bool purchased = data.HasPurchasedItem(itemId);

        //已经购买后隐藏价格
        if (priceRoot != null)
            priceRoot.SetActive(!purchased);

        if (priceText != null)
            priceText.text = price.ToString();

        //可选的“已购买”标记
        if (purchasedRoot != null)
            purchasedRoot.SetActive(purchased);
    }


    /// <summary>
    /// 点击商品时调用
    /// </summary>
    public void BuyItem()
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning($"{name}：没有填写商品 itemId");
            return;
        }

        if (string.IsNullOrEmpty(GameFlowData.CurrentPlayer))
        {
            Debug.LogWarning($"{name}：CurrentPlayer为空，无法购买");
            return;
        }

        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        //已经购买，不再重复扣钱
        if (data.HasPurchasedItem(itemId))
        {
            RefreshUI();

            //装配物品
            Equip();

            return;
        }

        //钱不够
        if (data.Money < price)
        {
            Debug.Log($"金钱不足：需要 {price}，当前持有 {data.Money}");

            //这里以后可以播放“钱不够”的声音或者显示提示
            AudioManager.Instance.PlayFX(AudioManager.Instance.Attack_pai1);
            return;
        }

        //扣钱
        data.Money -= price;

        //登记购买
        data.AddPurchasedItem(itemId);

        //一次性保存
        SaveManager.SaveGame(data);

        //刷新顶部金钱显示
        if (BalanceManager.instance != null)
        {
            BalanceManager.instance.ChangeMoney(0, false);
        }

        //刷新这个商品UI
        RefreshUI();

        Debug.Log($"购买成功：{itemId}");


        AudioManager.Instance.PlayFX(AudioManager.Instance.SE_Reji_2);

    }


    /// <summary>
    /// 判断要求的关卡是否已经解锁
    /// </summary>
    private bool IsStageUnlocked(SaveData data)
    {
        data.InitStageData();

        int chapterIndex = requiredChapter - 1;
        int stageIndex = requiredStage - 1;

        if (chapterIndex < 0 || chapterIndex >= data.chapterCount)
            return false;

        if (stageIndex < 0 || stageIndex >= data.stagePerChapter)
            return false;

        int index =
            chapterIndex * data.stagePerChapter +
            stageIndex;

        if (index < 0 || index >= data.stageStars.Length)
            return false;

        //-1表示未解锁，0～3表示已经解锁
        return data.stageStars[index] >= 0;
    }




    private void Equip()
    {
        switch (itemId)
        {
            case "melee_dagger":
                UIManager.instance.ChangeMelee(1);
                break;

            case "melee_katana":
                UIManager.instance.ChangeMelee(2);
                break;

            case "melee_kukri":
                UIManager.instance.ChangeMelee(3);
                break;





            case "ranged_colt_m1911":
                UIManager.instance.ChangeRanged(1);
                break;
            case "ranged_desert_eagle":
                UIManager.instance.ChangeRanged(2);
                break;
            case "ranged_glock":
                UIManager.instance.ChangeRanged(3);
                break;
            case "ranged_m4a1":
                UIManager.instance.ChangeRanged(11);
                break;
            case "ranged_ak47":
                UIManager.instance.ChangeRanged(12);
                break;




            case "throw_grenade":
                UIManager.instance.ChangeThrowable(1);
                break;
            case "throw_smoke":
                UIManager.instance.ChangeThrowable(2);
                break;
            case "throw_flash":
                UIManager.instance.ChangeThrowable(3);
                break;
            case "throw_incendiary":
                UIManager.instance.ChangeThrowable(4);
                break;
            case "throw_stun":
                UIManager.instance.ChangeThrowable(5);
                break;
            case "throw_knife":
                UIManager.instance.ChangeThrowable(6);
                break;





            case "clothes_pink_shirt":
                UIManager.instance.ChangeClothes(2);
                break;
            case "clothes_bunny":
                UIManager.instance.ChangeClothes(3);
                break;



            case "gloves_white_lace":
                UIManager.instance.ChangeGloves(2);
                break;
            case "gloves_black_silicone":
                UIManager.instance.ChangeGloves(3);
                break;




            case "skirt_black":
                UIManager.instance.ChangeSkirt(2);
                break;




            case "panties_white_lace":
                UIManager.instance.ChangePanties(2);
                break;

            case "panties_flat_chastity":
                UIManager.instance.ChangePanties(10);
                break;
            case "panties_chastity":
                UIManager.instance.ChangePanties(11);
                break;




            case "stockings_white_lace":
                UIManager.instance.ChangeStockings(2);
                break;
            case "stockings_black":
                UIManager.instance.ChangeStockings(3);
                break;





            case "shoes_black_boots":
                UIManager.instance.ChangeShoes(2);
                break;
            case "shoes_black_heels":
                UIManager.instance.ChangeShoes(3);
                break;

        }
    }
}