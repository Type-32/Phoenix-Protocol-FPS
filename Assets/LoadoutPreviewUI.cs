using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutPreviewUI : MonoBehaviour
{
    //public Button[] buttons;
    [SerializeField] LoadoutSelectionScript loadoutSelection;
    public Text[] texts;
    public Image[] images;

    public void SetWeaponSlotInfo(int index, WeaponData weaponData)
    {
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
        texts[index <= 1 ? index : 0].text = temp + weaponData.itemName;
        images[index <= 1 ? index : 0].sprite = weaponData.itemIcon;
        loadoutSelection.loadoutDataList[loadoutSelection.selectedLoadoutIndex].weaponData[index] = weaponData;
    }
    public void SetPreviewInfo(LoadoutData data)
    {
        SetWeaponSlotInfo(0, data.weaponData[0]);
        SetWeaponSlotInfo(1, data.weaponData[1]);
    }
    public void SetSelectionMenuSlotIndex(int index)
    {
        loadoutSelection.forSelectedSlot = index;
    }
    public void OnCustomizeButtonPress(int index)
    {
        SetSelectionMenuSlotIndex(index);
        loadoutSelection.customButtonsHolder.SetAllIcons(loadoutSelection.forSelectedSlot);
        loadoutSelection.loadoutCustomization.RemoveAttachmentUIItems();
        loadoutSelection.loadoutCustomization.AttachmentUIItemInstantiation();
        loadoutSelection.ToggleCustomizationMenu(true);
        loadoutSelection.ToggleCustomizeButtonsUI(true);
        loadoutSelection.DisablePreview();
        loadoutSelection.CloseLoadoutButtonsVisual();
    }
    public void QuitCustomizationUI()
    {
        loadoutSelection.ToggleCustomizationMenu(false);
        loadoutSelection.ToggleCustomizeButtonsUI(false);
        loadoutSelection.EnablePreview();
        loadoutSelection.OpenLoadoutButtonsVisual();
    }
}
