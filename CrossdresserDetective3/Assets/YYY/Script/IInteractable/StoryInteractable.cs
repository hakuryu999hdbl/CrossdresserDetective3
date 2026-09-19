using UnityEngine;

public class StoryInteractable : MonoBehaviour, IInteractable
{
    [Header("靠近时显示的文字物体")]
    public GameObject textTip;

    [Header("按下E后开启的过场碰撞体")]
    public GameObject cutsceneTrigger;

    [Header("是否只能触发一次")]
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