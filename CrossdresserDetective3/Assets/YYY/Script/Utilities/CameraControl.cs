using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void Start()
    {
        GetNewCameraBounds();
    }

    private void GetNewCameraBounds() 
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if(obj == null){ return; }

        //设置边界
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }


    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

}
