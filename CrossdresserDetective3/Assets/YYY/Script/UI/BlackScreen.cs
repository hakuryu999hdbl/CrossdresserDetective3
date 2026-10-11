using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{

    public Animator _BlackScreen;
    public Image BlackImage;

    private void Awake()
    {
        // 暂停时也能正常播放黑幕动画
        _BlackScreen.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void SetFadeIn()
    {
        ClearTriggers();

        // 直接播放，不等待 Animator 判断过渡条件
        _BlackScreen.Play("Base Layer.FadeIn", 0, 0f);
        BlackImage.raycastTarget = true;
    }

    public void SetFadeOut()
    {
        ClearTriggers();

        _BlackScreen.Play("Base Layer.FadeOut", 0, 0f);
        BlackImage.raycastTarget = false;
    }

    private void ClearTriggers()
    {
        _BlackScreen.ResetTrigger("FadeIn");
        _BlackScreen.ResetTrigger("FadeOut");
    }
}
