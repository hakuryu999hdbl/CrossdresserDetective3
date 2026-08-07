using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFlowData
{
    public static string nextAreaId = null;     //重刷场景名称("CG_Clock" / null 等)

    public static int CurrentChapter;//目前的章节
    public static int CurrentStage;//目前的章节内关卡



    public static string CurrentPlayer = null;   //目前使用的是哪个存档
    public static string returnPath = null;      // 回来的路径 ("cg" / "chapter_1" / null 等)

    public static bool suppressNextSelectSound = false;//二级菜单按钮选中声音吞掉
    public static bool suppressNextClickSound = false;//商店购买声音吞掉

    public enum EquipPart
    {
        Belt,
        Clothes,
        Gloves,
        Panties,
        Shoes,
        Skirt,
        Stockings,

        Melee,
        Pistol,
        Rifle,
        Throw
    }
}
