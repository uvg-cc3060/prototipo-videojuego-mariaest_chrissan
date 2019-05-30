using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public bool lockCursor;
    public float mouseSensitivity = 10;
    private Transform target;
    public float dstFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (pitch < 0)
        {
            dstFromTarget = 3 - (pitch*1.2f) / pitchMinMax.x;
        }
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * dstFromTarget;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.6f, transform.position.z);

        if (transform.position.y <= target.position.y)
        {
            transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);
        }

    }

    public void AssignTarget(Transform t)
    {
        target = t;
    }

}