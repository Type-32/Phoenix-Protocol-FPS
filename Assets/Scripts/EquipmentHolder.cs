using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EquipmentHolder : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerControllerManager player;
    public int weaponIndex = 0;
    public int previousWeaponIndex = -1;
    [SerializeField] private UIManager uiManager;
    //[SerializeField] WeaponData[] weapons;
    //[SerializeField] ThrowablesData[] equipments;
    public bool inversedScrollWheel = true;
    public Gun[] weaponSlots;
    public Item[] equipmentSlots;
    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = player.GetComponentInChildren<UIManager>();
    }
    void Start()
    {

        for (int i = 0; i < 2; i++)
        {
            if(player.playerManager.slotHolderScript.slotWeaponData[i] != null)
            {
                InstantiateWeapon(player.playerManager.slotHolderScript.slotWeaponData[i], i);
                weaponSlots[i].InitializeAwake();
                weaponSlots[i].InitializeStart();
            }
        }
        if (!player.pv.IsMine)
        {
            Transform[] muzzle1 = weaponSlots[0].gun.muzzleFire.gameObject.GetComponentsInChildren<Transform>();
            Transform[] muzzle2 = weaponSlots[1].gun.muzzleFire.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < muzzle1.Length; i++) muzzle1[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < muzzle2.Length; i++) muzzle2[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            weaponSlots[0].gun.muzzleFire.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            weaponSlots[1].gun.muzzleFire.gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            Transform[] list1 = weaponSlots[0].gun.gunVisual.GetComponentsInChildren<Transform>();
            Transform[] list2 = weaponSlots[1].gun.gunVisual.GetComponentsInChildren<Transform>();
            Transform[] list3 = weaponSlots[0].gun.attachment.attachmentHolder.GetComponentsInChildren<Transform>();
            Transform[] list4 = weaponSlots[1].gun.attachment.attachmentHolder.GetComponentsInChildren<Transform>();
            Debug.Log("Init Start Line 51");
            EquipWeapon(0);
            for (int i = 0; i < list1.Length; i++) list1[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list2.Length; i++) list2[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list3.Length; i++) list3[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list4.Length; i++) list4[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            Transform[] obj1 = weaponSlots[0].gun.handsVisual.GetComponentsInChildren<Transform>();
            Transform[] obj2 = weaponSlots[1].gun.handsVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < obj1.Length; i++) Destroy(obj1[i].gameObject);
            for (int i = 0; i < obj2.Length; i++) Destroy(obj2[i].gameObject);
            Destroy(weaponSlots[0].gun.handsVisual);
            Destroy(weaponSlots[1].gun.handsVisual);
            weaponSlots[0].gun.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            weaponSlots[1].gun.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
        }
        if (player.pv.IsMine)
        {
            Debug.Log("Init Start Line 36");
            EquipWeapon(0);
        }
        else
        {
            Camera[] temp = GetComponentsInChildren<Camera>();
            for (int i = 0; i < temp.Length; i++)
            Destroy(temp[i].gameObject);
        }
        //SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.pv.IsMine) return;

        if (player.playerManager.openedOptions || player.playerManager.openedLoadoutMenu) return;
        KeySwitchWeapon();
        ScrollWheelSwitchWeapon();
    }
    public void SelectWeapon()
    {
        for(int i = 0; i < 2; i++)
        {
            if(i == weaponIndex)
            {
                EquipWeapon(i);
            }
            else
            {
                weaponSlots[i].item.SetActive(false);
            }
        }
    }
    void KeySwitchWeapon()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipWeapon(i);
            }
        }
    }
    void ScrollWheelSwitchWeapon()
    {
        if (inversedScrollWheel)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (weaponIndex >= weaponSlots.Length - 1) EquipWeapon(0);
                else EquipWeapon(weaponIndex + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (weaponIndex <= 0) EquipWeapon(weaponSlots.Length - 1);
                else EquipWeapon(weaponIndex - 1);
            }
        }
        else
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (weaponIndex >= weaponSlots.Length - 1) EquipWeapon(0);
                else EquipWeapon(weaponIndex + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (weaponIndex <= 0) EquipWeapon(weaponSlots.Length - 1);
                else EquipWeapon(weaponIndex - 1);
            }
        }
    }
    public void EquipWeapon(int _index)
    {
        if (_index == previousWeaponIndex) return;
        weaponIndex = _index;
        weaponSlots[weaponIndex].item.SetActive(true);
        if(previousWeaponIndex != -1)
        {
            weaponSlots[previousWeaponIndex].item.SetActive(false);
        }
        previousWeaponIndex = weaponIndex;

        if (player.pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }
    public bool InstantiateWeapon(WeaponData data, int index)
    {
        if (weaponSlots.Length > 2) return false;
        GameObject temp = Instantiate(data.weaponPrefab, transform);
        temp.GetComponent<GunManager>().player = player;
        weaponSlots[index] = temp.GetComponent<GunManager>();
        weaponSlots[index].item = temp;
        weaponSlots[index].itemData = data;
        weaponSlots[index].InitializeAwake();
        weaponSlots[index].InitializeStart();
        weaponSlots[index].item.SetActive(false);
        EquipWeapon(index);
        temp.GetComponent<GunManager>().attachment.EnableGunAttachments(index);
        return true;
    }
    public bool InstantiateEquipment(ThrowablesData data, int index)
    {
        if (equipmentSlots.Length > 2) return false;
        GameObject temp = Instantiate(data.throwablesPrefab, transform);
        equipmentSlots[index].item = temp;
        return true;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!player.pv.IsMine && targetPlayer == player.pv.Owner && changedProps.ContainsKey("weaponIndex"))
        {
            EquipWeapon((int)changedProps["weaponIndex"]);
            //player.playerManager.slotHolderScript.slotWeaponData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["weaponDataChanged"]];
        }
    }
    public void WeaponFunction()
    {
        if (!player.pv.IsMine) return;
        weaponSlots[weaponIndex].Use();
    }
    public void InitializeAwakeOnGuns()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].InitializeAwake();
        }
    }
    public void InitializeStartOnGuns()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            weaponSlots[i].InitializeStart();
        }
    }
}
