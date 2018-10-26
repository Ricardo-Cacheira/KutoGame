using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow instance;
    private Camera myCamera;
    private Func<Vector3> GetCameraFollowPosition;
    private float size;

    public void Setup(float size, Func<Vector3> GetCameraFollowPosition) 
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
        Vector3 desiredPosition = GetCameraFollowPosition() + new Vector3(0,0,-10);
        transform.position = desiredPosition;
        myCamera.orthographicSize = size;
    }
}

