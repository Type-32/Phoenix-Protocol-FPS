using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MouseLookScript : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    private Vector2 MouseInput;
    private float mouseX;
    private float mouseY;
    public float mouseSensitivityValve;
    [SerializeField] Camera itemLayerCamera;
    public Camera playerMainCamera;
    public Camera minimapCamera;
    [SerializeField] GameObject cameraHolder;
    //float xRot = 0f;

    void Start()
    {
        if (!player.pv.IsMine)
        {
            minimapCamera.gameObject.SetActive(false);
            return;
        }
        else
        {
            playerMainCamera.fieldOfView = player.stats.cameraFieldOfView;
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivityValve = player.stats.mouseSensitivity;
            mouseX = 0f;
            mouseY = 0f;
        }
    }

    void Update()
    {
        if (!player.pv.IsMine) return;
        CameraInput();
        CameraMovement();
    }
    void CameraInput()
    {
        if (player.stats.mouseMovementEnabled)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivityValve * Time.deltaTime;
            mouseY += Input.GetAxisRaw("Mouse Y") * mouseSensitivityValve * Time.deltaTime;
        }
        mouseY = Mathf.Clamp(mouseY, player.stats.ClampCamRotX, player.stats.ClampCamRotZ);
        //xRot -= mouseY;
        //xRot = Mathf.Clamp(xRot, player.stats.ClampCamRotX, player.stats.ClampCamRotZ);
    }
    void CameraMovement()
    {
        if (player.stats.mouseMovementEnabled)
        {
            transform.localRotation = Quaternion.Euler(-mouseY, 0, 0);
            player.transform.Rotate(Vector3.up * mouseX);
            //itemLayerCamera.transform.localRotation = transform.localRotation;
        }
    }
    void CameraRotationClamp(float y, float x, float z)
    {
        mouseY = Mathf.Clamp(y, x, z);
    }
}
