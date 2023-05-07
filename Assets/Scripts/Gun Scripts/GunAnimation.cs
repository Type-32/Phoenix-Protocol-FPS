using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [Header("Global References")]
    public GunManager gun;
    public GunStats stats;
    public Animator animate;
    public GameObject gunModel;
    public GameObject gunRecoilModel;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;

    [Space]
    public bool enableAimValueInterpolation = false;
    public bool enableSprintValueInterpolation = false;
    public bool enableWalkValueInterpolation = false;
    public float aimIntDurationMultiplier = 2.5f, sprintIntDurationMultiplier = 2.5f, walkIntDurationMultiplier = 2f;
    public string aimValueKey = "aimingValue", sprintValueKey = "sprintValue", walkValueKey = "walkValue", movementHorizontalValueKey = "horizontalInput", movementVerticalValueKey = "verticalInput";
    [HideInInspector] public float aimInterpolation = 0f, sprintInterpolation = 0f, walkInterpolation = 0f;

    #region Private Variables
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Quaternion originRotation;

    private Vector3 gunInitialPosition;
    private Quaternion gunInitialRotation;
    private Quaternion gunOriginRotation;

    private float mouseInputX;
    private float mouseInputY;

    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private Vector3 gunCurrentPosition;
    private Vector3 gunTargetPosition;
    private Vector3 gunCurrentRotation;
    private Vector3 gunTargetRotation;

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
    float effectLayerValue = 0f;
    float targetEffectLayerValue = 0f;
    #endregion

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
        initialRotation = originRotation = gunModel.transform.localRotation;
        initialPosition = targetPosition = gunModel.transform.localPosition;
        defaultXPos = gunModel.transform.localPosition.x;
        defaultYPos = gunModel.transform.localPosition.y;
        gunInitialPosition = gunRecoilModel.transform.localPosition;
    }
    /*
    void Update()
    {
    }*/
    public void CoreAnimations()
    {
        //if (gun.ui.ui.openedOptions) return;
        WeaponBob();
        if (gun.player.stats.isSliding || gun.player.stats.isSprinting)
        {
            swayIntensityValve = Mathf.Lerp(swayIntensityValve, slideSwayIntensity, Time.deltaTime * 7f);
            maxSwayIntensityValve = Mathf.Lerp(maxSwayIntensityValve, maxSlideSwayIntensity, Time.deltaTime * 7f);
            rotSwayIntensityValve = Mathf.Lerp(rotSwayIntensityValve, slideRotSwayIntensity, Time.deltaTime * 7f);
            maxRotSwayIntensityValve = Mathf.Lerp(maxRotSwayIntensityValve, maxSlideRotSwayIntensity, Time.deltaTime * 7f);
        }
        else
        {
            swayIntensityValve = Mathf.Lerp(swayIntensityValve, (stats.isAiming ? stats.aimSwayIntensity : stats.swayIntensity), Time.deltaTime * 7f);
            maxSwayIntensityValve = Mathf.Lerp(maxSwayIntensityValve, stats.maxSwayIntensity, Time.deltaTime * 7f);
            rotSwayIntensityValve = Mathf.Lerp(rotSwayIntensityValve, (stats.isAiming ? stats.aimRotSwayIntensity : stats.rotSwayIntensity), Time.deltaTime * 7f);
            maxRotSwayIntensityValve = Mathf.Lerp(maxRotSwayIntensityValve, stats.maxRotSwayIntensity, Time.deltaTime * 7f);
        }
        CalculateSway();
        MoveSway();
        TiltSway();

        UpdateWeaponRotationRecoil();
        UpdateWeaponPositionRecoil();

        //UpdateSway();
        //animate.SetInteger("HasSightAttached", gun.stats.selectedSightIndex);
        animate.SetBool("isSprinting", gun.stats.isSprinting);
        animate.SetBool("isAiming", gun.stats.isAiming);
        animate.SetBool("isReloading", gun.stats.isReloading);
        animate.SetBool("isSliding", gun.player.stats.isSliding);
        
        //Aim Interpolation
        aimInterpolation = Mathf.SmoothStep(aimInterpolation, (stats.isAiming ? 1f : 0f), Time.deltaTime * stats.weaponData.aimSpeed * 2);
        animate.SetFloat(aimValueKey, aimInterpolation);
        aimInterpolation = animate.GetFloat(aimValueKey);
        
        if (enableSprintValueInterpolation)
        {
            //sprintInterpolation = Mathf.Lerp(sprintInterpolation, (stats.isSprinting ? 1f : 0f), Time.deltaTime * sprintIntDurationMultiplier);
            animate.SetFloat(sprintValueKey, Convert.ToSingle(gun.stats.isSprinting), 0.3f, Time.deltaTime * sprintIntDurationMultiplier);
        }
        if (enableWalkValueInterpolation)
        {
            //walkInterpolation = Mathf.Lerp(walkInterpolation, (stats.isWalking ? 1f : 0f), Time.deltaTime * walkIntDurationMultiplier);
            animate.SetFloat(walkValueKey, Convert.ToSingle(gun.stats.isWalking), 0.3f, Time.deltaTime);
            animate.SetFloat(movementHorizontalValueKey, Input.GetAxis("Horizontal"), 0.15f, Time.deltaTime);
            animate.SetFloat(movementVerticalValueKey, Input.GetAxis("Vertical"), 0.15f, Time.deltaTime);
        }
        if (gun.stats.isAiming) gun.attachment.CheckEnabledSightAimingPosition(gun.player.holder.weaponIndex);
        targetEffectLayerValue = Mathf.Lerp(targetEffectLayerValue, 0f, Time.deltaTime * 22f);
        effectLayerValue = Mathf.Lerp(effectLayerValue, targetEffectLayerValue, Time.deltaTime * 16f);
        //animate.SetBool("isSliding", gun.player.stats.isSliding);
        //animate.SetBool("isAttaching", gun.stats.isAttaching);
    }
    private void CalculateSway()
    {
        if (!gun.player.stats.mouseMovementEnabled) return;
        mouseInputX = -Input.GetAxis("Mouse X") - (gun.stats.isAiming ? Input.GetAxis("Horizontal") * 0.8f : Input.GetAxis("Horizontal") * 2.5f);
        mouseInputY = -Input.GetAxis("Mouse Y") + (gun.player.body.velocity.y * 4f);
    }
    private void MoveSway()
    {
        float moveX = Mathf.Clamp(mouseInputX * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
        float moveY = Mathf.Clamp(mouseInputY * swayIntensityValve, -maxSwayIntensityValve, maxSwayIntensityValve);
        Vector3 finalPosition = new Vector3(moveX, moveY, 0);
        gunModel.transform.localPosition = Vector3.Lerp(gunModel.transform.localPosition, finalPosition + initialPosition, Time.deltaTime * stats.smoothness);
    }
    private void TiltSway()
    {
        float tiltY = Mathf.Clamp(mouseInputX * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
        float tiltX = Mathf.Clamp(mouseInputY * rotSwayIntensityValve, -maxRotSwayIntensityValve, maxRotSwayIntensityValve);
        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));
        gunModel.transform.localRotation = Quaternion.Slerp(gunModel.transform.localRotation, finalRotation * initialRotation, Time.deltaTime * stats.rotSmoothness);
    }
    private void UpdateSway()
    {
        float mouseX = gun.player.fpsCam.mouseX;
        float mouseY = gun.player.fpsCam.mouseY;

        //Calculate targeted Rotation
        Quaternion tempRotX = Quaternion.AngleAxis(stats.swayIntensity * mouseX, Vector3.up);
        Quaternion tempRotY = Quaternion.AngleAxis(stats.swayIntensity * mouseY, Vector3.right);
        Quaternion targetRotation = originRotation * tempRotX * tempRotY;

        gunModel.transform.localRotation = Quaternion.Lerp(gunModel.transform.localRotation, targetRotation, Time.deltaTime * stats.smoothness);
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
        targetEffectLayerValue += kickBackZ;
        gun.player.playerGunLayerEffect.weight = Mathf.Clamp(effectLayerValue, 0f, 1f);
        gunTargetPosition -= new Vector3(0, 0, kickBackZ);
        gunTargetRotation += new Vector3(recoilX, UnityEngine.Random.Range(-recoilY, recoilY), UnityEngine.Random.Range(-recoilZ, recoilZ));
    }
    public void UpdateWeaponRotationRecoil()
    {
        gunTargetRotation = Vector3.Lerp(gunTargetRotation, Vector3.zero, Time.deltaTime * stats.gunRotationReturnAmount);
        gunCurrentRotation = Vector3.Slerp(gunCurrentRotation, gunTargetRotation, Time.fixedDeltaTime * stats.gunRotationSnappiness);
        gunRecoilModel.transform.localRotation = Quaternion.Euler(gunCurrentRotation);
    }
    public void UpdateWeaponPositionRecoil()
    {
        gunTargetPosition = Vector3.Lerp(gunTargetPosition, gunInitialPosition, Time.deltaTime * stats.gunReturnAmount);
        gunCurrentPosition = Vector3.Lerp(gunCurrentPosition, gunTargetPosition, Time.fixedDeltaTime * stats.gunSnappiness);
        gunRecoilModel.transform.localPosition = gunCurrentPosition;
    }
    public void WeaponBob()
    {
        gunModel.transform.localPosition = new Vector3(Mathf.Lerp(gunModel.transform.localPosition.x, defaultXPos, Time.deltaTime * 2), Mathf.Lerp(gunModel.transform.localPosition.y, defaultYPos, Time.deltaTime * 2), gunModel.transform.localPosition.z + (-Input.GetAxis("Vertical") / 1000));
        if (!gun.player.stats.onGround) return;
        if ((Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f))
        {
            float bobAmount = 1f;
            //defaultXPos + Mathf.Cos(timer) * (player.stats.isSprinting ? sprintBobAmount : walkBobAmount)
            timer += Time.deltaTime * (gun.player.stats.isSprinting ? 22f : gun.stats.isAiming ? 7f : 14f);
            gunModel.transform.localPosition = new Vector3(Mathf.Lerp(gunModel.transform.localPosition.x, defaultXPos + Mathf.Cos(timer / 2) * (gun.player.stats.isSprinting ? sprintBobAmount : gun.stats.isAiming ? aimBobAmount : gun.player.stats.isCrouching ? crouchBobAmount : walkBobAmount), Time.deltaTime * 8) * bobAmount, Mathf.Lerp(gunModel.transform.localPosition.y, defaultYPos + Mathf.Sin(timer) * (gun.player.stats.isSprinting ? sprintBobAmount : gun.stats.isAiming ? aimBobAmount : gun.player.stats.isCrouching ? crouchBobAmount : walkBobAmount), Time.deltaTime * 8) * bobAmount, gunModel.transform.localPosition.z);
        }
    }
}
