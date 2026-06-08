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
    float normalSize = 6f;
    float zoomSize = 3f;
    float zoomSpeed = 8f;

    private bool isZoomIn;
    private float targetSize;









    void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();

    }

    private void Start()
    {
        //GetNewCameraBounds();//设置相机边界


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



  



    //设置相机边界
   private void GetNewCameraBounds()
   {
       var obj = GameObject.FindGameObjectWithTag("Bounds");
       if (obj == null) { return; }
   
       //设置边界
       confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
       confiner2D.InvalidateCache();
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
    }//接受力度
}
