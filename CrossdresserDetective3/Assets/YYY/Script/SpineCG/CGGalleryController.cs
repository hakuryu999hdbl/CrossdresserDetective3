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

    [Header("CG播放界面")]
    public GameObject playbackUI;

    [Header("场景中的RBQ")]
    public RBQController rbqController;

    [Header("全部CG Slot")]
    public CGGallerySlot[] slots;

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
        //刚进入场景时没有任何CG被选中
        ClearSelection();
    }


    /// <summary>
    /// 初始没有选择任何CG
    /// </summary>
    public void ClearSelection()
    {
        currentSelectedSlot = null;
        isPlayingCG = false;

        galleryUI.SetActive(true);

        if (playbackUI != null)
        {
            playbackUI.SetActive(false);
        }

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
        if (slot == null)
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

        Debug.Log("当前选中CG：" + slot.cgId);
    }


    /// <summary>
    /// 点击Play按钮
    /// </summary>
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


        //当前选中移动到返回按钮
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentSelectedSlot.gameObject);
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