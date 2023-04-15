using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using UnityEngine.Serialization;

public class PlayerControllerManager : MonoBehaviourPunCallbacks, IDamagable
{
    [Header("Generic Setup")]
    public bool BotMode = false;
    [Space, Header("Script Control")]
    public PlayerControls controls;
    public PlayerStats stats;
    public PlayerSounds sfx;
    public PlayerAnimation anim;
    public UIManager ui;
    public MouseLookScript cam;
    public EquipmentHolder holder;
    public IDamagable damagable;
    //public GadgetUsageScript gadgetFunc;

    [Space]
    [Header("Character Customization")]
    [SerializeField] GameObject playerHead;
    [SerializeField] GameObject playerBody;
    [SerializeField] GameObject playerFeet1;
    [SerializeField] GameObject playerFeet2;
    [SerializeField] Material local_headMaterial;
    [SerializeField] Material local_bodyMaterial;
    [SerializeField] Material local_feetMaterial;
    public Material local_handMaterial;
    [SerializeField] Material global_headMaterial;
    [SerializeField] Material global_bodyMaterial;
    [SerializeField] Material global_feetMaterial;
    public Material global_handMaterial;
    [SerializeField] Material team_headMaterial;
    [SerializeField] Material team_bodyMaterial;
    [SerializeField] Material team_feetMaterial;
    public Material team_handMaterial;
    public Material local_trailMaterial;
    [FormerlySerializedAs("universal_trailMaterial")] public Material global_trailMaterial;

    [Space]
    [Header("References")]
    public CharacterController body;
    public CapsuleCollider capsuleCollider;
    public MouseLookScript fpsCam;
    public GameObject deathCam;
    public Animator cameraAnim;
    public Recoil recoilScript;
    public Transform groundCheck;
    public GameObject playerBloodSplatter;
    public GameObject playerCritBloodSplatter;
    public GameObject playerDeathEffect;
    //public Camera cameraView;
    public List<PlayerHitboxPart> playerPartHitboxes = new();

    [Space]
    [Header("Ground Masks")]
    public LayerMask groundMask;

    [Space]
    [Header("Volume Effects")]
    [SerializeField] Volume playerVolumeEffect, playerHurtEffect, armorHurtEffect, nightVisionEffect;
    public Volume playerGunLayerEffect;

    [Space]
    [Header("Multiplayer")]
    public PhotonView pv;
    public PlayerManager playerManager;
    public GameObject bodyRotatePoint;
    public List<GameObject> playerDeathLoots;
    private bool hasArmor = false;
    public bool IsTeam = false;
    private float timePassedAfterDamageTaken = 5f;
    public bool usingStreakGifts = false;
    public GameObject playerMinimapDot;
    public List<GameObject> allMinimapDots;
    public bool hidePlayerHUD = false;

    [SerializeField] int weaponIndex1;
    [SerializeField] int weaponIndex2;

