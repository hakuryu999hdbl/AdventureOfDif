using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    Transform cameraTransform;
    public float parallaxFactor = 0.5f;  // 0 = 不动（远处），1 = 跟随摄像机完全同步（前景）
    private Vector3 previousCameraPosition;

    void Start()
    {

        cameraTransform = Camera.main.transform;

        previousCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - previousCameraPosition;
        transform.position += new Vector3(delta.x * parallaxFactor, 0, 0); // 只左右移动
        previousCameraPosition = cameraTransform.position;
    }
}
