using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UserConfiguration;

public class EquipmentHolder : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerControllerManager player;
    public int weaponIndex = 0;
    public int previousWeaponIndex = -1;
    public int previousIndex = 0;
    [SerializeField] private UIManager uiManager;
    public bool inversedScrollWheel = true;
    public Gun[] weaponSlots = new Gun[2];
    public Equipment[] equipmentSlots = new Equipment[2];
    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = player.GetComponentInChildren<UIManager>();
    }
    void Start()
    {
        if (!player.pv.IsMine)
        {
            for (int i = 0; i < 2; i++)
            {
                InstantiateWeapon(GlobalDatabase.singleton.allWeaponDatas[i == 0 ? ((int)player.pv.Owner.CustomProperties["selectedMainWeaponIndex"]) : ((int)player.pv.Owner.CustomProperties["selectedSecondWeaponIndex"])], i);
                //weaponSlots[i].InitializeAwake();
                //weaponSlots[i].InitializeStart();
            }
            for (int i = 0; i < 2; i++)
            {
                InstantiateEquipment(GlobalDatabase.singleton.allEquipmentDatas[i == 0 ? ((int)player.pv.Owner.CustomProperties["selectedEquipmentIndex1"]) : ((int)player.pv.Owner.CustomProperties["selectedEquipmentIndex2"])], i);
            }
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
            EquipItem(0);
            for (int i = 0; i < list1.Length; i++) list1[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list2.Length; i++) list2[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list3.Length; i++) list3[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < list4.Length; i++) list4[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            /*
            Transform[] obj1 = weaponSlots[0].gun.handsVisual.GetComponentsInChildren<Transform>();
            Transform[] obj2 = weaponSlots[1].gun.handsVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < obj1.Length; i++) obj1[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < obj2.Length; i++) obj2[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            Transform[] obj3 = weaponSlots[0].gun.thirdPersonHandsVisual.GetComponentsInChildren<Transform>();
            Transform[] obj4 = weaponSlots[1].gun.thirdPersonHandsVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < obj3.Length; i++) obj3[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < obj4.Length; i++) obj4[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            */

            /*
            if (weaponSlots[0] != null)
            {
                Destroy(weaponSlots[0].gun.handsVisual);
                weaponSlots[0].gun.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            if (weaponSlots[1] != null)
            {
                Destroy(weaponSlots[1].gun.handsVisual);
                weaponSlots[1].gun.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }*/

            Transform[] el1 = equipmentSlots[0].equipment.equipmentVisual.GetComponentsInChildren<Transform>();
            Transform[] el2 = equipmentSlots[1].equipment.equipmentVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < el1.Length; i++) el1[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < el2.Length; i++) el2[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            Transform[] el3 = equipmentSlots[0].equipment.handsVisual.GetComponentsInChildren<Transform>();
            Transform[] el4 = equipmentSlots[1].equipment.handsVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < el3.Length; i++) el3[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < el4.Length; i++) el4[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            Transform[] el5 = equipmentSlots[0].equipment.thirdPersonHandsVisual.GetComponentsInChildren<Transform>();
            Transform[] el6 = equipmentSlots[1].equipment.thirdPersonHandsVisual.GetComponentsInChildren<Transform>();
            for (int i = 0; i < el5.Length; i++) el5[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");
            for (int i = 0; i < el6.Length; i++) el6[i].gameObject.layer = LayerMask.NameToLayer("DefaultItem");

            if (equipmentSlots[0] != null)
            {
                Destroy(equipmentSlots[0].equipment.handsVisual);
                equipmentSlots[0].equipment.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            if (equipmentSlots[1] != null)
            {
                Destroy(equipmentSlots[1].equipment.handsVisual);
                equipmentSlots[1].equipment.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        }
        if (player.pv.IsMine)
        {
            for (int i = 0; i < 2; i++)
            {
                if (player.playerManager.slotHolderScript.slotWeaponData[i] != null)
                {
                    InstantiateWeapon(player.playerManager.slotHolderScript.slotWeaponData[i], i);
                    weaponSlots[i].InitializeAwake();
                    weaponSlots[i].InitializeStart();
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (player.playerManager.slotHolderScript.slotEquipmentData[i] != null)
                {
                    InstantiateEquipment(player.playerManager.slotHolderScript.slotEquipmentData[i], i);
                    equipmentSlots[i].InitializeAwake();
                    equipmentSlots[i].InitializeStart();
                }
            }
            //if (weaponSlots[0] != null) Destroy(weaponSlots[0].gun.thirdPersonHandsVisual);
            //if (weaponSlots[1] != null) Destroy(weaponSlots[1].gun.thirdPersonHandsVisual);
            if (equipmentSlots[0] != null) Destroy(equipmentSlots[0].equipment.thirdPersonHandsVisual);
            if (equipmentSlots[1] != null) Destroy(equipmentSlots[1].equipment.thirdPersonHandsVisual);
            Debug.Log("Init Start Line 36");
            EquipItem(0);
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
    /*
    void Update()
    {
        if (!player.pv.IsMine) return;

        if (player.playerManager.openedOptions || player.playerManager.openedLoadoutMenu) return;
        //
    }*/
    void Update()
    {
        if (!player.pv.IsMine) return;
        if (player.playerManager.openedOptions || player.playerManager.openedLoadoutMenu) return;
        KeySwitchWeapon();
        ScrollWheelSwitchWeapon();
    }
    void KeySwitchWeapon()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            int ke = i + 1;
            if (Input.GetKeyDown(ke.ToString()) && ke <= 2 && ke >= 1)
            {
                EquipItem(i);
            }
        }
        /*
        for (int i = 2; i < equipmentSlots.Length + 2; i++)
        {
            int ke = i + 1;
            if (Input.GetKeyDown(ke.ToString()) && ke <= 4 && ke >= 3)
            {
                EquipItem(i);
            }
        }*/
        if (Input.GetKeyDown(KeyCode.E) && equipmentSlots[0] != null)
        {
            EquipItem(2);
        }
        if (Input.GetKeyDown(KeyCode.G) && equipmentSlots[1] != null)
        {
            EquipItem(3);
        }
    }
    void ScrollWheelSwitchWeapon()
    {
        if (inversedScrollWheel)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (weaponIndex >= weaponSlots.Length - 1) EquipItem(0);
                else EquipItem(weaponIndex + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (weaponIndex <= 0) EquipItem(weaponSlots.Length - 1);
                else EquipItem(weaponIndex - 1);
            }
        }
        else
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                if (weaponIndex >= weaponSlots.Length - 1) EquipItem(0);
                else EquipItem(weaponIndex + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                if (weaponIndex <= 0) EquipItem(weaponSlots.Length - 1);
                else EquipItem(weaponIndex - 1);
            }
        }
    }
    public void EquipItem(int _index)
    {
        if (_index <= 1)
        {
            Debug.Log("Using Weapon " + _index);
            if (_index == previousWeaponIndex || _index == -1) return;
            weaponIndex = _index;
            weaponSlots[_index].gameObject.SetActive(true);
            if (previousWeaponIndex != -1)
            {
                if (previousWeaponIndex >= 2) equipmentSlots[previousWeaponIndex - 2].gameObject.SetActive(false);
                else weaponSlots[previousWeaponIndex].gameObject.SetActive(false);
            }
            previousWeaponIndex = weaponIndex;

            if (player.pv.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponIndex", weaponIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
        else
        {
            if (equipmentSlots[_index - 2].equipment.stats.count < 1) return;
            Debug.Log("Using Equipment " + _index);
            if (_index == previousWeaponIndex || _index == -1) return;
            previousIndex = weaponIndex;
            weaponIndex = _index;
            equipmentSlots[_index - 2].gameObject.SetActive(true);
            if (previousWeaponIndex != -1)
            {
                if (previousWeaponIndex < 2) weaponSlots[previousWeaponIndex].gameObject.SetActive(false);
                else equipmentSlots[previousWeaponIndex - 2].gameObject.SetActive(false);
            }
            previousWeaponIndex = weaponIndex;

            if (player.pv.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("weaponIndex", weaponIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
    }
    public bool InstantiateWeapon(WeaponData data, int index)
    {
        if (weaponSlots.Length > 2) return false;
        GameObject temp = Instantiate(data.weaponPrefab, transform);
        temp.GetComponent<GunManager>().player = player;
        weaponSlots[index] = temp.GetComponent<GunManager>();
        weaponSlots[index].gun = temp.GetComponent<GunManager>();
        weaponSlots[index].item = temp;
        weaponSlots[index].itemData = data;
        weaponSlots[index].InitializeAwake();
        weaponSlots[index].InitializeStart();
        weaponSlots[index].gun.SetFirstPersonViewHandsMaterial(player.local_handMaterial);
        weaponSlots[index].gun.SetThirdPersonViewHandsMaterial(player.global_handMaterial);
        weaponSlots[index].item.SetActive(false);
        EquipItem(index);
        temp.GetComponent<GunManager>().attachment.EnableGunCustomizations(index);
        return true;
    }
    public bool InstantiateEquipment(EquipmentData data, int index)
    {
        if (equipmentSlots.Length > 2) return false;
        GameObject temp = Instantiate(data.equipmentPrefab, transform);
        temp.GetComponent<EquipmentManager>().player = player;
        equipmentSlots[index] = temp.GetComponent<EquipmentManager>();
        equipmentSlots[index].equipment = temp.GetComponent<EquipmentManager>();
        equipmentSlots[index].item = temp;
        equipmentSlots[index].itemData = data;
        equipmentSlots[index].InitializeAwake();
        equipmentSlots[index].InitializeStart();
        equipmentSlots[index].equipment.inEquipmentState = index;
        equipmentSlots[index].equipment.SetFirstPersonViewHandsMaterial(player.local_handMaterial);
        equipmentSlots[index].equipment.SetThirdPersonViewHandsMaterial(player.global_handMaterial);
        equipmentSlots[index].item.SetActive(false);
        EquipItem(index + 2);
        return true;
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!player.pv.IsMine && targetPlayer == player.pv.Owner && changedProps.ContainsKey("weaponIndex"))
        {
            EquipItem((int)changedProps["weaponIndex"]);
            //player.playerManager.slotHolderScript.slotWeaponData[(int)changedProps["weaponDataChangedMode"]] = GlobalDatabase.singleton.allWeaponDatas[(int)changedProps["weaponDataChanged"]];
        }
    }
    public void WeaponFunction()
    {
        if (!player.pv.IsMine) return;
        if (weaponIndex < 2) weaponSlots[weaponIndex].Use();
        else equipmentSlots[weaponIndex - 2].Use();
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
