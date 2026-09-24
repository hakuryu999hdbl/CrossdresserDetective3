using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EquippedItemMark : MonoBehaviour
{
    public enum EquipType
    {
        Clothes,
        Gloves,
        Panties,
        Shoes,
        Skirt,
        Stockings,

        Melee,
        Pistol,
        Rifle,
        Throw,


        RangedNone //远程武器“无”
    }

    [Header("对应的装备类型")]
    public EquipType equipType;

    [Header("这个商品对应的装备数字")]
    public int itemIndex;

    [Header("装备中标记")]
    public GameObject equippedMark; //旁边的E

    private Button button;


    private void Awake()
    {
        button = GetComponent<Button>();

        //点击商品后自动刷新
        button.onClick.AddListener(OnItemClicked);
    }


    private void Start()
    {
        RefreshAllMarks();
    }


    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnItemClicked);
        }
    }


    private void OnItemClicked()
    {
        //等待原本的装备代码完成并保存
        StartCoroutine(RefreshNextFrame());
    }


    private IEnumerator RefreshNextFrame()
    {
        yield return null;

        RefreshAllMarks();
    }


    /// <summary>
    /// 刷新场景中所有商品的E标记
    /// </summary>
    public void RefreshAllMarks()
    {
        if (string.IsNullOrEmpty(GameFlowData.CurrentPlayer))
            return;

        SaveData data =
            SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        EquippedItemMark[] allMarks =
            FindObjectsByType<EquippedItemMark>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (EquippedItemMark mark in allMarks)
        {
            mark.RefreshMark(data);
        }
    }


    private void RefreshMark(SaveData data)
    {
        bool isEquipped = false;

        switch (equipType)
        {
            //远程武器：无
            case EquipType.RangedNone:

                isEquipped = data.rangedSlot == 0;
                break;


            //手枪不仅要型号相同
            //当前远程栏还必须选择手枪
            case EquipType.Pistol:

                isEquipped =
                    data.rangedSlot == -1 &&
                    data.pistolType == itemIndex;

                break;


            //步枪不仅要型号相同
            //当前远程栏还必须选择步枪
            case EquipType.Rifle:

                isEquipped =
                    data.rangedSlot == -2 &&
                    data.rifleType == itemIndex;

                break;


            //其他服装、近战、投掷物保持原来的判断
            default:

                int currentIndex =
                    GetCurrentEquipIndex(data);

                isEquipped =
                    currentIndex == itemIndex;

                break;
        }

        equippedMark.SetActive(isEquipped);
    }


    private int GetCurrentEquipIndex(SaveData data)
    {
        switch (equipType)
        {
            case EquipType.Clothes:
                return data.clothesIndex;

            case EquipType.Gloves:
                return data.glovesIndex;

            case EquipType.Panties:
                return data.pantiesIndex;

            case EquipType.Shoes:
                return data.shoesIndex;

            case EquipType.Skirt:
                return data.skirtIndex;

            case EquipType.Stockings:
                return data.stockingsIndex;

            case EquipType.Melee:
                return data.meleeType;

            case EquipType.Pistol:
                return data.pistolType;

            case EquipType.Rifle:
                return data.rifleType;

            case EquipType.Throw:
                return data.throwType;
        }

        return -999;
    }
}