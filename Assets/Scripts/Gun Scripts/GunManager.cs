using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerManager player;
    public EquipmentHolder holder;
    public GunCoreFunc core;
    public GunStats stats;
    public GunUI ui;
    public GunAnimation animate;
    public GunAttachments attachment;
    public GunLogic logic;
    public new GunAudio audio;

    [Space]
    [Header("References")]
    public GameObject fpsCam;
    //public GadgetUsageScript gadgetFunc;
    public ParticleSystem muzzleFire;
    public GameObject shellEject;
    public Transform shellEjectPos;
    public GameObject bulletImpact;
    public GameObject bulletImpactBlood;
    public GameObject pickup;
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
        fpsCam = FindObjectOfType<MouseLookScript>().gameObject;
    }
    private void Awake()
    {
        fpsCam = FindObjectOfType<MouseLookScript>().gameObject;
        player = FindObjectOfType<PlayerManager>();
        camRecoil = FindObjectOfType<Recoil>();
    }
    void Start()
    {
        fpsCam = FindObjectOfType<MouseLookScript>().gameObject;
        player = FindObjectOfType<PlayerManager>();
        camRecoil = FindObjectOfType<Recoil>();
        //gadgetFunc = FindObjectOfType<GadgetUsageScript>();
        holder = FindObjectOfType<EquipmentHolder>();
    }
    private void Update()
    {
        if (ui.ui.openedLoadoutMenu)
        {
            stats.gunInteractionEnabled = false;
        }
        else if (ui.ui.openedOptions)
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
}
