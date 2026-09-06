using UnityEngine;
using UnityEngine.UI;

public class CGGallerySlot : MonoBehaviour
{
    public enum CGPlaybackType
    {
        RBQ,    // 当前场景内播放RBQ
        Spine   // 跳转Spine场景
    }

    [Header("所属控制器")]
    public CGGalleryController galleryController;

    [Header("CG信息")]
    public string cgId;
    public Sprite previewSprite;

    [Header("播放方式")]
    public CGPlaybackType playbackType;

    [Tooltip("只有RBQ类型才需要填写")]
    public int animationType;

    [Header("选中显示")]
    public GameObject selectedMark;

    /// <summary>
    /// 由Button的OnClick调用
    /// </summary>
    public void SelectThisCG()
    {
        galleryController.SelectCG(this);
    }

    public void SetSelected(bool selected)
    {
        if (selectedMark != null)
        {
            selectedMark.SetActive(selected);
        }
    }
}