using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyboxSample : MonoBehaviour
{
    public Material[] mats;
    public Camera mainCamera;

    void Awake()
    {

        //场景固定换，早晚随机
        //if (Random.Range(0, 2) == 0) { PlayerPrefs.SetInt("Time", 0); }
        //else { PlayerPrefs.SetInt("Time", 1); }

        //Black();
        //DayOrNight();
        //Night();

        GameManager.instance.IsSkyboxSample(this);

        Day();
    }

    void Update()
    {
        // 按 T 随机换装测试
        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            DayOrNight();
        }
    }


    public void DayOrNight()
    {

        mainCamera.clearFlags = CameraClearFlags.Skybox;//用天空盒

        if (Random.Range(0, 2) == 1)
        {

            Day();

        }
        else
        {
            Night();
        }
    }



    public Light dayLight;

    public int DayType;//0白天 1黑夜

    public void Day()
    {
        mainCamera.clearFlags = CameraClearFlags.Skybox; // 确保切换回来
        switch (Random.Range(0, 4))
        {
            case 0:
                RenderSettings.skybox = mats[0];
                break;
            case 1:
                RenderSettings.skybox = mats[2];
                break;
            case 2:
                RenderSettings.skybox = mats[3];
                break;
            case 3:
                RenderSettings.skybox = mats[4];
                break;
        }

        dayLight.intensity = 0.8f;

        DayType = 0;
    }

    public void Night()
    {
        mainCamera.clearFlags = CameraClearFlags.Skybox; // 确保切换回来
        RenderSettings.skybox = mats[1];

        //dayLight.intensity = 0.4f;
        dayLight.intensity = 0.1f;//夜晚直接去掉直照光

        DayType = 1;
    }

    public void RedSky()
    {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;//用纯色
        mainCamera.backgroundColor = Color.red;
        dayLight.intensity = 0.2f;
    }

    public void WhiteSky()
    {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;//用纯色
        mainCamera.backgroundColor = Color.white;
        dayLight.intensity = 0.2f;
    }

    public void Black()
    {
        mainCamera.clearFlags = CameraClearFlags.SolidColor;//用纯色
        mainCamera.backgroundColor = Color.black;
        dayLight.intensity = 1f;
    }
}
