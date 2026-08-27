using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonUI : MonoBehaviour
{
    public Button button;


    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public void SetStageState(int star)
    {
        bool unlocked = star >= 0;

        gameObject.SetActive(unlocked);

        if (button != null)
            button.interactable = unlocked;

        star1.SetActive(unlocked && star >= 1);
        star2.SetActive(unlocked && star >= 2);
        star3.SetActive(unlocked && star >= 3);
    }

    [Header("关卡")]
    public int chapter;
    public int stage;

    [Header("任务类型")]
    public GameFlowData.MissionType missionType;

    [Header("地图类型")]
    public GameFlowData.MapType mapType;

    public void Click()
    {
        GameFlowData.CurrentMissionType = missionType;//记录当前任务
        GameFlowData.CurrentMapType = mapType;//记录当前地图

        MenuManager.instance.NewGame(chapter, stage);
    }
}
