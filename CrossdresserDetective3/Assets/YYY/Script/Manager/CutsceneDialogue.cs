using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneDialogue : MonoBehaviour
{
    [Header("UI")]
    public Text textLabel;

    [Header("文字速度")]
    public float textSpeed = 0.04f;

    [Header("台词显示结束后保持时间")]
    public float showTime = 3f;


    private List<string> textList = new List<string>();

    private int currentIndex = 0;

    private Coroutine typingCoroutine;
    private Coroutine hideCoroutine;

    //private bool textFinished = true;


    /// <summary>
    /// UIManager告诉本脚本读取哪一段过场。
    /// 例如 Chapter_01_1
    /// </summary>
    public void LoadDialogue(string fileName)
    {
        // 清理上一段过场
        StopCurrentDialogue();

        currentIndex = 0;
        textList.Clear();


        GetLanguagePath(
            out string folder,
            out string prefix
        );


        string path =
            $"{folder}/{prefix}{fileName}";


        TextAsset textAsset =
            Resources.Load<TextAsset>(path);


        if (textAsset == null)
        {
            Debug.LogWarning(
                "找不到过场台词文件：" + path
            );

            return;
        }


        ReadText(textAsset);


        textLabel.text = "";
        textLabel.gameObject.SetActive(false);


        Debug.Log(
            $"读取过场台词：{path}，共 {textList.Count} 行"
        );
    }



    /// <summary>
    /// 获取语言文件夹以及文件前缀。
    /// </summary>
    private void GetLanguagePath(
        out string folder,
        out string prefix)
    {
        switch (PlayerPrefs.GetInt("language"))
        {
            default:
            case 0:
                folder = "TXT_Japanese";
                prefix = "J_";
                break;

            case 1:
                folder = "TXT_Simplified_Chinese";
                prefix = "C1_";
                break;

            case 2:
                folder = "TXT_Traditional_Chinese";
                prefix = "C2_";
                break;

            case 3:
                folder = "TXT_English";
                prefix = "E_";
                break;

            case 4:
                folder = "TXT_Korean";
                prefix = "K_";
                break;
        }
    }



    /// <summary>
    /// 读取TXT。
    /// 保持：
    ///
    /// YYY
    /// 台词
    /// Thief_A
    /// 台词
    ///
    /// 这种格式。
    /// </summary>
    private void ReadText(TextAsset file)
    {
        textList.Clear();


        string[] lines =
            file.text.Split('\n');


        foreach (string rawLine in lines)
        {
            string line =
                rawLine.Trim();


            if (string.IsNullOrEmpty(line))
                continue;


            textList.Add(line);
        }
    }



    /// <summary>
    /// Timeline帧事件调用。
    ///
    /// 每调用一次：
    /// 读取人物标记
    /// ↓
    /// 读取下一行台词
    /// ↓
    /// 逐字显示
    /// </summary>
    public void ShowNext()
    {
        if (textList.Count == 0)
        {
            Debug.LogWarning("当前没有读取过场台词。");
            return;
        }

        if (currentIndex >= textList.Count)
        {
            Debug.LogWarning("当前过场台词已经播放完毕。");
            return;
        }


        // =========================
        // 新台词来了
        // 上一句所有行为立即取消
        // =========================

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }


        // 直接清掉上一句
        textLabel.text = "";


        // =========================
        // 读取人物标记
        // =========================

        string speaker = textList[currentIndex];

        SetSpeakerStyle(speaker);

        currentIndex++;


        if (currentIndex >= textList.Count)
        {
            Debug.LogWarning(
                "人物标记后没有台词：" + speaker
            );

            return;
        }


        // =========================
        // 读取台词
        // =========================

        string dialogue = textList[currentIndex];

        currentIndex++;


        textLabel.gameObject.SetActive(true);


        // 立刻开始新一句
        typingCoroutine =
            StartCoroutine(
                SetTextUI(dialogue)
            );
    }



    /// <summary>
    /// 逐字显示。
    /// 参照原 DialogSystem。
    /// </summary>
    private IEnumerator SetTextUI(string dialogue)
    {
        textLabel.text = "";

        int letter = 0;

        while (letter < dialogue.Length)
        {
            textLabel.text += dialogue[letter];

            letter++;

            yield return new WaitForSeconds(textSpeed);
        }


        textLabel.text = dialogue;

        typingCoroutine = null;


        // 只是兜底：
        // 如果后面一直没有新的ShowNext，
        // 3秒后自己消失
        hideCoroutine =
            StartCoroutine(
                HideDelay()
            );
    }


    /// <summary>
    /// 人物标记。
    /// 参照原 DialogSystem。
    /// </summary>


    [Header("名称")]
    public GameObject YYY;
    public GameObject Thief_A;
    public GameObject Thief_B;

    private void SetSpeakerStyle(string speaker)
    {

        //通用隐藏
        YYY.SetActive(false);
        Thief_A.SetActive(false);
        Thief_B.SetActive(false);

        switch (speaker)
        {

            case "Man":

                textLabel.color = Color.white;
                break;


            case "YYY":
                YYY.SetActive(true);
                textLabel.color = Color.white;
                break;

            case "YYY_Red":
                YYY.SetActive(true);
                textLabel.color = Color.red;
                break;


            case "Girl":

                textLabel.color = Color.white;
                break;


            case "Thief_A":
                Thief_A.SetActive(true);
                textLabel.color =Color.white;

                break;


            case "Thief_B":
                Thief_B.SetActive(true);
                textLabel.color = Color.white;
                break;


            default:

                Debug.LogWarning(
                    "未知人物标记：" + speaker
                );

                textLabel.color = Color.white;

                break;
        }
    }



    /// <summary>
    /// 文字全部打完后保持一段时间再隐藏。
    /// </summary>
    private IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(
            showTime
        );


        textLabel.gameObject.SetActive(false);

        hideCoroutine = null;
    }



    /// <summary>
    /// 过场结束或者跳过。
    /// </summary>
    public void HideImmediately()
    {
        StopCurrentDialogue();

        textLabel.text = "";

        textLabel.gameObject.SetActive(false);
    }



    private void StopCurrentDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);

            typingCoroutine = null;
        }


        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);

            hideCoroutine = null;
        }


        //textFinished = true;
    }
}