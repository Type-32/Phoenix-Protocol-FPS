using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentAnimation : MonoBehaviour
{
    [Header("Global References")]
    public EquipmentManager equipment;
    public EquipmentStats stats;
    public Animator animate;
    public GameObject equipmentModel;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion originRotation;

    private Vector3 equipmentInitialPosition;
    private Quaternion equipmentInitialRotation;
    private Quaternion equipmentOriginRotation;

    private float mouseInputX;
    private float mouseInputY;

    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private Vector3 equipmentCurrentPosition;
    private Vector3 equipmentTargetPosition;
    private Vector3 equipmentCurrentRotation;
    private Vector3 equipmentTargetRotation;

    private float slideSwayIntensity = 0.1f;
    private float maxSlideSwayIntensity = 0.2f;
    private float slideRotSwayIntensity = 1f;
    private float maxSlideRotSwayIntensity = 2f;

    private float swayIntensityValve = 0f;
    private float maxSwayIntensityValve = 0f;
    private float rotSwayIntensityValve = 0f;
    private float maxRotSwayIntensityValve = 0f;

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 Rot;

    public float aimBobAmount = 0.015f;
    public float walkBobAmount = 0.1f;
    public float crouchBobAmount = 0.02f;
    public float sprintBobAmount = 0.15f;
    public float aimBobSpeed = 8f;
    public float walkBobSpeed = 14f;
    public float crouchBobSpeed = 7f;
    public float sprintBobSpeed = 20f;
    public float returnDuration = 5f;

    private float defaultYPos = 0f;
    private float defaultXPos = 0f;
    private float timer;
    /*
    private void Start()
    {
    }*/
    public void InitializeValues()
    {
        slideSwayIntensity = stats.swayIntensity * 1.5f;
        maxSlideSwayIntensity = stats.maxSwayIntensity * 1.5f;
        slideRotSwayIntensity = stats.rotSwayIntensity * 2f;
        maxSlideRotSwayIntensity = stats.maxRotSwayIntensity * 2f;
        initialRotation = originRotation = equipmentModel.transform.localRotation;
        initialPosition = targetPosition = equipmentModel.transform.localPosition;
        defaultXPos = equipmentModel.transform.localPosition.x;
        defaultYPos = equipmentModel.transform.localPosition.y;
    }
    /*
    void Update()
    {
    }*/
    public void CoreAnimations()
    {
        //if (equipment.ui.ui.openedOptions) return;
        WeaponBob();
        if (equipment.player.stats.isSliding || equipment.player.stats.isSprinting)
        {
            swayIntensityValve = Mathf.Lerp(swayIntensityValve, slideSwayIntensity, Time.deltaTime * 7f);
            maxSwayIntensityValve = Mathf.Lerp(maxSwayIntensityValve, maxSlideSwayIntensity, Time.deltaTime * 7f);
            rotSwayIntensityValve = Mathf.Lerp(rotSwayIntensityValve, slideRotSwayIntensity, Time.deltaTime * 7f);
            maxRotSwayIntensityValve = Mathf.Lerp(maxRotSwayIntensityValve, maxSlideRotSwayIntensity, Time.deltaTime * 7f);
        }
        else
        {
            swayIntensityValve = Mathf.Lerp(swayIntensityValve, (stats.swayIntensity), Time.deltaTime * 7f);
            maxSwayIntensityValve = Mathf.Lerp(maxSwayIntensityValve, stats.maxSwayIntensity, Time.deltaTime * 7f);
            rotSwayIntensityValve = Mathf.Lerp(rotSwayIntensityValve, (stats.rotSwayIntensity), Time.deltaTime * 7f);
            maxRotSwayIntensityValve = Mathf.Lerp(maxRotSwayIntensityValve, stats.maxRotSwayIntensity, Time.deltaTime * 7f);
        }
        CalculateSway();
        MoveSway();
        TiltSway();

        UpdateWeaponRotationRecoil();
        UpdateWeaponPositionRecoil();

        //UpdateSway();
        //animate.SetInteger("HasSightAttached", equipment.stats.selectedSightIndex);
        animate.SetBool("isSprinting", equipment.stats.isSprinting);
        animate.SetBool("isHolding", equipment.core.throwState);
        animate.SetBool("isSliding", equipment.player.stats.isSliding);
        //animate.SetBool("isSliding", equipment.player.stats.isSliding);
        //animate.SetBool("isAttaching", equipment.stats.isAttaching);
    }
    private void CalculateSway()
    {
        mouseInputX = -Input.GetAxis("Mouse X") - (Input.GetAxis("Horizontal") * 1.8f);
        mouseInputY = -Input.GetAxis("Mouse Y") + (equipment.player.body.velocity.y * 2f);
    }
    private void MoveSway()
    {
        float moveX = Mathf.Clamp(mouseInputX * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
        float moveY = Mathf.Clamp(mouseInputY * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
        Vector3 finalPosition = new Vector3(moveX, moveY, 0);
        equipmentModel.transform.localPosition = Vector3.Lerp(equipmentModel.transform.localPosition, finalPosition + initialPosition, Time.deltaTime * stats.smoothness);
    }
    private void TiltSway()
    {
        float tiltY = Mathf.Clamp(mouseInputX * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
        float tiltX = Mathf.Clamp(mouseInputY * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));
        equipmentModel.transform.localRotation = Quaternion.Slerp(equipmentModel.transform.localRotation, finalRotation * initialRotation, Time.deltaTime * stats.rotSmoothness);
    }
    private void UpdateSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //Calculate targeted Rotation
        Quaternion tempRotX = Quaternion.AngleAxis(stats.swayIntensity * mouseX, Vector3.up);
        Quaternion tempRotY = Quaternion.AngleAxis(stats.swayIntensity * mouseY, Vector3.right);
        Quaternion targetRotation = originRotation * tempRotX * tempRotY;

        equipmentModel.transform.localRotation = Quaternion.Lerp(equipmentModel.transform.localRotation, targetRotation, Time.deltaTime * stats.smoothness);
    }
    /*
    private void UpdateRecoil()
    {
        targetPosition = Vector3.Lerp(targetPosition, transform.localPosition, returnSpeed * Time.deltaTime);
        currentPosition = Vector3.Slerp(currentPosition, targetPosition, snappiness * Time.fixedDeltaTime);
        transform.localPosition = currentPosition;
    }
    */

    public void TriggerWeaponRecoil(float recoilX, float recoilY, float recoilZ, float kickBackZ)
    {
        equipmentTargetPosition -= new Vector3(0, 0, kickBackZ);
        equipmentTargetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
    public void UpdateWeaponRotationRecoil()
    {
        equipmentTargetRotation = Vector3.Lerp(equipmentTargetRotation, Vector3.zero, Time.deltaTime * stats.equipmentRotationReturnAmount);
        equipmentCurrentRotation = Vector3.Slerp(equipmentCurrentRotation, equipmentTargetRotation, Time.fixedDeltaTime * stats.equipmentRotationSnappiness);
    }
    public void UpdateWeaponPositionRecoil()
    {
        equipmentTargetPosition = Vector3.Lerp(equipmentTargetPosition, equipmentInitialPosition, Time.deltaTime * stats.equipmentReturnAmount);
        equipmentCurrentPosition = Vector3.Lerp(equipmentCurrentPosition, equipmentTargetPosition, Time.fixedDeltaTime * stats.equipmentSnappiness);
    }
    public void WeaponBob()
    {
        equipmentModel.transform.localPosition = new Vector3(Mathf.Lerp(equipmentModel.transform.localPosition.x, defaultXPos, Time.deltaTime * 2), Mathf.Lerp(equipmentModel.transform.localPosition.y, defaultYPos, Time.deltaTime * 2), equipmentModel.transform.localPosition.z + (-Input.GetAxis("Vertical") / 1000));
        if (!equipment.player.stats.onGround) return;
        if ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f))
        {
            float bobAmount = 1f;
            //defaultXPos + Mathf.Cos(timer) * (player.stats.isSprinting ? sprintBobAmount : walkBobAmount)
            timer += Time.deltaTime * (equipment.player.stats.isSprinting ? sprintBobSpeed : walkBobSpeed);
            equipmentModel.transform.localPosition = new Vector3(Mathf.Lerp(equipmentModel.transform.localPosition.x, defaultXPos + Mathf.Cos(timer / 2) * (equipment.player.stats.isSprinting ? sprintBobAmount : equipment.player.stats.isCrouching ? crouchBobAmount : walkBobAmount), Time.deltaTime * 8) * bobAmount, Mathf.Lerp(equipmentModel.transform.localPosition.y, defaultYPos + Mathf.Sin(timer) * (equipment.player.stats.isSprinting ? sprintBobAmount : equipment.player.stats.isCrouching ? crouchBobAmount : walkBobAmount), Time.deltaTime * 8) * bobAmount, equipmentModel.transform.localPosition.z);
        }
    }
}
