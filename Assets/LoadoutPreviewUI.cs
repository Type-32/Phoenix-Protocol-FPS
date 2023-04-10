using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using UserConfiguration;

public class LoadoutPreviewUI : MonoBehaviour
{
    //public Button[] buttons;
    [SerializeField] LoadoutSelectionScript loadoutSelection;
    public Text[] texts;
    public Image[] images;
    public LoadoutWeaponStatisticsDisplay[] statDisplays;
    public Transform[] attachPreviewHolders;
    public GameObject attachPreviewPrefab;
    public Image customizeWeaponIcon;
    public Sprite nullWeaponIcon;
    public Text[] e_texts;
    public Image[] e_images;

    public void SetWeaponSlotInfo(int index, WeaponData weaponData)
    {
        Debug.Log("Setting WeaponSlot Preview");
        string temp = "";
        switch (index)
        {
            case 0:
                temp = "PRIMARY - ";
                break;
            case 1:
                temp = "SECONDARY - ";
                break;
        }
        if (weaponData != null)
        {
            texts[index <= 1 ? index : 0].text = temp + weaponData.itemName;
            images[index <= 1 ? index : 0].sprite = weaponData.itemIcon;
            attachPreviewHolders[index <= 1 ? index : 0].gameObject.SetActive(true);
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index] = weaponData;
            if (index <= 1)
            {
                if (index == 0)
                {
                    loadoutSelection.selectedWeaponIndex1 = weaponData.GlobalWeaponIndex;
                }
                else
                {
                    loadoutSelection.selectedWeaponIndex2 = weaponData.GlobalWeaponIndex;
                }
            }
            statDisplays[index <= 1 ? index : 0].SetInfo(weaponData);
        }
        else
        {
            texts[index <= 1 ? index : 0].text = temp + "None";
            images[index <= 1 ? index : 0].sprite = nullWeaponIcon;
            //attachPreviewHolders[index <= 1 ? index : 0].gameObject.SetActive(false);
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index] = null;
            statDisplays[index <= 1 ? index : 0].SetInfo(0, 0, 0);
        }
        SetPreviewCustomizations(index, weaponData);
        loadoutSelection.WriteLoadoutDataToJSON();
    }
    public void SetEquipmentSlotInfo(int index, EquipmentData equipmentData)
    {
        Debug.Log("Setting EquipmentSlot Preview");
        string temp = "";
        switch (index)
        {
            case 0:
                temp = "EQUIPMENT 1 - ";
                break;
            case 1:
                temp = "EQUIPMENT 2 - ";
                break;
        }
        if (equipmentData != null)
        {
            e_texts[index <= 1 ? index : 0].text = temp + equipmentData.itemName;
            e_images[index <= 1 ? index : 0].sprite = equipmentData.itemIcon;
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[index] = equipmentData;
            if (index <= 1)
            {
                if (index == 0)
                {
                    loadoutSelection.selectedEquipmentIndex1 = equipmentData.GlobalEquipmentIndex;
                }
                else
                {
                    loadoutSelection.selectedEquipmentIndex2 = equipmentData.GlobalEquipmentIndex;
                }
            }
        }
        else
        {
            e_texts[index <= 1 ? index : 0].text = temp + "None";
            e_images[index <= 1 ? index : 0].sprite = nullWeaponIcon;
            //attachPreviewHolders[index <= 1 ? index : 0].gameObject.SetActive(false);
            loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].equipmentData[index] = null;
        }
        loadoutSelection.WriteLoadoutDataToJSON();
    }
    public void ClearPreviewAttachments(int index)
    {
        int tmp = (index <= 1 ? index : 0);
        Transform[] ar = attachPreviewHolders[tmp].GetComponentsInChildren<Transform>();
        for (int i = 0; i < ar.Length; i++)
        {
            if (ar[i] != attachPreviewHolders[tmp])
            {
                Destroy(ar[i].gameObject);
            }
        }
    }
    public void ClearPreviewAppearances(int index)
    {
        int tmp = (index <= 1 ? index : 0);
        images[tmp].sprite = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[tmp].itemIcon;
    }
    public void SetPreviewCustomizations(int index, WeaponData weaponData)
    {
        Debug.Log("Setting Attachments Preview");
        int tmp = (index <= 1 ? index : 0);
        Transform[] ar = attachPreviewHolders[tmp].GetComponentsInChildren<Transform>();
        for (int i = 0; i < ar.Length; i++)
        {
            if (ar[i] != attachPreviewHolders[tmp])
            {
                Destroy(ar[i].gameObject);
            }
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetInfo(GlobalDatabase.Instance.allWeaponAttachmentDatas[loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedBarrelIndex[tmp]]);
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetInfo(GlobalDatabase.Instance.allWeaponAttachmentDatas[loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelLeftIndex[tmp]]);
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetInfo(GlobalDatabase.Instance.allWeaponAttachmentDatas[loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSidebarrelRightIndex[tmp]]);
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetInfo(GlobalDatabase.Instance.allWeaponAttachmentDatas[loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedSightIndex[tmp]]);
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[tmp] != -1)
        {
            LAPreview i = Instantiate(attachPreviewPrefab, attachPreviewHolders[tmp]).GetComponentInChildren<LAPreview>();
            i.SetInfo(GlobalDatabase.Instance.allWeaponAttachmentDatas[loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedUnderbarrelIndex[tmp]]);
        }
        if (loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[tmp] != -1)
        {
            images[tmp].sprite = FindAppearanceIcon(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].selectedAppearanceDataIndex[tmp]);
        }
    }
    public void SetPreviewInfo(LoadoutData data)
    {
        SetWeaponSlotInfo(0, data.weaponData[0]);
        SetWeaponSlotInfo(1, data.weaponData[1]);
        SetEquipmentSlotInfo(0, data.equipmentData[0]);
        SetEquipmentSlotInfo(1, data.equipmentData[1]);
    }
    public void SetSelectionMenuSlotIndex(int index)
    {
        loadoutSelection.forSelectedSlot = index;
    }
    Sprite FindAttachmentIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponAttachmentDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.Instance.allWeaponAttachmentDatas[i].attachmentIcon;
        }
        return null;
    }
    Sprite FindAppearanceIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponAppearanceDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.Instance.allWeaponAppearanceDatas[i].itemIcon;
        }
        return null;
    }
    public void OnCustomizeButtonPress(int index)
    {
        SetSelectionMenuSlotIndex(index);
        loadoutSelection.customButtonsHolder.SetAllIcons(loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutCustomization.RemoveCustomizationUIItems();
        loadoutSelection.loadoutCustomization.AttachmentUIItemInstantiation();
        loadoutSelection.loadoutCustomization.AppearanceUIItemInstantiation();
        loadoutSelection.ToggleCustomizationMenu(true);
        loadoutSelection.ToggleCustomizeButtonsUI(true);
        loadoutSelection.ToggleCustomizeSelectionUI(false);
        //loadoutSelection.DisablePreview();
        //loadoutSelection.CloseLoadoutButtonsVisual();
        customizeWeaponIcon.sprite = loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index].itemIcon;
    }
    public void QuitCustomizationUI()
    {
        loadoutSelection.ToggleCustomizationMenu(false);
        loadoutSelection.ToggleCustomizeButtonsUI(false);
        loadoutSelection.EnablePreview();
        //loadoutSelection.OpenLoadoutButtonsVisual();
        SetPreviewInfo(loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex]);
    }
}
