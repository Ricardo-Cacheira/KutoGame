using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow instance;
    private Camera myCamera;
    private Func<Vector3> GetCameraFollowPosition;
    private int size;
    public float PPU = 32; // pixels per unit

    public void Setup(int size, Func<Vector3> GetCameraFollowPosition) 
    {
        this.size = size;
        myCamera = transform.GetComponent<Camera>();
        SetGetCameraFollowPosition(GetCameraFollowPosition);
    }

    public void SetGetCameraFollowPosition(Func<Vector3> GetCameraFollowPosition) 
    {
        this.GetCameraFollowPosition = GetCameraFollowPosition;
    }
    void Update () 
    {
        HandleCameraMove();
    }

    private void HandleCameraMove() 
    {
        Vector3 cameraFollowPos = GetCameraFollowPosition();

        cameraFollowPos.z = transform.position.z;
        Vector3 cameraMoveDir = (cameraFollowPos - transform.position).normalized;
        float dist = Vector3.Distance(cameraFollowPos, transform.position);
        float cameraMoveSpeed = dist;
        cameraMoveSpeed *= 3; //check it

        if (dist > 0f) {
            Vector3 mainCameraNewPos = transform.position + (cameraMoveDir * cameraMoveSpeed) * Time.deltaTime;

            // Test Overshoot
            float distAfter = Vector3.Distance(cameraFollowPos, transform.position);
            if (distAfter > dist) 
            {
                // Overshot
                transform.position = cameraFollowPos;
            }

                transform.position = mainCameraNewPos;
        }
    } 
}

