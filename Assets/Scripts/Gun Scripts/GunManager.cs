using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunManager : Gun
{
    [Header("Scripts")]
    [HideInInspector] public PlayerControllerManager player;
    [HideInInspector] public EquipmentHolder holder;
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
    [HideInInspector] public MouseLookScript fpsCam;
    public ParticleSystem muzzleFire;
    public GameObject shellEject;
    [HideInInspector] public GameObject pickup;
    public GameObject gunVisual;
    //public GameObject handsVisual;
    //public GameObject thirdPersonHandsVisual;
    //public Joint rightHandEnd;
    //public Joint leftHandEnd;
    [HideInInspector] public Recoil camRecoil;

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
        //InitializeAwake();
    }
    void Start()
    {
        //InitializeStart();
        camRecoil = GetComponentInParent<Recoil>();
        holder = GetComponentInParent<EquipmentHolder>();
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
    public void SelfDestruct()
    {
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
            if (temp != null)
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
    public void SetFirstPersonViewHandsMaterial(Material mat)
    {
        /*
        MeshRenderer[] tmp = handsVisual.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].material = mat;
        }*/
    }
    public void SetThirdPersonViewHandsMaterial(Material mat)
    {
        /*
        MeshRenderer[] tmp = thirdPersonHandsVisual.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i].material = mat;
        }*/
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
        if (!gun.player.stats.isSliding) core.ShootUnderFiremode();
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
        muzzleFire.playOnAwake = false;
        FindingReferences();
    }
}
