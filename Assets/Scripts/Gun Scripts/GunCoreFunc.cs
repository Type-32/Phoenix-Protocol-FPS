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
        timePassedUntillNextShot = gun.stats.boltRecoveryDuration;
        gun.stats.autoReload = true;
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
        if (stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.SniperRifle || stats.weaponData.isSniperProperties)
        {
            fmList.Clear();
            fmList.Add(QuantityStatsHUD.FireMode.SniperSingle);
            stats.fireMode = QuantityStatsHUD.FireMode.SniperSingle;
        }
        if (stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.Shotgun)
        {
            fmList.Clear();
            fmList.Add(QuantityStatsHUD.FireMode.Shotgun);
            stats.fireMode = QuantityStatsHUD.FireMode.Shotgun;
        }
        stats.fireMode = fmList[0];
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
        if (!gun.player.pv.IsMine) return;

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
        if (Input.GetKeyDown("r") && !gun.stats.isReloading && gun.stats.ammo != gun.stats.weaponData.maxAmmoPerMag && gun.stats.ammoPool > 0) StartCoroutine(Reload());
        if (gun.stats.ammo <= 0 && gun.stats.ammoPool > 0 && !gun.stats.isReloading) StartCoroutine(Reload());
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
        else if (stats.fireMode == QuantityStatsHUD.FireMode.Shotgun)
        {
            ShootWithShotgunProperties();
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
    public void ShootWithShotgunProperties()
    {
        if (stats.fireMode != QuantityStatsHUD.FireMode.Shotgun) stats.fireMode = QuantityStatsHUD.FireMode.Shotgun;
        if (timePassedUntillNextShot < gun.stats.boltRecoveryDuration) timePassedUntillNextShot += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
        {
            if (gun.stats.ammo <= 0 || stats.isSprinting) return;
            for (int i = 0; i < gun.stats.weaponData.pelletsPerFire; i++) Shoot(gun.stats.range, gun.stats.weaponData.damagePerPellet);
            gun.stats.ammo--;
            if (gun.stats.weaponData.ejectCasingAfterRechamber) StartCoroutine(Rechamber());
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
        if(gun.shellEject != null && !gun.stats.weaponData.ejectCasingAfterRechamber) gun.shellEject.GetComponent<ParticleSystem>().Play();
        if (gun.stats.weaponData.weaponType == QuantityStatsHUD.WeaponType.SniperRifle) StartCoroutine(Rechamber());

        float decreasedKickback = 0f;
        if(gun.player.holder.weaponIndex == 0)
        {
            if((int)PhotonNetwork.LocalPlayer.CustomProperties["SMWA_UnderbarrelIndex1"] != -1)
            {
                float temp = stats.kickBackZ;
                decreasedKickback = temp - (temp * 0.4f);
                anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ - (stats.recoilZ * 0.35f), decreasedKickback);
            }else anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        }
        else
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties["SMWA_UnderbarrelIndex2"] != -1)
            {
                float temp = stats.kickBackZ;
                decreasedKickback = temp - (temp * 0.4f);
                anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ - (stats.recoilZ * 0.35f), decreasedKickback);
            }else anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        }
        TriggerCameraRecoil(stats.verticalRecoil, stats.horizontalRecoil, stats.sphericalShake, stats.positionRecoilRetaliation, stats.positionRecoilVertical, stats.positionTransitionalSnappiness, stats.positionRecoilReturnSpeed, stats.transitionalSnappiness, stats.recoilReturnSpeed);

        anim.animate.SetTrigger("isFiring");
        if(gun.stats.weaponData.weaponType != QuantityStatsHUD.WeaponType.Shotgun) gun.stats.ammo--;
        gun.player.InvokeGunEffects();
        RaycastHit hit;
        Ray ray = gun.fpsCam.playerMainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 shootDirection = gun.fpsCam.transform.forward;
        if (gun.stats.weaponData.hasHipfireInaccuracy)
        {
            float spreadX = 0f, spreadY = 0f;
            spreadX = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadY = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadX *= gun.stats.weaponData.hipfireSpread;
            spreadY *= gun.stats.weaponData.hipfireSpread;
            shootDirection.x += spreadX * 0.1f;
            shootDirection.y += spreadY * 0.1f;
            shootDirection.z += spreadX * 0.1f;
        }
        ray.origin = gun.fpsCam.transform.position;
        ray.direction = shootDirection;
        if (Physics.Raycast(ray, out hit, range)){
            //Debug.Log(hit.transform.name);
            //IDamagable player = hit.transform.GetComponent<IDamagable>();
            if (hit.collider.gameObject.GetComponent<IDamagable>() != null)
            {
                bool hitmarkerFlag = false;
                hitmarkerFlag = (bool)hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(damage, false, gun.player.transform.position, gun.player.transform.rotation);
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

    IEnumerator Rechamber()
    {
        //Debug.Log("Rechambering!");
        yield return new WaitForSeconds(gun.stats.weaponData.rechamberDelay);
        anim.animate.SetTrigger("rechamberAnim");
        gun.audio.PlayRechamberSound();
        StartCoroutine(PlayShellEject(gun.stats.weaponData.shellEjectionDelay));
    }
    IEnumerator PlayShellEject(float delay)
    {
        yield return new WaitForSeconds(delay);
        gun.shellEject.GetComponent<ParticleSystem>().Play();
    }
    IEnumerator Reload()
    {
        int storeRes = 0;
        gun.stats.isReloading = true;
        //Debug.Log("Reloading... ");
        //anim.animate.Play("Reload"); 
        float reloadTime = 0f;
        if (gun.stats.weaponData.reloadByBullet)
        {
            reloadTime = gun.stats.weaponData.reloadTimePerPellet;
        }
        else
        {
            reloadTime = gun.stats.weaponData.reloadTime;
        }
        yield return new WaitForSeconds(reloadTime);
        if (!gun.stats.weaponData.reloadByBullet)
        {
            gun.stats.ammoPool += gun.stats.ammo;
            if (gun.stats.ammoPool >= gun.stats.maxAmmo)
            {
                storeRes = gun.stats.maxAmmo - gun.stats.ammo;
                for (int i = 1; i <= storeRes; i++)
                {
                    gun.stats.ammo++;
                }
                gun.stats.ammoPool -= gun.stats.maxAmmo;
            }
            else
            {
                gun.stats.ammo = 0;
                //gun.stats.ammo = gun.stats.ammoPool;
                for (int i = 1; i <= gun.stats.ammoPool; i++)
                {
                    gun.stats.ammo++;
                }
                gun.stats.ammoPool = 0;
            }
            gun.stats.isReloading = false;
        }
        else
        {
            //gun.stats.isReloading = false;
            if (gun.stats.ammo < gun.stats.weaponData.maxAmmoPerMag && gun.stats.ammoPool > 0)
            {
                gun.stats.ammo++;
                gun.stats.ammoPool--;
                if (gun.stats.ammo < gun.stats.weaponData.maxAmmoPerMag && gun.stats.ammoPool > 0) StartCoroutine(Reload());
                else gun.stats.isReloading = false;
            }
            else gun.stats.isReloading = false;
        }
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
