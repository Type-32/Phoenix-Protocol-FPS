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
            InstantiateWeapon(player.playerManager.slotHolderScript.slotWeaponData[i], i);
            weaponSlots[i].InitializeAwake();
            weaponSlots[i].InitializeStart();
        }
        if (player.pv.IsMine)
        {
            EquipWeapon(weaponIndex);
        }
        else
        {
            Camera[] temp = GetComponentsInChildren<Camera>();
            for (int i = 0; i < temp.Length; i++)
            Destroy(temp[i].gameObject);
        }
        if (!player.pv.IsMine)
        {
            weaponSlots[0].gun.muzzleFire.gameObject.layer = LayerMask.NameToLayer("Default");
            weaponSlots[1].gun.muzzleFire.gameObject.layer = LayerMask.NameToLayer("Default");
            Transform[] list1 = weaponSlots[0].gun.gunVisual.GetComponentsInChildren<Transform>();
            Transform[] list2 = weaponSlots[1].gun.gunVisual.GetComponentsInChildren<Transform>();
            EquipWeapon(0);
            for (int i = 0; i < list1.Length; i++)
            {
                list1[i].gameObject.layer = LayerMask.NameToLayer("Default");
            }
            for (int i = 0; i < list2.Length; i++)
            {
                list2[i].gameObject.layer = LayerMask.NameToLayer("Default");
            }
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
        //EquipWeapon(index);
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
