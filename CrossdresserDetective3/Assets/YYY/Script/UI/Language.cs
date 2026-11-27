using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Language : MonoBehaviour
{

    Text Text;


    public string J;
    public string C_1;
    public string C_2;
    public string E;
    public string K;
    void OnEnable()
    {
        Text = GetComponent<Text>();
        Refresh();
    }

    public void Refresh()
    {

        string fullText = ResolveText(CurrentIndex, J, C_1, C_2, E, K);

        if (isTyped)
        {
            StartCoroutine(ShowText(fullText));  // 开始逐字显示文本
        }
        else
        {
            Text.text = fullText.ToString();//直接显示
        }

    }

    internal static int CurrentIndex => PlayerPrefs.GetInt("language");

    internal static string ResolveText(int index, string japanese, string simplifiedChinese,
        string traditionalChinese, string english, string korean)
    {
        return index switch
        {
            0 => japanese,
            1 => simplifiedChinese,
            2 => traditionalChinese,
            4 => korean,
            _ => english
        };
    }

    public bool isTyped = false;//是否需要这种打字效果
    float delay = 0.05f;   // 每个字的显示延迟时间

    IEnumerator ShowText(string textToShow)
    {
        Text.text = "";  // 先清空文本
        // 逐字显示
        for (int i = 0; i <= textToShow.Length; i++)
        {
            Text.text = textToShow.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
    }
}
