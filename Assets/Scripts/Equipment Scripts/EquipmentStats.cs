using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStats : MonoBehaviour
{
    [Header("Equipment Data")]
    public EquipmentData equipmentData;

    public int count = 3;
    [Range(0f, 200f)] public float damage = 80f;
    public float areaOfInfluence = 5f;
    public float influenceForce = 10f;
    public float recoveryTime = 0.5f;
    public float throwForce;
    public float throwUpwardForce;
    public bool interactionEnabled = true;
    public Transform attackPoint;
    public GameObject explosionEffect;
    public bool dealDamage = true;

    [Space]
    [Header("Positional Equipment Sway")]
    public float swayIntensity = 0.05f;
    public float maxSwayIntensity = 0.15f;
    public float smoothness = 5f;
    public float aimSwayIntensity = 0.005f;

    [Space]
    [Header("Rotational Equipment Sway")]
    public float rotSwayIntensity = 1f;
    public float maxRotSwayIntensity = 2f;
    public float rotSmoothness = 5f;
    public float aimRotSwayIntensity = 0.05f;

    [Space]
    [Header("Equipment Camera Recoil Stats")]
    public float verticalRecoil = 1f;
    public float horizontalRecoil = 0.2f;
    public float sphericalShake = 0.8f;
    public float positionRecoilRetaliation = 0.02f;
    public float positionRecoilVertical = 0.01f;
    public float transitionalSnappiness = 5f;
    public float recoilReturnSpeed = 8f;
    public float positionTransitionalSnappiness = 5f;
    public float positionRecoilReturnSpeed = 8f;

    [Space]
    [Header("Equipment Body Recoil Stats")]
    public float recoilX = 2f;
    public float recoilY = 2f;
    public float recoilZ = 4f;
    public float kickBackZ = 0.2f;
    public float equipmentSnappiness = 20f, equipmentReturnAmount = 24f;
    public float equipmentRotationSnappiness = 5f, equipmentRotationReturnAmount = 8f;

    [HideInInspector] public bool isWalking = false;
    [HideInInspector] public bool isSliding = false;
    [HideInInspector] public bool isHolding = false;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public string photonGameObjectString = "";
    [HideInInspector] public string effectString = "";

    public void InitializeEquipmentStats()
    {
        damage = equipmentData.damage;
        areaOfInfluence = equipmentData.areaOfInfluence;
        influenceForce = equipmentData.influenceForce;
        recoveryTime = equipmentData.recoveryTime;
        count = equipmentData.initialCount;
        photonGameObjectString = equipmentData.photonGameObjectString;
        effectString = equipmentData.effectString;
        dealDamage = equipmentData.dealDamage;
        throwForce = equipmentData.throwForce;
        throwUpwardForce = equipmentData.throwUpwardForce;
    }
}
