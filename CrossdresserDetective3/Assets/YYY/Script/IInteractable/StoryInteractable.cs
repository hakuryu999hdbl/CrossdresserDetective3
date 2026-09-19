using UnityEngine;

public class StoryInteractable : MonoBehaviour, IInteractable
{
    [Header("¿¿½E±ÏÔÊ¾µÄÎÄ×ÖÎ")]
    public GameObject textTip;

    [Header("°´ÏÂEºó¿ªÆôµÄ¹ı³¡Åö×²Ì")]
    public GameObject cutsceneTrigger;

    [Header("ÊÇ·ñÖ»ÄÜ´¥·¢Ò»´Î")]
    public bool triggerOnce = true;

    private bool isTriggered;

    private void Start()
    {
        if (textTip != null)
            textTip.SetActive(false);

        if (cutsceneTrigger != null)
            cutsceneTrigger.SetActive(false);
    }

    public void TriggerAction()
    {
        if (triggerOnce && isTriggered)
            return;

        isTriggered = true;

        if (textTip != null)
            textTip.SetActive(false);

        if (cutsceneTrigger != null)
            cutsceneTrigger.SetActive(true);

        if (triggerOnce)
            gameObject.tag = "Untagged";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered)
            return;

        if (other.GetComponent<PlayerController>() != null)
        {
            if (textTip != null)
                textTip.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (textTip != null)
                textTip.SetActive(false);
        }
    }
}