    private void Awake()
    {
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
        pv = GetComponent<PhotonView>();
        //ui = FindObjectOfType<UIManager>();
        //if(playerHeadMat != null) gameObject.GetComponent<MeshRenderer>playerHeadMat

    }
    private void Start()
    {
        //PlayerHitboxPart[] tb = GetComponentsInChildren<PlayerHitboxPart>();
        //foreach (PlayerHitboxPart g in tb) playerPartHitboxes.Add(g);
        if (pv.IsMine)
        {
            //if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Team Deathmatch") IsTeam = (bool)pv.Owner.CustomProperties["team"];
            ui.hostileTIGroup.alpha = 0;
            weaponIndex1 = (int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)];
            weaponIndex2 = (int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)];
            recoilScript = FindObjectOfType<Recoil>();
            DerivePlayerStatsToHUDInitialize();
            playerHead.SetActive(false);
            playerBody.SetActive(false);
            playerFeet1.SetActive(false);
            playerFeet2.SetActive(false);
            if (local_headMaterial != null) playerHead.GetComponent<MeshRenderer>().material = local_headMaterial;
            if (local_bodyMaterial != null) playerBody.GetComponent<MeshRenderer>().material = local_bodyMaterial;
            if (local_feetMaterial != null) playerFeet1.GetComponent<MeshRenderer>().material = local_feetMaterial;
            if (local_feetMaterial != null) playerFeet2.GetComponent<MeshRenderer>().material = local_feetMaterial;
            stats.enableNightVision = playerManager.nightVisionState;
            nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
            MinimapDotIdentifier[] tempget;
            tempget = FindObjectsOfType<MinimapDotIdentifier>();
            for (int i = 0; i < tempget.Length; i++)
            {
                allMinimapDots.Add(tempget[i].gameObject);
            }
            DisableAllMinimapDots();
            playerMinimapDot.SetActive(true);
            ui.hud.GetComponent<CanvasGroup>().alpha = 1;
            ui.FFA_UI.SetActive(false);
            ui.TDM_UI.SetActive(false);
            ui.CTF_UI.SetActive(false);
            ui.DZ_UI.SetActive(false);
            if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.FFA)
            {
                ui.FFA_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.TDM)
            {
                ui.TDM_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.CTF)
            {
                ui.CTF_UI.SetActive(true);
            }
            else if (CurrentMatchManager.Instance.roomMode == MenuManager.Gamemodes.DZ)
            {
                ui.DZ_UI.SetActive(true);
            }
            playerManager.cameraObject.enabled = false;
        }
        else
        {
            body.enabled = false;
            capsuleCollider.enabled = false;
            playerHead.SetActive(true);
            playerBody.SetActive(true);
            playerFeet1.SetActive(true);
            playerFeet2.SetActive(true);
            if (global_headMaterial != null) playerHead.GetComponent<MeshRenderer>().material = global_headMaterial;
            if (global_bodyMaterial != null) playerBody.GetComponent<MeshRenderer>().material = global_bodyMaterial;
            if (global_feetMaterial != null) playerFeet1.GetComponent<MeshRenderer>().material = global_feetMaterial;
            if (global_feetMaterial != null) playerFeet2.GetComponent<MeshRenderer>().material = global_feetMaterial;
            SetGlobalBodyMaterialColor(Color.red.r, Color.red.g, Color.red.b);
            SetGlobalFeetMaterialColor(Color.red.r, Color.red.g, Color.red.b);
            SetGlobalHeadMaterialColor(Color.red.r, Color.red.g, Color.red.b);
            SetGlobalHandMaterialColor(Color.red.r, Color.red.g, Color.red.b);
            if (PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.RoomMode].ToString() == "Team Deathmatch")
            {
                IsTeam = playerManager.IsTeam;
                if (IsTeam == CurrentMatchManager.Instance.localClientPlayer.IsTeam)
                {
                    if (team_headMaterial != null) playerHead.GetComponent<MeshRenderer>().material = team_headMaterial;
                    if (team_bodyMaterial != null) playerBody.GetComponent<MeshRenderer>().material = team_bodyMaterial;
                    if (team_feetMaterial != null) playerFeet1.GetComponent<MeshRenderer>().material = team_feetMaterial;
                    if (team_feetMaterial != null) playerFeet2.GetComponent<MeshRenderer>().material = team_feetMaterial;
                    SetTeamBodyMaterialColor(Color.blue.r, Color.blue.g, Color.blue.b);
                    SetTeamFeetMaterialColor(Color.blue.r, Color.blue.g, Color.blue.b);
                    SetTeamHeadMaterialColor(Color.blue.r, Color.blue.g, Color.blue.b);
                    SetTeamHandMaterialColor(Color.blue.r, Color.blue.g, Color.blue.b);
                }
            }
            weaponIndex1 = (int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)];
            weaponIndex2 = (int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)];
            Destroy(ui.gameObject);
            nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
            playerMinimapDot.SetActive(false);
        }
    }
    void FixedUpdate()
    {
        if (BotMode) return;
        if (!pv.IsMine) return;
        CrosshairNametagDetect();
    }
    private void Update()
    {
        if (BotMode)
        {
            if (transform.position.y < -50) Die(true, -1);
            return;
        }
        if (!pv.IsMine) return;
        if (transform.position.y < -50) Die(true, -1);
        DerivePlayerStatsToHUD();
        PlayerGUIReference();
        if (Input.GetKeyDown("l"))
        {
            if (hidePlayerHUD)
            {
                hidePlayerHUD = false;
                ui.hud.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                hidePlayerHUD = true;
                ui.hud.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        if (playerManager.recordKills >= 3)
        {
            ui.streakBackground.value = Mathf.Lerp(ui.streakBackground.value, 1f, Time.deltaTime * 3);
            ui.streakHUDAlpha.alpha = Mathf.Lerp(ui.streakHUDAlpha.alpha, 1f, Time.deltaTime * 3);
        }
        else
        {
            ui.streakBackground.value = Mathf.Lerp(ui.streakBackground.value, 0f, Time.deltaTime * 3);
            ui.streakHUDAlpha.alpha = Mathf.Lerp(ui.streakHUDAlpha.alpha, 0.2f, Time.deltaTime * 3);
        }

        if (timePassedAfterDamageTaken < 5f) timePassedAfterDamageTaken += Time.deltaTime;
        else
        {
            if (stats.health < stats.healthLimit) stats.health += 1f;
        }

        /*
        * stats.playerMovementEnabled and stats.mouseMovementEnabled are both managed by the conditions below.
        ! Not dynamic, as it is setted in a static way, SetPlayerControlState() would be useless for modifying these two values.
        */
        if (playerManager.openedLoadoutMenu)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else if (playerManager.openedOptions)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else if (CurrentMatchManager.Instance.gameEnded)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else if (stats.isDowned || stats.isDead)
        {
            stats.playerMovementEnabled = false;
            stats.mouseMovementEnabled = false;
        }
        else
        {
            stats.playerMovementEnabled = true;
            stats.mouseMovementEnabled = true;
        }
    }
    public void ChangePlayerHitbox(Vector3 center, float radius, float height)
    {
        pv.RPC(nameof(RPC_ChangePlayerHitbox), RpcTarget.All, center, radius, height);
    }
    public void EnableAllMinimapDots()
    {
        MinimapDotIdentifier[] tempget;
        tempget = FindObjectsOfType<MinimapDotIdentifier>();
        for (int i = 0; i < tempget.Length; i++)
        {
            tempget[i].gameObject.SetActive(true);
        }
        //OperateAllMinimapDots(false);
        playerMinimapDot.SetActive(true);
    }
    public void DisableAllMinimapDots()
    {
        MinimapDotIdentifier[] tempget;
        tempget = FindObjectsOfType<MinimapDotIdentifier>();
        for (int i = 0; i < tempget.Length; i++)
        {
            tempget[i].gameObject.SetActive(false);
        }
        //OperateAllMinimapDots(false);
        playerMinimapDot.SetActive(true);
    }
    public void SetMouseSensitivity(float value)
    {
        stats.mouseSensitivity = value;
        controls.aimingMouseSensitivity = stats.mouseSensitivity * 0.8f;
    }

    public bool TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot, int weaponIndex, bool isWeapon)
    {
        bool tempflag = false;
        stats.health -= amount;
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, amount, bypassArmor, targetPos, targetRot, weaponIndex, isWeapon);
        /*
        if (stats.health <= 0)//Lag compensation here for visual player death lag, but it didn't work
        {
            tempflag = true;
            ClientFakeDeath();
            InvokePlayerDeathEffects();
        }*/
        //if (stats.health - amount <= 0) Destroy(gameObject);
        return tempflag;
        //ui.ShowHealthBar(2f);

    }

    #region Body Materials
    public void SetBodyMaterialColor(Color color)
    {
        if (local_bodyMaterial == null) return;
        local_bodyMaterial.color = color;
    }
    public void SetFeetMaterialColor(Color color)
    {
        if (local_feetMaterial == null) return;
        local_feetMaterial.color = color;
    }
    public void SetHeadMaterialColor(Color color)
    {
        if (local_headMaterial == null) return;
        local_headMaterial.color = color;
    }
    public void SetHandMaterialColor(Color color)
    {
        if (local_handMaterial == null) return;
        local_handMaterial.color = color;
    }
    public void SetGlobalBodyMaterialColor(float r, float g, float b)
    {
        if (global_bodyMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        global_bodyMaterial.color = color;
    }
    public void SetGlobalFeetMaterialColor(float r, float g, float b)
    {
        if (global_feetMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        global_feetMaterial.color = color;
    }
    public void SetGlobalHeadMaterialColor(float r, float g, float b)
    {
        if (global_headMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        global_headMaterial.color = color;
    }
    public void SetGlobalHandMaterialColor(float r, float g, float b)
    {
        if (global_handMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        global_handMaterial.color = color;
    }
    public void SetTeamBodyMaterialColor(float r, float g, float b)
    {
        if (team_bodyMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        team_bodyMaterial.color = color;
    }
    public void SetTeamFeetMaterialColor(float r, float g, float b)
    {
        if (team_feetMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        team_feetMaterial.color = color;
    }
    public void SetTeamHeadMaterialColor(float r, float g, float b)
    {
        if (team_headMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        team_headMaterial.color = color;
    }
    public void SetTeamHandMaterialColor(float r, float g, float b)
    {
        if (team_handMaterial == null) return;
        Color color = new Color(r, g, b, 1);
        team_handMaterial.color = color;
    }
    #endregion
    public IEnumerator UseStreakGift(float duration, int cost)
    {
        ui.hostileTIGroup.alpha = 1;
        usingStreakGifts = true;
        playerManager.recordKills -= cost;
        yield return new WaitForSeconds(duration);
        usingStreakGifts = false;
        ui.hostileTIGroup.alpha = 0;
    }
    private void TakeHitEffect()
    {

    }
    private void DerivePlayerStatsToHUD()
    {
        if (ui == null) return;
        ui.healthBar.value = Mathf.Lerp(ui.healthBar.value, stats.health, 8 * Time.deltaTime);
        //ui.healthBarFill.color = Color.Lerp(ui.healthBarFill.color, (stats.health >= 50f) ? Color.green : (stats.health < 50f && stats.health >= 30f) ? Color.yellow : Color.red, 5 * Time.deltaTime);
        //ui.healthText.text = ((int)stats.health).ToString();
        ui.healthText.text = ((int)stats.health).ToString();
        ui.armorText.text = ((int)stats.armor).ToString();
        playerVolumeEffect.weight = 1f - (stats.health / 100f);
        playerHurtEffect.weight = Mathf.Lerp(playerHurtEffect.weight, 0f, 8 * Time.deltaTime);
        armorHurtEffect.weight = Mathf.Lerp(armorHurtEffect.weight, 0f, 8 * Time.deltaTime);
    }
    private void DerivePlayerStatsToHUDInitialize()
    {
        if (ui == null) return;
        ui.healthBar.maxValue = stats.healthLimit;
        stats.stress = 0;
    }
    private void CrosshairNametagDetect()
    {
        RaycastHit hit;
        if (Physics.Raycast(holder.transform.position, holder.transform.forward, out hit, stats.playerNametagDistance))
        {
            PhotonView _pv = hit.collider.GetComponent<PhotonView>();
            ProjectileBehaviour _proj = hit.collider.GetComponent<ProjectileBehaviour>();
            if (_pv != null && _proj == null && _pv.Owner != pv.Owner)
            {
                ui.nametagIndicatorObject.SetActive(true);
                ui.nametagIndicator.text = (_pv.Owner.NickName == null ? "Anonymous" : _pv.Owner.NickName);
                ui.SetReticleColor(Color.red);
                ui.nametagIndicator.color = Color.red;
            }
            else
            {
                ui.SetReticleColor(Color.white);
                ui.nametagIndicatorObject.SetActive(false);
            }
        }
        else
        {
            ui.SetReticleColor(Color.white);
            ui.nametagIndicatorObject.SetActive(false);
        }
    }
    private void PlayerGUIReference()
    {
        if (ui == null) return;
        if (playerManager.openedInventory) { stats.mouseMovementEnabled = false; stats.playerMovementEnabled = false; }
        else { stats.mouseMovementEnabled = true; stats.playerMovementEnabled = true; }
    }
    public void ClientFakeDeath()
    {
        capsuleCollider.enabled = false;
        playerHead.SetActive(false);
        playerBody.SetActive(false);
        playerFeet1.SetActive(false);
        playerFeet2.SetActive(false);
        gameObject.SetActive(false);
    }
    public void SpawnDeathLoot()
    {
        if (playerDeathLoots.Count > 0)
        {
            int randomIndex = Random.Range(0, playerDeathLoots.Count - 1);
            pv.RPC(nameof(RPC_SpawnDeathLoot), RpcTarget.All, transform.position, randomIndex);
        }
    }
    public void TogglePlayerPartsHitboxes(bool value)
    {
        foreach (PlayerHitboxPart t in playerPartHitboxes) t.enabled = value;
        capsuleCollider.enabled = value;
        body.enabled = value;
    }
    [PunRPC]
    public void RPC_OnZeroedHealth(bool isDeath = true)
    {
        if (isDeath)
        {
            Debug.Log($"{pv.Owner.NickName} is dead");
            stats.isDead = stats.isDowned = true;
            SetPlayerControlState(false);
            TogglePlayerPartsHitboxes(false);
            holder.gameObject.SetActive(false);
            fpsCam.playerMainCamera.GetComponent<AudioListener>().enabled = false;
            capsuleCollider.enabled = false;
            body.enabled = false;
        }
        else
        {
            Debug.Log($"{pv.Owner.NickName} is downed");
            stats.isDowned = true;
            SetPlayerControlState(false);
            holder.gameObject.SetActive(false);
            fpsCam.playerMainCamera.GetComponent<AudioListener>().enabled = false;
            capsuleCollider.enabled = false;
            body.enabled = false;
        }
    }
    public void Die(bool isSuicide, int ViewID, string killer = null)
    {
        pv.RPC(nameof(RPC_OnZeroedHealth), RpcTarget.All, true);
        holder.gameObject.SetActive(false);
        playerManager.Die(isSuicide, ViewID, killer);

        InvokePlayerDeathEffects();
        //SynchronizePlayerState(true, 5);
        SpawnDeathLoot();
        DisableAllMinimapDots();
        usingStreakGifts = false;
        stats.enableGravity = false;
        ui.gameObject.SetActive(false);
        Debug.Log("Player " + stats.playerName + " was Killed");
        return;
    }
    public void Downed(bool isSuicide, int ViewID, string killer = null)
    {
        InvokePlayerDeathEffects();
        //SynchronizePlayerState(true, 4);
        usingStreakGifts = false;
        pv.RPC(nameof(RPC_OnZeroedHealth), RpcTarget.All, false);
        Debug.Log("Player " + stats.playerName + " is Downed");
        return;
    }
    public void ToggleNightVision()
    {
        if (stats.enableNightVision)
        {
            stats.enableNightVision = false;
        }
        else
        {
            stats.enableNightVision = true;
        }
        nightVisionEffect.gameObject.SetActive(stats.enableNightVision);
        playerManager.nightVisionState = stats.enableNightVision;
        if (stats.enableNightVision) sfx.InvokeUseNightVisionAudio();
        else sfx.InvokeRemoveNightVisionAudio();
    }
    [PunRPC]
    void RPC_ChangePlayerHitbox(Vector3 center, float radius, float height)
    {
        body.center = capsuleCollider.center = center;
        body.radius = capsuleCollider.radius = radius;
        body.height = capsuleCollider.height = height;
    }
    [PunRPC]
    void RPC_SpawnDeathLoot(Vector3 pos, int randomIndex)
    {
        pos.y += 1.5f;
        GameObject temp = Instantiate(playerDeathLoots[randomIndex], pos, transform.rotation);
        Destroy(temp, 10f);
    }
    Transform tempTransform;
    [PunRPC]
    void RPC_TakeDamage(float amount, bool bypassArmor, Vector3 targetPos, Quaternion targetRot, int weaponIndex, bool isWeapon, PhotonMessageInfo info)
    {
        Debug.Log($"{info.Sender.NickName} is attacking {pv.Owner.NickName}");
        if (stats.isDead) return;
        if (info.Sender == pv.Owner && isWeapon) return;
        Debug.Log("Took Damage " + amount + " from " + info.Sender.NickName + " using " + (isWeapon ? "Weapon " : "Equipment ") + (weaponIndex.ToString()));
        //tempTransform.position = transform.position;
        //tempTransform.rotation = transform.rotation;
        //tempTransform.position = targetPos;
        //tempTransform.rotation = targetRot;
        UIManager.ValueTransform tmp;
        tmp.position = targetPos;
        tmp.rotation = targetRot;
        UIManager.CreateIndicator(tmp);
        //Core Take Damage Functions
        recoilScript.RecoilFire(0.4f, 0.8f, 4, 0.12f, 0, 5, 12, 5, 12);
        if (bypassArmor)
        {
            stats.health -= amount;
            timePassedAfterDamageTaken = 0f;
            playerHurtEffect.weight = 1f;
            sfx.InvokePlayerHurtAudio();
        }
        else
        {
            if (stats.armor - amount <= 0)
            {
                if (hasArmor) sfx.InvokeArmorDamagedAudio();
                hasArmor = false;
                if (stats.armor - amount < 0)
                {
                    float temp = stats.armor - amount;
                    stats.armor = 0f;
                    stats.health += temp;
                    timePassedAfterDamageTaken = 0f;
                }
                else
                {
                    stats.armor = 0f;
                }
                playerHurtEffect.weight = 1f;
            }
            else
            {
                stats.armor -= amount;
                hasArmor = true;
                armorHurtEffect.weight = 1f;
                sfx.InvokePlayerHurtAudio();
            }

        }
        stats.totalAbsorbedDamage += amount;
        if (stats.health <= 0f)
        {
            int tm = (int)info.Sender.CustomProperties["weaponIndex"] == 0 ? (int)info.Sender.CustomProperties[LoadoutKeys.SelectedWeaponIndex(1)] : (int)info.Sender.CustomProperties[LoadoutKeys.SelectedWeaponIndex(2)];
            if (info.Sender != pv.Owner)
            {
                if (CurrentMatchManager.Instance.allowDownedState)
                { stats.isDowned = true; stats.isDead = false; }
                else
                { stats.isDowned = true; stats.isDead = true; }
                PlayerManager.Find(info.Sender).GetKill(pv.Owner.NickName, (weaponIndex == -1 ? tm : weaponIndex), isWeapon);
                Die(false, pv.ViewID, info.Sender.NickName);
            }
            else if (info.Sender == pv.Owner && !isWeapon)
            {
                if (CurrentMatchManager.Instance.allowDownedState)
                { stats.isDowned = true; stats.isDead = false; }
                else
                { stats.isDowned = true; stats.isDead = true; }
                Die(true, pv.ViewID, pv.Owner.NickName);
            }
        }
        return;
    }
    public void InvokeGunEffects(Vector3 origin, Vector3 point = new Vector3(), Vector3 normal = new Vector3(), bool playSound = true)
    {
        pv.RPC(nameof(RPC_InvokeGunEffects), RpcTarget.All, origin, point, normal, playSound);
    }
    public void InvokePlayerDeathEffects()
    {
        //Debug.LogWarning("Invoking PLayer Death Effects RPC...");
        pv.RPC(nameof(RPC_InvokePlayerDeathEffects), RpcTarget.All);
    }
    public void SynchronizePlayerState(bool value, int stateIndex)
    {
        pv.RPC(nameof(RPC_ChangePlayerState), RpcTarget.All, value, stateIndex);
    }
    [PunRPC]
    public void RPC_ChangePlayerState(bool value, int ind)
    {
        /*
        0. Sprinting
        1. Crouching
        2. Sliding
        3. Walking
        4. Downed
        5. Dead
        */
        switch (ind)
        {
            case 0:
                stats.isSprinting = value;
                break;
            case 1:
                stats.isCrouching = value;
                break;
            case 2:
                stats.isSliding = value;
                break;
            case 3:
                stats.isWalking = value;
                break;
            case 4:
                stats.isDowned = value;
                break;
            case 5:
                stats.isDead = value;
                break;
            default:
                break;
        }
    }
    [PunRPC]
    public void RPC_InvokePlayerDeathEffects()
    {
        GameObject obj = Instantiate(playerDeathEffect, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);
        Destroy(obj, 5f);
    }
    [PunRPC]
    public void RPC_InvokeGunEffects(Vector3 origin, Vector3 point = new Vector3(), Vector3 normal = new Vector3(), bool playSound = true)
    {
        if (playSound)
        {
            if (pv.IsMine)
            {
                if (holder.weaponIndex == 0)
                {
                    if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 1)] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
                    else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
                }
                else
                {
                    if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 2)] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(false);
                    else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayGunSound(true);
                }
            }
            else
            {
                if (holder.weaponIndex == 0)
                {
                    if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 1)] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayNPCGunSound(false);
                    else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayNPCGunSound(true);
                }
                else
                {
                    if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 2)] == -1) holder.weaponSlots[holder.weaponIndex].gun.audio.PlayNPCGunSound(false);
                    else holder.weaponSlots[holder.weaponIndex].gun.audio.PlayNPCGunSound(true);
                }
            }
        }

        if (holder.weaponIndex == 0)
        {
            if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 1)] == -1)
            {
                if (holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.Shotgun)
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("ShotgunFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
                else if (holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.SniperRifle || holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.MarksmanRifle)
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("MarksmanFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
                else
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("StandardFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
                //Destroy(temp, 3f);
            }
        }
        else
        {
            if ((int)pv.Owner.CustomProperties[LoadoutKeys.SelectedWeaponCustomization(AttachmentTypes.Barrel, 2)] == -1)
            {
                if (holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.Shotgun)
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("ShotgunFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
                else if (holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.SniperRifle || holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType == WeaponType.MarksmanRifle)
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("MarksmanFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
                else
                {
                    ParticleSystem temp = ObjectPooler.Instance.SpawnFromPool("StandardFireMuzzleFlash", holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.position, holder.weaponSlots[holder.weaponIndex].gun.muzzleFire.transform.rotation).GetComponent<ParticleSystem>();
                    if (pv.IsMine)
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("Item");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("Item");
                        }
                    }
                    else
                    {
                        temp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        Transform[] obj = temp.gameObject.GetComponentsInChildren<Transform>();
                        foreach (Transform tp in obj)
                        {
                            tp.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
                        }
                    }
                    temp.Play();
                }
            }
        }

        if (point != new Vector3() && normal != new Vector3())
        {
            Collider[] colliders = Physics.OverlapSphere(point, 0.3f);
            if (colliders.Length != 0)
            {
                ParticleSystem bulletImpactObject = ObjectPooler.Instance.SpawnFromPool("BulletImpactEffect", point + normal * 0.01f, Quaternion.LookRotation(-normal, Vector3.up)).GetComponent<ParticleSystem>();
                bulletImpactObject.Play();
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].GetComponent<PlayerControllerManager>() != null || colliders[i].GetComponent<PlayerHitboxPart>() != null)
                    {
                        bulletImpactObject.Stop();
                        return;
                    }
                }
                bulletImpactObject.transform.SetParent(colliders[0].transform);
                //bulletImpactObject.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x,transform.rotation.y, Random.Range(0f, 90f)));
            }
            StartCoroutine(UseTrailEffect(holder.weaponIndex < 2 ? holder.weaponSlots[holder.weaponIndex].gun.stats.weaponData.weaponType !=
                                                                   (WeaponType.SniperRifle | WeaponType.MarksmanRifle) ? 0.002f : 0.01f : 0.002f, origin, point));
        }
    }
    IEnumerator UseTrailEffect(float miliseconds, Vector3 from, Vector3 to)
    {
        LineRenderer temp = ObjectPooler.Instance.SpawnFromPool("BulletTrailEffect", from, Quaternion.identity).GetComponent<LineRenderer>();
        
        temp.material = pv.IsMine ? local_trailMaterial : global_trailMaterial;
        temp.SetPosition(0, from);
        temp.SetPosition(1, to);
        yield return new WaitForSeconds(miliseconds);
        temp.gameObject.SetActive(false);
    }
    public void SetPlayerControlState(bool state)
    {
        stats.mouseMovementEnabled = state;
        stats.gunInteractionEnabled = state;
        stats.playerMovementEnabled = state;
        stats.playerCameraBobEnabled = state;
    }
}
