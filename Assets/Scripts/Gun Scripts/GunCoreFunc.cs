using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class GunCoreFunc : MonoBehaviour
{
    public GunManager gun;
    public GunStats stats;
    public GunAnimation anim;
    private float nextTimeToFire = 0f;
    [SerializeField] private float originFOV;
    [SerializeField] private float originMultiplierFOV;
    public float sightFOV;

    private float il_originFOV, il_originMultiplierFOV, il_sightFOV;

    private Recoil recoilScript;
    public bool enableGunCoreFunc = true;
    private float timePassedUntillNextShot = 0f;
    [SerializeField] private List<FireMode> fmList = new List<FireMode>();
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
        originMultiplierFOV = originFOV / gun.stats.weaponData.FOVMultiplier;
        sightFOV = originMultiplierFOV;
        il_originFOV = 60f;
        il_originMultiplierFOV = il_originFOV / gun.stats.weaponData.FOVMultiplier;
        il_sightFOV = il_originMultiplierFOV;
    }
    void FiremodeInitialize()
    {
        if (!gun.ModelMode)
        {
            if (gun.stats.weaponData.enableAutomatic) stats.fireMode = FireMode.Automatic;
            if (stats.weaponData.enableAutomatic) fmList.Add(FireMode.Automatic);
            if (stats.weaponData.enableBurst) fmList.Add(FireMode.Burst);
            if (stats.weaponData.enableSingle) fmList.Add(FireMode.Single);
            if (stats.weaponData.weaponType == WeaponType.SniperRifle || stats.weaponData.isSniperProperties)
            {
                fmList.Clear();
                fmList.Add(FireMode.SniperSingle);
                stats.fireMode = FireMode.SniperSingle;
            }
            if (stats.weaponData.weaponType == WeaponType.Shotgun)
            {
                fmList.Clear();
                fmList.Add(FireMode.Shotgun);
                stats.fireMode = FireMode.Shotgun;
            }
            stats.fireMode = fmList[0];
        }
        else
        {
            fmList.Clear();
            fmList.Add(FireMode.Single);
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
        if (!gun.player.pv.IsMine) return;

    }*/
    public void AimingMechanics()
    {
        EnterAiming(gun.stats.isAiming);
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

    public void ShootUnderRestriction()
    {
        nextTimeToFire = Time.time + 1f / gun.stats.fireRate;
        Shoot(gun.stats.range, gun.stats.damage);
        //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
    }
    public void ShootUnderFiremode()
    {
        if (!gun.stats.gunInteractionEnabled) return;
        if (stats.fireMode == FireMode.Automatic)
        {
            if ((Input.GetButton("Fire1") || (Input.GetKey(KeyCode.Slash))) && Time.time >= nextTimeToFire)
            {
                ShootUnderRestriction();
            }
        }
        else if (stats.fireMode == FireMode.SniperSingle)
        {
            ShootWithSniperSingle();
        }
        else if (stats.fireMode == FireMode.Shotgun)
        {
            ShootWithShotgunProperties();
        }
        else if (stats.fireMode == FireMode.Single)
        {
            if ((Input.GetButtonDown("Fire1") || (Input.GetKeyDown(KeyCode.Slash) || Input.GetKeyDown(KeyCode.Slash))) && Time.time >= nextTimeToFire)
            {
                ShootUnderRestriction();
            }
        }
    }
    public void ShootWithSniperSingle()
    {
        if (stats.fireMode != FireMode.SniperSingle) stats.fireMode = FireMode.SniperSingle;
        if (timePassedUntillNextShot < gun.stats.boltRecoveryDuration) timePassedUntillNextShot += Time.deltaTime;
        if (!gun.ModelMode)
        {
            if ((Input.GetButtonDown("Fire1") || (Input.GetKeyDown(KeyCode.Slash) || Input.GetKeyDown(KeyCode.Slash))) && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
            {
                Shoot(gun.stats.range, gun.stats.damage);
                //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
                timePassedUntillNextShot = 0f;
            }
        }
        else
        {
            if (gun.ModelFire && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
            {
                Shoot(gun.stats.range, gun.stats.damage);
                //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
                timePassedUntillNextShot = 0f;
            }
        }
    }
    public void ShootWithShotgunProperties()
    {
        if (stats.fireMode != FireMode.Shotgun) stats.fireMode = FireMode.Shotgun;
        if (timePassedUntillNextShot < gun.stats.boltRecoveryDuration) timePassedUntillNextShot += Time.deltaTime;
        if (!gun.ModelMode)
        {
            if ((Input.GetButtonDown("Fire1") || (Input.GetKeyDown(KeyCode.Slash) || Input.GetKeyDown(KeyCode.Slash))) && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
            {
                if (gun.stats.ammo <= 0 || stats.isSprinting) return;
                for (int i = 0; i < gun.stats.weaponData.pelletsPerFire; i++) Shoot(gun.stats.range, gun.stats.weaponData.damagePerPellet);
                gun.stats.ammo--;
                if (gun.stats.weaponData.ejectCasingAfterRechamber) StartCoroutine(Rechamber());
                //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
                timePassedUntillNextShot = 0f;
            }
        }
        else
        {
            if (gun.ModelFire && timePassedUntillNextShot >= gun.stats.boltRecoveryDuration)
            {
                if (gun.stats.ammo <= 0 || stats.isSprinting) return;
                for (int i = 0; i < gun.stats.weaponData.pelletsPerFire; i++) Shoot(gun.stats.range, gun.stats.weaponData.damagePerPellet);
                gun.stats.ammo--;
                if (gun.stats.weaponData.ejectCasingAfterRechamber) StartCoroutine(Rechamber());
                //RequestShootServerRpc(gun.stats.range, gun.stats.damage);
                timePassedUntillNextShot = 0f;
            }
        }
    }
    public void EnterAiming(bool check)
    {
        if (gun.ModelMode) return;
        if (gun.fpsCam.playerMainCamera == null) return;
        if (check)
        {
            if (gun.stats.selectedSightIndex != 0) gun.player.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.playerMainCamera.fieldOfView, sightFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
            else gun.player.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.playerMainCamera.fieldOfView, originMultiplierFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
            if (gun.stats.selectedSightIndex != 0) gun.player.fpsCam.itemLayerCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.itemLayerCamera.fieldOfView, il_sightFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
            else gun.player.fpsCam.itemLayerCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.itemLayerCamera.fieldOfView, il_originMultiplierFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
        }
        else
        {
            gun.player.fpsCam.playerMainCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.playerMainCamera.fieldOfView, originFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
            gun.player.fpsCam.itemLayerCamera.fieldOfView = Mathf.Lerp(gun.player.fpsCam.itemLayerCamera.fieldOfView, il_originFOV, Time.deltaTime * gun.stats.weaponData.aimSpeed);
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
    void BotShoot(float range, float damage)
    {
        if (gun.stats.ammo <= 0 || stats.isSprinting) return;
        if (gun.shellEject != null && !gun.stats.weaponData.ejectCasingAfterRechamber) gun.shellEject.GetComponent<ParticleSystem>().Play();
        if (gun.stats.weaponData.weaponType == WeaponType.SniperRifle) StartCoroutine(Rechamber());

        anim.animate.SetTrigger("isFiring");
        if (gun.stats.weaponData.weaponType != WeaponType.Shotgun) gun.stats.ammo--;
        RaycastHit hit;
        Ray ray = gun.fpsCam.playerMainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 shootDirection = gun.fpsCam.transform.forward;
        if (gun.stats.weaponData.hasHipfireInaccuracy)
        {
            float spreadX = 0f, spreadY = 0f;
            spreadX = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadY = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadX *= (gun.stats.weaponData.hipfireSpread * (gun.player.stats.isJumping ? 1.1f : gun.player.stats.isWalking ? 0.85f : gun.player.stats.isCrouching ? 0.4f : 0.6f));
            spreadY *= (gun.stats.weaponData.hipfireSpread * (gun.player.stats.isJumping ? 1.1f : gun.player.stats.isWalking ? 0.85f : gun.player.stats.isCrouching ? 0.4f : 0.6f));
            shootDirection.x += spreadX * 0.1f;
            shootDirection.y += spreadY * 0.1f;
            shootDirection.z += spreadX * 0.1f;
        }
        ray.origin = gun.fpsCam.transform.position;
        ray.direction = shootDirection;
        if (Physics.Raycast(ray, out hit, range))
        {
            IDamagable damageable = hit.collider.gameObject.GetComponent<IDamagable>();
            PlayerHitboxPart part = hit.collider.gameObject.GetComponent<PlayerHitboxPart>();
            Vector3 tempPoint = new Vector3(), tempNormal = new Vector3();
            //Debug.Log(hit.transform.name);
            //IDamagable player = hit.transform.GetComponent<IDamagable>();
            if (damageable != null)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Team Deathmatch")
                {
                    if (part.player.IsTeam != gun.player.IsTeam)
                        damageable?.TakeDamage(damage, false, gun.player.transform.position, gun.player.transform.rotation, gun.stats.weaponData.GlobalWeaponIndex, true);
                }
                else
                {
                    damageable?.TakeDamage(damage, false, gun.player.transform.position, gun.player.transform.rotation, gun.stats.weaponData.GlobalWeaponIndex, true);
                    if (part != null)
                    {
                        ParticleSystem psys = ObjectPooler.Instance.SpawnFromPool(part.part != PlayerHitboxPart.PlayerPart.Head ? "BloodSplatter" : "CritBloodSplatter", hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal, Vector3.up)).GetComponent<ParticleSystem>();
                        psys.Play();
                    }
                }
            }
            else
            {
                tempPoint = hit.point;
                tempNormal = hit.normal;
            }
            gun.player.InvokeGunEffects(gun.muzzleFire.transform.position, tempPoint, tempNormal);
        }
        else
            gun.player.InvokeGunEffects(gun.muzzleFire.transform.position, new Vector3(), new Vector3());
        anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        TriggerCameraRecoil(stats.verticalRecoil, stats.horizontalRecoil, stats.sphericalShake, stats.positionRecoilRetaliation, stats.positionRecoilVertical, stats.positionTransitionalSnappiness, stats.positionRecoilReturnSpeed, stats.transitionalSnappiness, stats.recoilReturnSpeed);
    }
    void Shoot(float range, float damage)
    {
        if (gun.ModelMode)
        {
            BotShoot(range, damage);
            return;
        }
        if (gun.stats.ammo <= 0 || stats.isSprinting) return;
        if (gun.shellEject != null && !gun.stats.weaponData.ejectCasingAfterRechamber) gun.shellEject.GetComponent<ParticleSystem>().Play();
        if (gun.stats.weaponData.weaponType == WeaponType.SniperRifle) StartCoroutine(Rechamber());

        float decreasedKickback = 0f;

        anim.animate.SetTrigger("isFiring");
        if (gun.stats.weaponData.weaponType != WeaponType.Shotgun) gun.stats.ammo--;
        RaycastHit hit;
        Ray ray = gun.fpsCam.playerMainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        Vector3 shootDirection = gun.fpsCam.transform.forward;
        if (gun.stats.weaponData.hasHipfireInaccuracy)
        {
            float spreadX = 0f, spreadY = 0f;
            spreadX = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadY = Random.Range(-gun.logic.spreadConstant, gun.logic.spreadConstant);
            spreadX *= (gun.stats.weaponData.hipfireSpread * (gun.player.stats.isJumping ? 1.1f : gun.player.stats.isWalking ? 0.85f : gun.player.stats.isCrouching ? 0.4f : 0.6f));
            spreadY *= (gun.stats.weaponData.hipfireSpread * (gun.player.stats.isJumping ? 1.1f : gun.player.stats.isWalking ? 0.85f : gun.player.stats.isCrouching ? 0.4f : 0.6f));
            shootDirection.x += spreadX * 0.1f;
            shootDirection.y += spreadY * 0.1f;
            shootDirection.z += spreadX * 0.1f;
        }
        ray.origin = gun.fpsCam.transform.position;
        ray.direction = shootDirection;
        if (Physics.Raycast(ray, out hit, range))
        {
            IDamagable damageable = hit.collider.gameObject.GetComponent<IDamagable>();
            PlayerHitboxPart part = hit.collider.gameObject.GetComponent<PlayerHitboxPart>();
            Vector3 tempPoint = new Vector3(), tempNormal = new Vector3();
            //Debug.Log(hit.transform.name);
            //IDamagable player = hit.transform.GetComponent<IDamagable>();
            if (damageable != null)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Team Deathmatch")
                {
                    if (part.player.IsTeam != gun.player.IsTeam)
                    {
                        bool hitmarkerFlag = false;
                        hitmarkerFlag = (bool)damageable?.TakeDamage(damage, false, gun.player.transform.position, gun.player.transform.rotation, gun.stats.weaponData.GlobalWeaponIndex, true);
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
                }
                else
                {
                    bool hitmarkerFlag = false;
                    hitmarkerFlag = (bool)damageable?.TakeDamage(damage, false, gun.player.transform.position, gun.player.transform.rotation, gun.stats.weaponData.GlobalWeaponIndex, true);

                    if (!hitmarkerFlag)
                    {
                        if (part.part == PlayerHitboxPart.PlayerPart.Head) gun.ui.ui.InvokeHitmarker(UIManager.HitmarkerType.Killmarker, Color.yellow);
                        else gun.ui.ui.InvokeHitmarker(UIManager.HitmarkerType.Hitmarker);
                        gun.player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Hitmarker);
                    }
                    else if (hitmarkerFlag)
                    {
                        gun.ui.ui.InvokeHitmarker(UIManager.HitmarkerType.Killmarker);
                        gun.player.sfx.InvokeHitmarkerAudio(UIManager.HitmarkerType.Killmarker);
                    }
                    if (part != null)
                    {
                        ParticleSystem psys = ObjectPooler.Instance.SpawnFromPool(part.part != PlayerHitboxPart.PlayerPart.Head ? "BloodSplatter" : "CritBloodSplatter", hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal, Vector3.up)).GetComponent<ParticleSystem>();
                        psys.Play();
                    }
                }
            }
            else
            {
                tempPoint = hit.point;
                tempNormal = hit.normal;
            }
            gun.player.InvokeGunEffects(gun.muzzleFire.transform.position, tempPoint, tempNormal);
        }
        else
        {
            gun.player.InvokeGunEffects(gun.muzzleFire.transform.position, new Vector3(), new Vector3());
        }
        if (gun.player.holder.weaponIndex == 0)
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 1)] != -1)
            {
                float temp = stats.kickBackZ;
                decreasedKickback = temp - (temp * 0.4f);
                anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ - (stats.recoilZ * 0.35f), decreasedKickback);
            }
            else anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        }
        else
        {
            if ((int)PhotonNetwork.LocalPlayer.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Underbarrel, 2)] != -1)
            {
                float temp = stats.kickBackZ;
                decreasedKickback = temp - (temp * 0.4f);
                anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ - (stats.recoilZ * 0.35f), decreasedKickback);
            }
            else anim.TriggerWeaponRecoil(stats.isAiming ? stats.aimingRecoilX : stats.recoilX, stats.recoilY, stats.recoilZ, stats.kickBackZ);
        }
        TriggerCameraRecoil(stats.verticalRecoil, stats.horizontalRecoil, stats.sphericalShake, stats.positionRecoilRetaliation, stats.positionRecoilVertical, stats.positionTransitionalSnappiness, stats.positionRecoilReturnSpeed, stats.transitionalSnappiness, stats.recoilReturnSpeed);
    }

    IEnumerator Rechamber()
    {
        //Debug.Log("Rechambering!");
        yield return new WaitForSeconds(gun.stats.weaponData.rechamberDelay);
        anim.animate.SetTrigger("rechamberAnim");
        if (gun.stats.weaponData.useRechamberClipAudioList) gun.audio.PlayRechamberSound();
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

    public void SpawnPickup()
    {
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
