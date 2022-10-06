using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunManager : Gun
{
    [Header("Scripts")]
    public PlayerControllerManager player;
    public EquipmentHolder holder;
    public GunCoreFunc core;
    public GunStats stats;
    public GunUI ui;
    public GunAnimation animate;
    public GunAttachments attachment;
    public GunLogic logic;
    public new GunAudio audio;
    //public PhotonView gunPV;

    [Space]
    [Header("References")]
    public MouseLookScript fpsCam;
    //public GadgetUsageScript gadgetFunc;
    public ParticleSystem muzzleFire;
    public GameObject shellEject;
    public Transform shellEjectPos;
    public GameObject bulletImpact;
    public GameObject bulletImpactBlood;
    public GameObject pickup;
    public GameObject gunVisual;
    public GameObject handsVisual;
    public Recoil camRecoil;

    [Space]
    [Header("Weapon Body")]
    public GameObject swayModel;
    public GameObject recoilModel;

    [Space]
    [Header("Audio Sources")]
    public AudioSource fireSoundSource;
    public AudioSource mechSoundSource;
    public AudioSource bassSoundSource;

    private void OnEnable()
    {
        fpsCam = GetComponentInParent<MouseLookScript>();
    }
    private void Awake()
    {
        //gunPV = GetComponent<PhotonView>();
        //InitializeAwake();
        //player = FindObjectOfType<PlayerControllerManager>();
    }
    void Start()
    {
        //InitializeStart();
        //fpsCam = FindObjectOfType<MouseLookScript>();
        //player = FindObjectOfType<PlayerControllerManager>();
        camRecoil = GetComponentInParent<Recoil>();
        //gadgetFunc = FindObjectOfType<GadgetUsageScript>();
        holder = GetComponentInParent<EquipmentHolder>();
        if (!player.pv.IsMine)
        {
            Destroy(handsVisual);
        }
    }
    /*
    private void Update()
    {
        if (!player.pv.IsMine) return;

    }*/
    public void DeterminatesFunction()
    {
        if (ui.ui.player.playerManager.openedLoadoutMenu)
        {
            stats.gunInteractionEnabled = false;
        }
        else if (ui.ui.player.playerManager.openedOptions)
        {
            stats.gunInteractionEnabled = false;
        }
        else
        {
            stats.gunInteractionEnabled = true;
        }
    }
    public void SelfDestruct(){
        //ui.ui.currentAmmo.text = "??";
        //ui.ui.ammoPool.text = "??";
        //ui.ui.fireModeIndicator.text = "----";
        Debug.Log("Gun Destructed ");
        Destroy(gameObject);
    }
    public void GetPickupsForGun()
    {
        /*
        RaycastHit detectRay;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out detectRay, 3f))
        {
            PickupScript tempPickup = detectRay.transform.GetComponent<PickupScript>();
            if (tempPickup != null)
            {
                if (tempPickup.pickupData.pickupFunctionType == PickupsSO.PickupFunctions.AmmoSupply)
                {
                    if (tempPickup.pickupData.pickupName != "Small Supply Pack") gadgetFunc.ammoPack.GetComponent<Animator>().Play("Enerplasm Pack");
                    else gadgetFunc.smallAmmoSupplyPack.GetComponent<Animator>().Play("SmallPackUse");
                    stats.ammoPool += tempPickup.pickupData.ammoSupplyAmount;
                    Destroy(tempPickup.gameObject);
                }
                if (tempPickup.pickupData.pickupFunctionType == PickupsSO.PickupFunctions.GadgetSupply)
                {
                    gadgetFunc.gadgetAmount += tempPickup.pickupData.gadgetSupplyAmount;
                    Destroy(tempPickup.gameObject);
                }
            }
        }
        */
    }
    public void PickGun()
    {
        RaycastHit detect;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out detect, 3f))
        {
            GunPickup temp = detect.transform.GetComponent<GunPickup>();
            if(temp != null)
            {
                GameObject transferredTemp = Instantiate(temp.mainGunObject, holder.transform);
                GunStats tempStats = transferredTemp.GetComponent<GunStats>();
                tempStats = temp.GetComponent<GunStats>();
                Destroy(temp.gameObject);
                core.SpawnPickup();
                SelfDestruct();
            }
        }
    }
    public override void Use()
    {
        DeterminatesFunction();
        ui.UIFunctions();
        if (!stats.gunInteractionEnabled) return;
        animate.CoreAnimations();
        logic.GunGeneralLogic();
        if (!core.enableGunCoreFunc) return;
        //Debug.Log("Using Weapon " + stats.weaponData.itemName);
        core.FiremodeSwitchMechanics();
        core.AimingMechanics();

        if (gun.stats.isReloading) return;
        core.ReloadMechanics();
        core.ShootUnderFiremode();
    }
    void FindingReferences()
    {
        fpsCam = GetComponentInParent<MouseLookScript>();
        camRecoil = GetComponentInParent<Recoil>();
        ui.GunUIAwake();
    }
    public override void InitializeStart()
    {
        core.CoreInitialize();
        animate.InitializeValues();
    }
    public override void InitializeAwake()
    {
        FindingReferences();
    }
}
