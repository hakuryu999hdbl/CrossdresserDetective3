using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterButtonSelect : MonoBehaviour,
    ISelectHandler,
    IPointerEnterHandler
{
    [Range(1, 6)]
    public int chapterNumber = 1;

    public ChapterMapController mapController;

    public void OnSelect(BaseEventData eventData)
    {
        mapController.SelectChapter(chapterNumber);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}