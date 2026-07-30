using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textLabel;

    private Dictionary<int, TextAsset> textAssets = new Dictionary<int, TextAsset>();


    public int index;
    public float textSpeed;
    bool textFinished;//是否完成打字
    bool cancelTyping;//取消打字
    List<string> textList = new List<string>();

   

    [Header("这是哪个动画需要的对话")]
    public int animation_number;

    [Header("Spine动画器总控制")]
    public Spine_FrameEvents spine_FrameEvents;


    private void OnEnable()
    {




        spine_FrameEvents.SetCurrentAnimator();

        Invoke("Read", 0.1f);

        

    }//一开始不会产生空白，OnEnable会在Start之前，Awake之后被调用





    public void ForceEndDialogue()
    {
        // 清除当前对话状态
        textList.Clear();
        index = 0;

        // 设置 textFinished 为 true，以便退出正在进行的协程
        textFinished = true;

        // 将对话系统 UI 隐藏
        gameObject.SetActive(false);

        //Debug.Log("对话已强制结束并重置");


    }//强制关闭对话

    void Read()
    {
        // Clear the existing dictionary to avoid key conflicts
        textAssets.Clear();

        switch (PlayerPrefs.GetInt("language"))
        {
            case 0:
                //剧情开头
                textAssets.Add(1, Resources.Load<TextAsset>("TXT_Japanese/J_CG_01"));

                break;
            case 1:
                //剧情开头
                textAssets.Add(1, Resources.Load<TextAsset>("TXT_Simplified_Chinese/C1_CG_01"));


                break;
            case 2:
                //剧情开头
                textAssets.Add(1, Resources.Load<TextAsset>("TXT_Traditional_Chinese/C2_CG_01"));


                break;
            case 3:
                //剧情开头
                textAssets.Add(1, Resources.Load<TextAsset>("TXT_English/E_CG_01"));




                break;
            case 4:
                //剧情开头
                textAssets.Add(1, Resources.Load<TextAsset>("TXT_Korean/K_CG_01"));


                break;
        }






        // 使用字典查找相应的 TextAsset
        if (textAssets.TryGetValue(animation_number, out TextAsset selectedText))
        {
            GetTextFormFile(selectedText);         
        }
        else
        {
            Debug.LogError("No TextAsset found for animation_number: " + animation_number);
        }

        textFinished = true;
        StartCoroutine(SetTextUI());
    }





    public void ShowText()
    {
        Debug.Log("点击下一句");

        if (textFinished && !cancelTyping)
        {
            if (index >= textList.Count) // 添加边界检查
            {
                gameObject.SetActive(false);
                index = 0;

                ChangeStory();//结束重刷场景

                Debug.Log("对话已结束");
                return;
            }

            if (gameObject.activeSelf)
            {
                StartCoroutine(SetTextUI());
            }
        }
        else if (!textFinished)
        {
            cancelTyping = !cancelTyping;
        }

    }//点击下一句

    void GetTextFormFile(TextAsset file)
    {
        textList.Clear(); index = 0;//首先将列表内的字符清空

        var lineDate = file.text.Split('\n');//以回车切割每一段

        foreach (var line in lineDate)
        {
            textList.Add(line);
        }
    }

    IEnumerator SetTextUI()
    {
        if (index >= textList.Count)
        {
            Debug.LogWarning("index 超出 textList 范围");
            yield break;
        }

        textFinished = false;
        textLabel.text = "";

        string line = textList[index].Trim();

        switch (line)
        {
            case "BG":
                textLabel.color = Color.white;
                index++;
                break;

            case "Man":
                textLabel.color = Color.blue;
                index++;
                break;

            case "YYY":
                textLabel.color = new Color(1.0f, 0.2f, 0.5f, 1.0f); //浅红色
                index++;
                break;

            case "Girl":
                textLabel.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色
                index++;
                break;


            case "--------------------NEXT--------------------":
                textLabel.color = Color.white;
                index++;
                //当前显示的Spine动画器里触发Next
                spine_FrameEvents.TriggerNext();
                break;
        }

      


        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textLabel.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }

        textLabel.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }

    public GameObject BlackScreen_FadeIn;
    //结束对话触发
    public void ChangeStory()
    {
        BlackScreen_FadeIn.SetActive(true);//黑幕淡出

        Invoke(nameof(BackToMenu), 0.95f);
    }


    private void BackToMenu()
    {

        SceneManager.LoadScene("Menu");
    }


}
