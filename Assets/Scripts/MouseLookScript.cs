using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookScript : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    private Vector2 MouseInput;
    private float mouseX;
    private float mouseY;
    public float mouseSensitivityValve;
    [SerializeField] Camera itemLayerCamera;
    float xRot = 0f;

    void Start()
    {
        transform.GetComponent<Camera>().fieldOfView = player.stats.cameraFieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivityValve = player.stats.mouseSensitivity;
        mouseX = 0f;
        mouseY = 0f;
    }

    void Update()
    {
        CameraInput();
        CameraMovement();
    }
    void CameraInput()
    {
        if (player.stats.mouseMovementEnabled)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivityValve * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityValve * Time.deltaTime;
        }
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, player.stats.ClampCamRotX, player.stats.ClampCamRotZ);
    }
    void CameraMovement()
    {
        if (player.stats.mouseMovementEnabled)
        {
            transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
            player.transform.Rotate(Vector3.up * mouseX);
            //itemLayerCamera.transform.localRotation = transform.localRotation;
        }
    }
    void CameraRotationClamp(float y, float x, float z)
    {
        mouseY = Mathf.Clamp(y, x, z);
    }
}
