using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject newGameButton;


    
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(newGameButton);//开头设置默认按钮
    }

    public void NewGame()
    {
       
        SceneManager.LoadScene(0);

    }//跳转编号场景

    public void QuitGame()
    {

        Application.Quit();
    }
}
