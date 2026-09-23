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

        if (impulseSource == null) return;

        Vector3 shakeVelocity = new Vector3(1f, 1f, 0f).normalized * force;

        impulseSource.GenerateImpulseWithVelocity(shakeVelocity);

    }//接受力度












    //切换当前跟随相机对象
    public void SetFollowTarget(Transform target)
    {
        if (virtualCamera == null || target == null)
            return;

        virtualCamera.Follow = target;
    }

    public void FollowPlayer()
    {
        if (RoomGenerator.instance == null ||
            RoomGenerator.instance.player == null)
            return;

        virtualCamera.Follow =
            RoomGenerator.instance.player.transform;
    }



}
