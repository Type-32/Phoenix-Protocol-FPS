using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        originFOV = gun.player.stats.cameraFieldOfView;
        originMultiplierFOV = originFOV / gun.stats.FOVMultiplier;
        sightFOV = originMultiplierFOV;
        if (gun.stats.weaponData.enableAutomatic) stats.fireMode = QuantityStatsHUD.FireMode.Automatic;
        recoilScript = FindObjectOfType<Recoil>();

        if (stats.weaponData.enableAutomatic) fmList.Add(QuantityStatsHUD.FireMode.Automatic);
        if (stats.weaponData.enableBurst) fmList.Add(QuantityStatsHUD.FireMode.Burst);
        if (stats.weaponData.enableSingle) fmList.Add(QuantityStatsHUD.FireMode.Single);
        if (stats.weaponData.isSniperProperties)
        {
            fmList.Clear();
            fmList.Add(QuantityStatsHUD.FireMode.SniperSingle);
        }
    }
    void Awake()
    {
        //originFOV = gun.fpsCam.GetComponent<Camera>().fieldOfView;
        //originMultiplierFOV = originFOV / gun.stats.FOVMultiplier;
        //sightFOV = originMultiplierFOV;
    }

    private void OnEnable()
    {
        gun.stats.isReloading = false;
        gun.animate.animate.SetBool("isReloading", false);
        gun.animate.animate.SetBool("isAiming", false);
        gun.animate.animate.SetBool("isSprinting", false);
        //gun.animate.animate.SetBool("isSliding", false);
    }

    void Update()
    {
        if (!gun.stats.gunInteractionEnabled) return;

        #region Firemodes
        //Switching Firemodes
        if (Input.GetKeyDown("b"))
        {
            firemodeIndex++;
            if (firemodeIndex >= fmList.Count) firemodeIndex = 0;
            stats.fireMode = fmList[firemodeIndex];
        }
        #endregion

        if (!enableGunCoreFunc) return;
        #region GetGunPickup
        if (Input.GetKeyDown("f") && !gun.stats.isAttaching)
        {
            gun.GetPickupsForGun();
            if(gun.holder.equipmentInHolder.Count >= 2) gun.PickGun();
        }
        #endregion

        #region AimingMechanics
        if (gun.stats.isAiming)
        {
            if(gun.stats.selectedSightIndex != 0) gun.fpsCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gun.fpsCam.GetComponent<Camera>().fieldOfView, sightFOV, gun.stats.aimSpeed * Time.deltaTime);
            else gun.fpsCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gun.fpsCam.GetComponent<Camera>().fieldOfView, originMultiplierFOV, gun.stats.aimSpeed * Time.deltaTime);
        }
        else
        {
            gun.fpsCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(gun.fpsCam.GetComponent<Camera>().fieldOfView, originFOV, gun.stats.aimSpeed * Time.deltaTime);
        }
        #endregion

        #region DropWeapon
        if (Input.GetKeyDown("g")){
            SpawnPickup();
        }
        #endregion

        #region ReloadMechanics
        if (gun.stats.isReloading) return;
        if (!gun.stats.autoReload){
            if(Input.GetKeyDown("r") && !gun.stats.isReloading) StartCoroutine(Reload());
        }else{
            if(gun.stats.ammo <= 0 && !gun.stats.isReloading) StartCoroutine(Reload());
        }
        #endregion

        #region GunShooting
        //Shooting
        if (stats.fireMode == QuantityStatsHUD.FireMode.Automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / gun.stats.fireRate;
                Shoot();
            }
        }else if (stats.fireMode == QuantityStatsHUD.FireMode.SniperSingle)
        {
            if(stats.fireMode != QuantityStatsHUD.FireMode.SniperSingle) stats.fireMode = QuantityStatsHUD.FireMode.SniperSingle;
            if (timePassedUntillNextShot < gun.stats.boltRecoveryDuration) timePassedUntillNextShot += Time.deltaTime;
            if (Input.GetButtonDown("Fire1") && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
            {
                Shoot();
                timePassedUntillNextShot = 0f;
            }
        }else if(stats.fireMode == QuantityStatsHUD.FireMode.Single)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / gun.stats.fireRate;
                Shoot();
            }
        }
        #endregion
    }
    public void TriggerCameraRecoil(float verticalRecoil, float horizontalRecoil, float sphericalShake, float positionRecoilRetaliation, float positionRecoilVertical, float positionTransitionalSnappiness, float positionRecoilReturnSpeed, float transitionalSnappiness, float recoilReturnSpeed)
    {
        gun.camRecoil.RecoilFire(verticalRecoil, horizontalRecoil, sphericalShake, positionRecoilRetaliation, positionRecoilVertical, positionTransitionalSnappiness, positionRecoilReturnSpeed, transitionalSnappiness, recoilReturnSpeed);
    }
    void Shoot()
    {
        if(gun.stats.ammo <= 0 || stats.isSprinting) return;
        if(gun.shellEject != null){
            //GameObject cachedShell = Instantiate(gun.shellEject, gun.shellEjectPos.position, gun.shellEjectPos.rotation);
            //cachedShell.GetComponent<Rigidbody>().AddForce(transform.right * 1.1f, ForceMode.Impulse);
            gun.shellEject.GetComponent<ParticleSystem>().Play();
        }
        anim.TriggerWeaponRecoil(stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        TriggerCameraRecoil(stats.verticalRecoil, stats.horizontalRecoil, stats.sphericalShake, stats.positionRecoilRetaliation, stats.positionRecoilVertical, stats.positionTransitionalSnappiness, stats.positionRecoilReturnSpeed, stats.transitionalSnappiness, stats.recoilReturnSpeed);
        //gun.animate.TriggerWeaponRecoil();
        //anim.animate.Play("Fire");
        //if (gun.stats.selectedBarrelIndex != 0 && gun.attachment.barrelAttachments[gun.stats.selectedBarrelIndex].GetComponent<AttachmentScript>().attachmentSO.changesMuzzleSound) gun.gunFireSFXSource.clip = gun.attachment.barrelAttachments[gun.stats.selectedBarrelIndex].GetComponent<AttachmentScript>().attachmentSO.overrideMuzzleSound;
        //else gun.gunFireSFXSource.clip = gun.defaultFireSFXClip;
        gun.audio.PlayGunSound();
        gun.stats.ammo--;
        gun.muzzleFire.Play();
        RaycastHit hit;
        if(Physics.Raycast(gun.fpsCam.transform.position, gun.fpsCam.transform.forward, out hit, gun.stats.range)){
            bool flag = false;
            Debug.Log(hit.transform.name);
            PlayerManager player = hit.transform.GetComponent<PlayerManager>();
            if (player != null && player != gun.player)
            {
                player.TakeDamageFromPlayer(gun.stats.damage, false);

            }
            //Target target = hit.transform.GetComponent<Target>();
            //EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            //if (target != null || enemy != null) gun.ui.ui.EnforceHitmarker(0.5f);
            /*
            if(target != null){
                target.TakeDamage(gun.stats.damage);
            }
            if(enemy != null)
            {
                flag = true;
                gun.ui.ui.hitmarkerTimePassed = 0f;
                gun.ui.ui.anim.Play("Hitmarker");
                int xptmp = enemy.TakeDamage(gun.stats.damage);
                gun.player.totalDealtDamage += (int)gun.stats.damage;
                //gun.ui.ui.fileManager.JSONplayerdata.xp += xptmp;
                gun.player.totalGainedXP += xptmp;
                if (enemy.health <= 0)
                {
                    //gun.ui.ui.KilledEnemy();
                    gun.player.totalKilledEnemies++;
                }
                GameObject impactCache = Instantiate(gun.bulletImpactBlood, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactCache, 1f);
            }
            */
            if(hit.transform.GetComponent<Rigidbody>() != null){
                flag = true;
                hit.transform.GetComponent<Rigidbody>().AddForce(-hit.normal * gun.stats.impactForce);
            }
            if (!flag)
            {
                GameObject temp = Instantiate(gun.bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
                if (hit.transform.GetComponent<PlayerManager>() == null)
                {
                    temp.GetComponent<Renderer>().material = hit.transform.GetComponent<Renderer>().material;
                    
                }
                Destroy(temp, 2f);
            }
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
        gun.holder.equipmentInHolder.Remove(this.GetComponent<GunManager>());
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
