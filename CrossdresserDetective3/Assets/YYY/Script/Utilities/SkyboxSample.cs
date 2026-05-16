using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Day();
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

        dayLight.intensity = 1f;
    }

    public void Night()
    {
        mainCamera.clearFlags = CameraClearFlags.Skybox; // 确保切换回来
        RenderSettings.skybox = mats[1];

        dayLight.intensity = 0.2f;
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
