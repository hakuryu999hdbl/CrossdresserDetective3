using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("玩家出生点")]
    public Transform playerSpawnPoints_investigate;
    public Transform playerSpawnPoints_rescue;
    public Transform playerSpawnPoints_escape; 
    public Transform playerSpawnPoints_eliminate;

    [Header("任务")]
    public GameObject investigateRoot;
    public GameObject rescueRoot;
    public GameObject escapeRoot;
    public GameObject eliminateRoot;


    private void Start()
    {
        GameManager.instance.IsRoomManager(this);
    }


    public void SetupInvestigate()
    {
        if (investigateRoot != null)
            investigateRoot.SetActive(true);

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
        if (escapeRoot != null)
            eliminateRoot.SetActive(true);

        if (playerSpawnPoints_eliminate != null)
            GameManager.instance.player.transform.position =
           playerSpawnPoints_eliminate.position;
    }

}
