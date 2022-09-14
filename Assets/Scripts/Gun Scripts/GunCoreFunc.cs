using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunCoreFunc : MonoBehaviour
{
    public GunManager gun;
    public GunStats stats;
    public GunAnimation anim;
    private float nextTimeToFire = 0f;
    [SerializeField] private float originFOV;
    [SerializeField] private float originMultiplierFOV;
    public float sightFOV;
    private Recoil recoilScript;
    public bool enableGunCoreFunc = true;
    private float timePassedUntillNextShot = 0f;
    [SerializeField] private List<QuantityStatsHUD.FireMode> fmList = new List<QuantityStatsHUD.FireMode>();
    [SerializeField] private int firemodeIndex = 0;
    //[SerializeField] PhotonView pv;

    private void Awake()
    {
        //pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        CoreInitialize();
    }
    public void CoreInitialize()
    {
        recoilScript = GetComponentInParent<Recoil>();
        SightFOVInitialize();
        FiremodeInitialize();
    }
    void SightFOVInitialize()
    {
        originFOV = gun.player.stats.cameraFieldOfView;
        originMultiplierFOV = originFOV / gun.stats.FOVMultiplier;
        sightFOV = originMultiplierFOV;
    }
    void FiremodeInitialize()
    {
        if (gun.stats.weaponData.enableAutomatic) stats.fireMode = QuantityStatsHUD.FireMode.Automatic;
        if (stats.weaponData.enableAutomatic) fmList.Add(QuantityStatsHUD.FireMode.Automatic);
        if (stats.weaponData.enableBurst) fmList.Add(QuantityStatsHUD.FireMode.Burst);
        if (stats.weaponData.enableSingle) fmList.Add(QuantityStatsHUD.FireMode.Single);
        if (stats.weaponData.isSniperProperties)
        {
            fmList.Clear();
            fmList.Add(QuantityStatsHUD.FireMode.SniperSingle);
        }
    }
    void StateInitalize()
    {
        gun.stats.isReloading = false;
        gun.animate.animate.SetBool("isReloading", false);
        gun.animate.animate.SetBool("isAiming", false);
        gun.animate.animate.SetBool("isSprinting", false);
    }
    private void OnEnable()
    {
        StateInitalize();
    }
    /*
    void Update()
    {
        //if (!IsOwner) return;
        

        #region Firemodes
        //Switching Firemodes
        #endregion

        
        #region GetGunPickup
        
        if (Input.GetKeyDown("f") && !gun.stats.isAttaching)
        {
            PickingGunUp();
        }
        #endregion

        #region AimingMechanics
        #endregion

        #region DropWeapon
        
        if (Input.GetKeyDown("g")) {
            SpawnPickup();
        }
        #endregion

        #region ReloadMechanics
        #endregion

        #region GunShooting
        //Shooting
        #endregion
    }*/
    public void AimingMechanics()
    {
        if (gun.stats.isAiming)
        {
            EnterAiming(true);
        }
        else
        {
            EnterAiming(false);
        }
    }
    public void FiremodeSwitchMechanics()
    {
        if (Input.GetKeyDown("b"))
        {
            ChangeFiremode();
        }
    }
    public void ReloadMechanics()
    {
        if (!gun.stats.autoReload)
        {
            if (Input.GetKeyDown("r") && !gun.stats.isReloading) StartCoroutine(Reload());
        }
        else
        {
            if (gun.stats.ammo <= 0 && !gun.stats.isReloading) StartCoroutine(Reload());
        }
    }
    private void RequestShootServerRpc(float range, float damage)
    {
        FireClientRpc(range, damage);
    }

    private void FireClientRpc(float range, float damage)
    {
        Shoot(range, damage);
    }

    public void ShootUnderRestriction()
    {
        nextTimeToFire = Time.time + 1f / gun.stats.fireRate;
        Shoot(gun.stats.range, gun.stats.damage);
        //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
    }
    public void ShootUnderFiremode()
    {
        if (!gun.stats.gunInteractionEnabled) return;
        if (stats.fireMode == QuantityStatsHUD.FireMode.Automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                ShootUnderRestriction();
            }
        }
        else if (stats.fireMode == QuantityStatsHUD.FireMode.SniperSingle)
        {
            ShootWithSniperSingle();
        }
        else if (stats.fireMode == QuantityStatsHUD.FireMode.Single)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                ShootUnderRestriction();
            }
        }
    }
    public void ShootWithSniperSingle()
    {
        if (stats.fireMode != QuantityStatsHUD.FireMode.SniperSingle) stats.fireMode = QuantityStatsHUD.FireMode.SniperSingle;
        if (timePassedUntillNextShot < gun.stats.boltRecoveryDuration) timePassedUntillNextShot += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
        {
            Shoot(gun.stats.range, gun.stats.damage);
            //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
            timePassedUntillNextShot = 0f;
        }
    }
    public void EnterAiming(bool check)
    {
        if (gun.fpsCam.playerMainCamera == null) return;
        if (check)
        {
            if (gun.stats.selectedSightIndex != 0) gun.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.fpsCam.playerMainCamera.fieldOfView, sightFOV, gun.stats.aimSpeed * Time.deltaTime);
            else gun.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.fpsCam.playerMainCamera.fieldOfView, originMultiplierFOV, gun.stats.aimSpeed * Time.deltaTime);
        }
        else
        {
            gun.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.fpsCam.playerMainCamera.fieldOfView, originFOV, gun.stats.aimSpeed * Time.deltaTime);
        }
    }
    public void ChangeFiremode()
    {
        firemodeIndex++;
        if (firemodeIndex >= fmList.Count) firemodeIndex = 0;
        stats.fireMode = fmList[firemodeIndex];
    }
    void PickingGunUp()
    {
        gun.GetPickupsForGun();
    }
    public void TriggerCameraRecoil(float verticalRecoil, float horizontalRecoil, float sphericalShake, float positionRecoilRetaliation, float positionRecoilVertical, float positionTransitionalSnappiness, float positionRecoilReturnSpeed, float transitionalSnappiness, float recoilReturnSpeed)
    {
        gun.camRecoil.RecoilFire(verticalRecoil, horizontalRecoil, sphericalShake, positionRecoilRetaliation, positionRecoilVertical, positionTransitionalSnappiness, positionRecoilReturnSpeed, transitionalSnappiness, recoilReturnSpeed);
    }

    void Shoot(float range, float damage)
    {
        if(gun.stats.ammo <= 0 || stats.isSprinting) return;
        if(gun.shellEject != null) gun.shellEject.GetComponent<ParticleSystem>().Play();
        anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        TriggerCameraRecoil(stats.verticalRecoil, stats.horizontalRecoil, stats.sphericalShake, stats.positionRecoilRetaliation, stats.positionRecoilVertical, stats.positionTransitionalSnappiness, stats.positionRecoilReturnSpeed, stats.transitionalSnappiness, stats.recoilReturnSpeed);

        anim.animate.SetTrigger("isFiring");
        gun.stats.ammo--;
        gun.player.InvokeGunEffects();
        RaycastHit hit;
        Ray ray = gun.fpsCam.playerMainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = gun.fpsCam.transform.position;
        if (Physics.Raycast(ray, out hit, range)){
            //Debug.Log(hit.transform.name);
            //IDamagable player = hit.transform.GetComponent<IDamagable>();
            if (hit.collider.gameObject.GetComponent<IDamagable>() != null)
            {
                bool hitmarkerFlag = false;
                hitmarkerFlag = (bool)hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage, false);
                if (!hitmarkerFlag)
                {
                    gun.ui.ui.InvokeHitmarker(UIManager.HitmarkerType.Hitmarker);
                    gun.player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Hitmarker);
                }
                else if (hitmarkerFlag)
                {
                    gun.ui.ui.InvokeHitmarker(UIManager.HitmarkerType.Killmarker);
                    gun.player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Killmarker);
                }
            }
            else
            {
                gun.player.CallShootRPCDecals(hit);
            }
            /*
            if (hit.transform.GetComponent<Rigidbody>() != null){
                flag = true;
                hit.transform.GetComponent<Rigidbody>().AddForce(-hit.normal * gun.stats.impactForce);
            }
            if (!flag)
            {
                GameObject temp = Instantiate(gun.bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                if (hit.transform.GetComponent<Renderer>() == null)
                {
                    temp.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;
                }
                Destroy(temp, 2f);
            }*/
        }
    }

    public void UpdateSightFOV()
    {
        /*
        if (gun.stats.selectedSightIndex != 0)
        {
            int tmpSelectedSightIndex = gun.stats.selectedSightIndex;
            if (gun.attachment.sightAttachments[tmpSelectedSightIndex].GetComponent<AttachmentScript>().attachmentSO.changesCameraDefaultFOV)
            {
                sightFOV = gun.attachment.sightAttachments[tmpSelectedSightIndex].GetComponent<AttachmentScript>().attachmentSO.cameraFOVChangedMultiplier * originMultiplierFOV;
            }
            else
            {
                sightFOV = originMultiplierFOV - gun.attachment.sightAttachments[tmpSelectedSightIndex].GetComponent<AttachmentScript>().attachmentSO.cameraFOVChangedAmount;
            }
        }
        else
        {
            sightFOV = originMultiplierFOV;
        }*/
    }

    IEnumerator Reload()
    {
        int storeRes = 0;
        gun.stats.isReloading = true;
        Debug.Log("Reloading... ");
        anim.animate.Play("Reload"); 
        yield return new WaitForSeconds(gun.stats.reloadTime);

        gun.stats.ammoPool += gun.stats.ammo;
        if(gun.stats.ammoPool >= gun.stats.maxAmmo){
            storeRes = gun.stats.maxAmmo - gun.stats.ammo;
            for(int i = 1; i <= storeRes; i++){
                gun.stats.ammo++;
            }
            gun.stats.ammoPool -= gun.stats.maxAmmo;
        }else{
            gun.stats.ammo = 0;
            //gun.stats.ammo = gun.stats.ammoPool;
            for(int i = 1; i <= gun.stats.ammoPool; i++){
                gun.stats.ammo++;
            }
            gun.stats.ammoPool = 0;
        }
        gun.stats.isReloading = false;

    }

    public void SpawnPickup(){
        GameObject temp = Instantiate(gun.pickup, gameObject.transform.position, gameObject.transform.rotation);
        Debug.Log("Gun Dropped From Control ");
        temp.GetComponent<GunStats>().weaponData = gun.stats.weaponData;
        temp.GetComponent<GunStats>().damage = gun.stats.damage;
        temp.GetComponent<GunStats>().range = gun.stats.range;
        temp.GetComponent<GunStats>().impactForce = gun.stats.impactForce;
        temp.GetComponent<GunStats>().reloadTime = gun.stats.reloadTime;
        temp.GetComponent<GunStats>().throwForce = gun.stats.throwForce;
        temp.GetComponent<GunStats>().autoReload = gun.stats.autoReload;
        temp.GetComponent<GunStats>().aimSpeed = gun.stats.aimSpeed;
        temp.GetComponent<GunStats>().FOVMultiplier = gun.stats.FOVMultiplier;
        temp.GetComponent<GunStats>().ammo = gun.stats.ammo;
        temp.GetComponent<GunStats>().maxAmmo = gun.stats.maxAmmo;
        temp.GetComponent<GunStats>().ammoPool = gun.stats.ammoPool;
        temp.GetComponent<GunStats>().maxAmmoPool = gun.stats.maxAmmoPool;
        temp.GetComponent<GunStats>().fireRate = gun.stats.fireRate;
        temp.GetComponent<GunStats>().boltRecoveryDuration = gun.stats.boltRecoveryDuration;
        temp.GetComponent<GunStats>().swayIntensity = gun.stats.swayIntensity;
        temp.GetComponent<GunStats>().maxSwayIntensity = gun.stats.maxSwayIntensity;
        temp.GetComponent<GunStats>().smoothness = gun.stats.smoothness;
        temp.GetComponent<GunStats>().rotSwayIntensity = gun.stats.rotSwayIntensity;
        temp.GetComponent<GunStats>().maxRotSwayIntensity = gun.stats.maxRotSwayIntensity;
        temp.GetComponent<GunStats>().rotSmoothness = gun.stats.rotSmoothness;
        temp.GetComponent<GunStats>().verticalRecoil = gun.stats.verticalRecoil;
        temp.GetComponent<GunStats>().horizontalRecoil = gun.stats.horizontalRecoil;
        temp.GetComponent<GunStats>().sphericalShake = gun.stats.sphericalShake;
        temp.GetComponent<GunStats>().transitionalSnappiness = gun.stats.transitionalSnappiness;
        temp.GetComponent<GunStats>().recoilReturnSpeed = gun.stats.recoilReturnSpeed;
        temp.GetComponent<GunStats>().selectedBarrelIndex = gun.stats.selectedBarrelIndex;
        temp.GetComponent<GunStats>().selectedSightIndex = gun.stats.selectedSightIndex;
        temp.GetComponent<GunStats>().selectedUnderbarrelIndex = gun.stats.selectedUnderbarrelIndex;
        Debug.Log("Data Copied");
        //if(gun.holder.selectedWeapon == 1) gun.holder.selectedWeapon = 1;
        //gun.holder.SelectWeapon();
        gun.SelfDestruct();
    }
}
