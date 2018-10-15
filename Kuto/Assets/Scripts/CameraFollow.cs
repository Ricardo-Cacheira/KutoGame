using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;

    private Camera camera;

    public float smoothSpeed = 0.125f;
    public float size;

    public float cameraMinimum = 2f;
    public float cameraMaximum = 7f;
    public float sensitivity = 5f;

    void Start()
    {
        camera = GetComponent<Camera>();

        size = 7f;
    }
 
    void Update () {
        float zoom = size;
        zoom -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        zoom = Mathf.Clamp(zoom, cameraMinimum, cameraMaximum);
        size = zoom;
    }
    
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + new Vector3(0,0,-10);

        transform.position = desiredPosition;

        camera.orthographicSize = size;
    }
}
