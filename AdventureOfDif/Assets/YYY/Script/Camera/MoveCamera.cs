using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform CameraPoint; // Player中的CameraPoint
    public float followSpeed = 5f;
    void Start()
    {
        if (CameraPoint != null)
        {
            transform.position = new Vector3(CameraPoint.position.x, CameraPoint.position.y, transform.position.z);
        }
    }
    void LateUpdate()
    {
        if (CameraPoint != null)
        {
            Vector3 targetPosition = new Vector3(CameraPoint.position.x, CameraPoint.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
