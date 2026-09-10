using UnityEngine;

public class ChapterMapController : MonoBehaviour
{
    [Header("章节地图动画器")]
    public Animator mapAnimator;

    private int currentChapter = -1;


    public GameObject Chapter_Name_1;
    public GameObject Chapter_Name_2;
    public GameObject Chapter_Name_3;
    public GameObject Chapter_Name_4;
    public GameObject Chapter_Name_5;
    public GameObject Chapter_Name_6;

    public void SelectChapter(int chapterNumber)
    {
        // 防止鼠标或EventSystem重复触发相同动画
        if (currentChapter == chapterNumber)
            return;

        currentChapter = chapterNumber;

        mapAnimator.SetInteger("Chapter", chapterNumber);


        Chapter_Name_1.SetActive(false);
        Chapter_Name_2.SetActive(false);
        Chapter_Name_3.SetActive(false);
        Chapter_Name_4.SetActive(false);
        Chapter_Name_5.SetActive(false);
        Chapter_Name_6.SetActive(false);


        switch (chapterNumber)
        {

            case 1:
                Chapter_Name_1.SetActive(true);
                break;

            case 2:
                Chapter_Name_2.SetActive(true);
                break;
            case 3:
                Chapter_Name_3.SetActive(true);
                break;
            case 4:
                Chapter_Name_4.SetActive(true);
                break;
            case 5:
                Chapter_Name_5.SetActive(true);
                break;
            case 6:
                Chapter_Name_6.SetActive(true);
                break;
        }



    }
}