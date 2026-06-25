using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    private CinemachineVirtualCamera virtualCamera;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;



    [Header("Zoom")]
    float normalSize = 10f;
    float zoomSize = 2.5f;
    float zoomSpeed = 8f;

    public bool isZoomIn;
    private float targetSize;





    //为了放大后相机能只让它继续水平跟随角色
    private CinemachineFramingTransposer framing;


    void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
          framing = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

    }

    private void Start()
    {
       // GetNewCameraBounds();//设置相机边界


        //获取缩小放大距离
        if (virtualCamera != null)
        {
            normalSize = virtualCamera.m_Lens.OrthographicSize;
            targetSize = normalSize;
        }
    }
    private void Update()
    {
        if (virtualCamera == null) return;

        virtualCamera.m_Lens.OrthographicSize =
            Mathf.Lerp(
                virtualCamera.m_Lens.OrthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );
    }



    //放大缩小镜头
    public void EnableConfiner()
    {
        GetNewCameraBounds();

    }

    public void DisableConfiner()
    {
        if (confiner2D == null) return;

        confiner2D.m_BoundingShape2D = null;
        confiner2D.InvalidateCache();

    }

    public void ToggleZoom()
    {
        isZoomIn = !isZoomIn;

        targetSize = isZoomIn ? zoomSize : normalSize;

        if (isZoomIn)
        {
            DisableConfiner();
            UIManager.instance.HidePortrait();
        }
        else 
        {
            EnableConfiner();
            UIManager.instance.ShowPortrait();
        }
           
    }



    //设置相机边界
    private void GetNewCameraBounds() 
    {
        //var obj = GameObject.FindGameObjectWithTag("Bounds");
        //if(obj == null){ return; }
        //
        ////设置边界
        //confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        //confiner2D.InvalidateCache();
    }




    //相机抖动SO事件
    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
    }

    private void OnCameraShakeEvent(float force)
    {
        impulseSource.GenerateImpulse(force);
    }

}
