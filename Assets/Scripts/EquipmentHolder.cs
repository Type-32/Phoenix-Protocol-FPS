using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentHolder : MonoBehaviour
{
    [SerializeField] private int selectedEquipment = 0;
    [SerializeField] private UIManager uiManager;
    public List<GunManager> equipmentInHolder = new List<GunManager>();
    public List<ThrowablesManager> deployablesInHolder = new List<ThrowablesManager>();
    Color slotHUDdisabledColor;
    Color slotHUDiconDisabledColor;
    public bool inversedScrollWheel = true;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        RefreshEquipmentHolder();
        //slotHUDdisabledColor = uiManager.equipmentHUDColumnList[0].background.color;
        //slotHUDiconDisabledColor = uiManager.equipmentHUDColumnList[0].icon.color;
        SelectEquipment();
    }

    // Update is called once per frame
    void Update()
    {
        if (uiManager.openedOptions || uiManager.openedLoadoutMenu) return;
        int previousSelectedEquipment = selectedEquipment;
        if (inversedScrollWheel)
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedEquipment >= transform.childCount - 1) selectedEquipment = 0;
                else selectedEquipment++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedEquipment <= 0) selectedEquipment = transform.childCount - 1;
                else selectedEquipment--;
            }
        }
        else
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (selectedEquipment >= transform.childCount - 1) selectedEquipment = 0;
                else selectedEquipment++;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (selectedEquipment <= 0) selectedEquipment = transform.childCount - 1;
                else selectedEquipment--;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedEquipment = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) selectedEquipment = 1;
        //if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3) selectedEquipment = 2;
        //if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4) selectedEquipment = 3;

        if (previousSelectedEquipment != selectedEquipment) SelectEquipment();
    }
    public void SelectEquipment()
    {
        RefreshEquipmentHolder();
        uiManager.anim.SetTrigger("onSelectedEquipmentChange");
        int i = 0;
        foreach (Transform equipment in transform)
        {
            if (i == selectedEquipment)
            {
                equipment.gameObject.SetActive(true);
                //uiManager.equipmentHUDColumnList[i].hasReference = true;
                //EnableSlotVisuals(i);
            }
            else
            {
                //uiManager.equipmentHUDColumnList[i].hasReference = true;
                equipment.gameObject.SetActive(false);
                //DisableSlotVisuals(i);
            }
            i++;
        }
    }
    void RefreshEquipmentHolder()
    {
        equipmentInHolder.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.GetComponent<GunManager>() != null)
            {
                equipmentInHolder.Add(transform.GetChild(i).GetComponent<GunManager>());
                //uiManager.equipmentHUDColumnList[i].hasReference = true;
                //uiManager.equipmentHUDColumnList[i].icon.sprite = transform.GetChild(i).gameObject.GetComponent<ItemManager>().stats.itemData.itemIcon;
            }
        }
        
    }
    public bool InstantiateWeapon(WeaponData data)
    {
        if (equipmentInHolder.Count > 2) return false;
        GameObject temp = Instantiate(data.weaponPrefab, transform);
        equipmentInHolder.Add(temp.GetComponent<GunManager>());
        return true;
    }
    public bool InstantiateEquipment(ThrowablesData data)
    {
        if (deployablesInHolder.Count > 2) return false;
        GameObject temp = Instantiate(data.throwablesPrefab, transform);
        deployablesInHolder.Add(temp.GetComponent<ThrowablesManager>());
        return true;
    }
}
