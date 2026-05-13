using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    Transform cameraTransform;
    public float parallaxFactor = 0.5f;

    private Vector3 startCameraPos;
    private Vector3 startLayerPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        startCameraPos = cameraTransform.position;
        startLayerPos = transform.position;

        StartCoroutine(UpdateParallaxAfterCamera());
    }

    IEnumerator UpdateParallaxAfterCamera()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            float cameraDeltaX = cameraTransform.position.x - startCameraPos.x;

            transform.position = new Vector3(
                startLayerPos.x + cameraDeltaX * parallaxFactor,
                startLayerPos.y,
                startLayerPos.z
            );
        }
    }
}
