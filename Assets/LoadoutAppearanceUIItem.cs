using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutAppearanceUIItem : MonoBehaviour
{
    [HideInInspector] public LoadoutSelectionScript script;
    public WeaponAppearanceMeshData weaponAppearanceData;
    public Image icon;
    public Text text;
    //public int attachmentGlobalIndex = 0;
    private void Awake()
    {
        script = GetComponentInParent<LoadoutSelectionScript>();
    }
    public void SetInfo(WeaponAppearanceMeshData data)
    {
        weaponAppearanceData = data;
        icon.sprite = data.itemIcon;
        text.text = data.itemName;
        //attachmentGlobalIndex = FindIndexFromData(data);
    }
    public int FindIndexFromData(WeaponAppearanceMeshData data)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponAppearanceDatas.Count; i++)
        {
            if (GlobalDatabase.Instance.allWeaponAppearanceDatas[i] == data) return i;
        }
        return -1;
    }
    public void OnButtonClick()
    {
        script.loadoutCustomization.ToggleAllAttachmentUI(false);
        script.ToggleCustomizeButtonsUI(true);
        script.ToggleCustomizeSelectionUI(false);
        script.loadoutDataList[script.selectedLoadoutIndex].SetAppearance(weaponAppearanceData, script.forSelectedSlot);
        script.customButtonsHolder.SetAllIcons(script.forSelectedSlot);
        script.WriteLoadoutDataToJSON();
        Launcher.Instance.SetLoadoutValuesToPlayer();
    }
}
