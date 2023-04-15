using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class MouseLookScript : MonoBehaviour
{

    [SerializeField] PlayerControllerManager player;
    private Vector2 MouseInput;
    public float mouseX;
    public float mouseY;
    public float mouseSensitivityValve;
    private float regularSensitivity;
    private float aimingSensitivity;
    public Camera itemLayerCamera;
    public Camera playerMainCamera;
    public Camera minimapCamera;
    [SerializeField] GameObject cameraHolder;
    //float xRot = 0f;

    void Start()
    {
        if (!player.pv.IsMine)
        {
            Destroy(minimapCamera.gameObject);
            Destroy(itemLayerCamera.gameObject);
            return;
        }
        else
        {
            playerMainCamera.fieldOfView = player.stats.cameraFieldOfView;
            Cursor.lockState = CursorLockMode.Locked;
            mouseSensitivityValve = player.stats.mouseSensitivity;
            mouseX = 0f;
            mouseY = 0f;
            ResetAimingSensitivity(mouseSensitivityValve);
            temp = new Vector3(transform.localPosition.x, 1.461f, transform.localPosition.z);
        }
    }
    public void ResetAimingSensitivity(float sensitivity)
    {
        regularSensitivity = sensitivity;
        aimingSensitivity = sensitivity / 1.5f;
    }
    public void SetPlayerFOV(float fov)
    {
        playerMainCamera.fieldOfView = fov;
        player.stats.cameraFieldOfView = fov;
        //PlayerPrefs.
    }
    Vector3 temp;
    void Update()
    {
        if (!player.pv.IsMine)
        {
            if (player.stats.isCrouching) transform.localPosition = new Vector3(transform.localPosition.x, 1.1f, transform.localPosition.z);
            else transform.localPosition = new Vector3(transform.localPosition.x, 1.461f, transform.localPosition.z);
        }
        if (!player.pv.IsMine) return;
        if (!player.stats.mouseMovementEnabled) return;
        CameraInput();
        CameraMovement();
        transform.localPosition = Vector3.Lerp(transform.localPosition, temp, Time.deltaTime * 8);
    }
    public void SetPlayerVerticalPosition(float value)
    {
        temp = new Vector3(transform.localPosition.x, value, transform.localPosition.z);
    }
    void CameraInput()
    {
        int sightIndex = player.holder.weaponIndex == 0 ? (int)player.pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 1)] : player.holder.weaponIndex == 1 ? (int)player.pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Sight, 2)] : -1;
        float multiplier = 1f;
        if (sightIndex != -1)
        {
            multiplier = (sightIndex == 1 ? 0.65f : sightIndex == 2 ? 0.45f : sightIndex == 3 ? 0.25f : 1f);
        }
        //Debug.Log("Multiplier: " + multiplier);
        mouseSensitivityValve = (player.stats.isAiming ? (aimingSensitivity * multiplier) : regularSensitivity);

        if (player.stats.mouseMovementEnabled)
        {
            mouseX = (Input.GetAxisRaw("Mouse X") + ((Input.GetKey("left") ? -1f : 0f) + (Input.GetKey("right") ? 1f : 0f))) * mouseSensitivityValve * Time.deltaTime;
            //Debug.Log(mouseX);
            mouseY += (Input.GetAxisRaw("Mouse Y") + ((Input.GetKey("down") ? -1f : 0f) + (Input.GetKey("up") ? 1f : 0f))) * mouseSensitivityValve * Time.deltaTime;
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
