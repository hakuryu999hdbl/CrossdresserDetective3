using UnityEngine;

public class ChapterMapController : MonoBehaviour
{
    [Header("章节地图动画器")]
    public Animator mapAnimator;

    private int currentChapter = -1;

    public void SelectChapter(int chapterNumber)
    {
        // 防止鼠标或EventSystem重复触发相同动画
        if (currentChapter == chapterNumber)
            return;

        currentChapter = chapterNumber;

        mapAnimator.SetInteger("Chapter", chapterNumber);
    }
}