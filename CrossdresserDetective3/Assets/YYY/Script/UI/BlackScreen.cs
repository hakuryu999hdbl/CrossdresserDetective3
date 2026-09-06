using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{

    public Animator _BlackScreen;
    public Image BlackImage;
    public void SetFadeIn()
    {
        _BlackScreen.SetTrigger("FadeIn");
        BlackImage.raycastTarget = true;
    }
    public void SetFadeOut()
    {
        _BlackScreen.SetTrigger("FadeOut");
        BlackImage.raycastTarget = false;
    }
}
