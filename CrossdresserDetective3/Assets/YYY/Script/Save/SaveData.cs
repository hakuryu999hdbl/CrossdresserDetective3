using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public string slotName = "CurrentPlayer"; //存档名称名字
    public string saveTime;// 存档时间（字符串）
    public string NextAreaId;//当前区域位置(进入该场景时保存)



    public int chapterCount = 8;//总共8个章节
    public int stagePerChapter = 10;//每个章节10关卡
    // 8章 × 10关 = 80
    public int[] stageStars;


    public void InitStageData()
    {
        int total = chapterCount * stagePerChapter;

        if (stageStars == null || stageStars.Length != total)
        {
            stageStars = new int[total];

            for (int i = 0; i < total; i++)
                stageStars[i] = -1; // 全部未解锁

            stageStars[0] = 0; // 第一章第一关默认解锁
        }
    }


    //服装
    public int clothesIndex;
    public int glovesIndex;
    public int pantiesIndex;
    public int shoesIndex;
    public int skirtIndex;
    public int stockingsIndex;

    //武器
    public int meleeType;
    public int pistolType;
    public int rifleType;
    public int throwType;


    //当前状态
    public int attackType;









    // ✅ 加上这个构造函数 ↓↓↓↓↓↓↓↓↓
    public SaveData(string name)
    {
        slotName = name;
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // ✅ 如果你也调用过 new SaveData() 这种无参数形式，也要保留这个：
    public SaveData()
    {
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    }


    public void InitDefaultEquip()
    {
        clothesIndex = 1;
        glovesIndex = 1;
        pantiesIndex = 1;
        shoesIndex = 1;
        skirtIndex = 1;
        stockingsIndex = 1;

        meleeType = 0;
        pistolType = 0;
        rifleType = 0;
        throwType = 0;

        attackType = 0;
    }//默认状态
}
