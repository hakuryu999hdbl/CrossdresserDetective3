using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CGGalleryController : MonoBehaviour
{
    [Header("选择界面")]
    public GameObject galleryUI;
    public Image largePreviewImage;
    public GameObject playButton;
    [SerializeField] private Button pageButton;
    [SerializeField] private Text pageLabel;

    [Header("CG播放界面")]
    public GameObject playbackUI;

    [Header("场景中的RBQ")]
    public RBQController rbqController;

    [Header("全部CG Slot")]
    public CGGallerySlot[] slots;

    private const int SlotsPerPage = 8;
    private int currentPage;
    private int pageCount = 1;
    private CGGallerySlot currentSelectedSlot;

    private PlayerInputControl inputControl;

    //当前是否正在播放CG
    private bool isPlayingCG;


    private void Awake()
    {
        inputControl = new PlayerInputControl();

        inputControl.UI.Cancel.started += OnCancel;
    }

    private void OnEnable()
    {
        inputControl.UI.Enable();
    }

    private void OnDisable()
    {
        inputControl.UI.Disable();
    }

    private void OnDestroy()
    {
        inputControl.UI.Cancel.started -= OnCancel;
        inputControl.Dispose();
    }

    private void Start()
    {

        RefreshGallerySlots();

        //刚进入场景时没有任何CG被选中
        ClearSelection();


    }


    /// <summary>
    /// 根据全局解锁记录显示CG Slot
    /// </summary>
    public void RefreshGallerySlots()
    {
        var unlockedIds = GlobalSaveManager.Load().unlockedCGIds;
        var playbackType = isCG ? CGGallerySlot.CGPlaybackType.Spine : CGGallerySlot.CGPlaybackType.RBQ;
        int unlockedCount = 0;
        foreach (CGGallerySlot slot in slots)
        {
            if (slot != null && slot.playbackType == playbackType && unlockedIds.Contains(slot.cgId))
            {
                unlockedCount++;
            }
        }

        pageCount = Mathf.Max(1, (unlockedCount + SlotsPerPage - 1) / SlotsPerPage);
        currentPage = Mathf.Clamp(currentPage, 0, pageCount - 1);
        int pageStart = currentPage * SlotsPerPage;
        int unlockedIndex = 0;
        foreach (CGGallerySlot slot in slots)
        {
            if (slot == null)
                continue;

            bool unlocked = slot.playbackType == playbackType && unlockedIds.Contains(slot.cgId);
            bool visible = unlocked && unlockedIndex >= pageStart && unlockedIndex < pageStart + SlotsPerPage;
            slot.gameObject.SetActive(visible);
            if (unlocked)
                unlockedIndex++;
        }

        if (currentSelectedSlot != null && !unlockedIds.Contains(currentSelectedSlot.cgId))
            ClearPreview();

        pageLabel.text = $"{currentPage + 1}/{pageCount}";
        pageButton.interactable = pageCount > 1;
    }

    public void NextPage()
    {
        if (pageCount <= 1)
            return;

        currentPage = (currentPage + 1) % pageCount;
        RefreshGallerySlots();
    }

    /// <summary>
    /// 初始没有选择任何CG
    /// </summary>
    public void ClearSelection()
    {
        isPlayingCG = false;

        galleryUI.SetActive(true);

        if (playbackUI != null)
        {
            playbackUI.SetActive(false);
        }

        ClearPreview();
    }

    private void ClearPreview()
    {
        currentSelectedSlot = null;

        //大图显示纯黑
        largePreviewImage.sprite = null;
        largePreviewImage.color = Color.black;
        largePreviewImage.enabled = true;

        //没有选择时隐藏Play
        playButton.SetActive(false);

        //取消全部红色选中标记
        foreach (CGGallerySlot slot in slots)
        {
            if (slot != null)
            {
                slot.SetSelected(false);
            }
        }
    }


    /// <summary>
    /// 点击下面的胶卷图片
    /// 只负责选择，不直接播放
    /// </summary>
    public void SelectCG(CGGallerySlot slot)
    {
        if (slot == null || !slot.gameObject.activeInHierarchy)
            return;

        currentSelectedSlot = slot;

        //显示对应的大图
        largePreviewImage.sprite = slot.previewSprite;
        largePreviewImage.color = Color.white;
        largePreviewImage.enabled =
            slot.previewSprite != null;

        //有CG选中后才显示Play
        playButton.SetActive(
            slot.previewSprite != null
        );

        //刷新当前选中标记
        foreach (CGGallerySlot gallerySlot in slots)
        {
            if (gallerySlot != null)
            {
                gallerySlot.SetSelected(
                    gallerySlot == currentSelectedSlot
                );
            }
        }

        //Debug.Log("当前选中CG：" + slot.cgId);
    }


    /// <summary>
    /// 点击Play按钮
    /// </summary>
    /// 

    public GameObject Toilet;

    public void PlayCurrentCG()
    {
        if (currentSelectedSlot == null)
        {
            Debug.LogWarning("当前没有选中CG");
            return;
        }

        switch (currentSelectedSlot.playbackType)
        {
            //当前CG场景内播放RBQ
            case CGGallerySlot.CGPlaybackType.RBQ:

                isPlayingCG = true;

                galleryUI.SetActive(false);

                if (playbackUI != null)
                {
                    playbackUI.SetActive(true);
                }

                rbqController.PlayGalleryCG(
                    currentSelectedSlot.animationType
                );

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(
                    playbackUI
                );


                //一定先切换
                isCG = false;
                EndResult.SetActive(true);
                CG.SetActive(false);


                //特殊背景厕所
                if (currentSelectedSlot.animationType==2) 
                {
                    Toilet.SetActive(true);
                }
                else
                {
                    Toilet.SetActive(false);
                }


                break;


            //跳转Spine场景播放剧情CG
            case CGGallerySlot.CGPlaybackType.Spine:

                ToCG(currentSelectedSlot.cgId);

                break;
        }

      



    }


    public bool isCG = false;
    public GameObject CG;
    public GameObject EndResult;
    public void CG_EndResult() 
    {
        if (isCG)
        {
            EndResult.SetActive(true);
            CG.SetActive(false);
        }
        else
        {
            EndResult.SetActive(false);
            CG.SetActive(true);
        }

        isCG = !isCG;
        currentPage = 0;
        ClearPreview();
        RefreshGallerySlots();

    }//切换CG和EndResult


    /// <summary>
    /// 多端输入返回
    /// </summary>
    #region
    [Header("CG播放界面")]
    public BlackScreen blackScreen;
    public void ExitPlayback()
    {
        if (!isPlayingCG)
            return;

        isPlayingCG = false;

        //强制停止并归零RBQ动画
        rbqController.ResetGalleryCG();

        //隐藏播放界面的Back
        if (playbackUI != null)
        {
            playbackUI.SetActive(false);
        }

        //重新显示CG选择界面
        galleryUI.SetActive(true);


        //缩略图在其他页时，焦点回到播放按钮。
        GameObject focusTarget = currentSelectedSlot != null && currentSelectedSlot.gameObject.activeInHierarchy
            ? currentSelectedSlot.gameObject
            : playButton;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(focusTarget);
    }// 从CG播放界面返回// 画面Back按钮和多端Cancel都调用这里

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (isPlayingCG)
        {
            ExitPlayback();
        }
    }//手柄B、Esc等多端返回输入


    public void ToCG(string Name)
    {

        GameFlowData.nextAreaId = Name;
        GameFlowData.returnPath = "cg";//在Menu那里写过了

        blackScreen.SetFadeIn();// 黑幕淡入
        Invoke(nameof(StartCG), 1f);

    }//跳转CG场景

    private void StartCG()
    {
        SceneManager.LoadScene("Spine");
    }


    public void BackToMenu()
    {
        blackScreen.SetFadeIn();// 黑幕淡入
        Invoke(nameof(StartMenu), 1f);
    }

    private void StartMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    #endregion




}
