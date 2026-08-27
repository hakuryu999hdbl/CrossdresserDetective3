using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("playerSpawnPoints")]
    public Transform playerSpawnPoints_investigate;
    public Transform playerSpawnPoints_rescue;
    public Transform playerSpawnPoints_escape; 
    public Transform playerSpawnPoints_eliminate;

    [Header("Root")]
    public GameObject investigateRoot;
    public GameObject rescueRoot;
    public GameObject escapeRoot;
    public GameObject eliminateRoot;

    public GameObject investigateRoot_2;//一般来说显示，隐藏搜查任务电脑

    [Header("gameOverPoint")]
    public Transform gameOverPoint; // 调教房 / 拷问室

    private void Start()
    {
        GameManager.instance.IsRoomManager(this);
    }


    public void SetupInvestigate()
    {
        if (investigateRoot != null)
            investigateRoot.SetActive(true);

        if (investigateRoot_2 != null)
            investigateRoot_2.SetActive(false);//隐藏多余电脑

        if (playerSpawnPoints_investigate != null)
            GameManager.instance.player.transform.position =
           playerSpawnPoints_investigate.position;
    }

    public void SetupRescue()
    {
        if (rescueRoot != null)
            rescueRoot.SetActive(true);

        if (playerSpawnPoints_rescue != null)
            GameManager.instance.player.transform.position =
           playerSpawnPoints_rescue.position;
    }

    public void SetupEscape()
    {
        if (escapeRoot != null)
            escapeRoot.SetActive(true);

        if (playerSpawnPoints_escape != null)
            GameManager.instance.player.transform.position =
           playerSpawnPoints_escape.position;
    }


    public void SetupEliminate()
    {
        if (eliminateRoot != null)
            eliminateRoot.SetActive(true);

        if (playerSpawnPoints_eliminate != null)
            GameManager.instance.player.transform.position =
           playerSpawnPoints_eliminate.position;
    }

}
