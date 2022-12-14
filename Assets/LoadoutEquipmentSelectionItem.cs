using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutEquipmentSelectionItem : MonoBehaviour
{
    public LoadoutSelectionScript loadoutSelection;
    public LoadoutCustomButtonsHolder customButtonsHolder;
    public EquipmentData equipmentData;
    [SerializeField] Image equipmentIcon;
    [SerializeField] Text equipmentName;
    public int equipmentIndex;
    private void Awake()
    {
        loadoutSelection = GetComponentInParent<LoadoutSelectionScript>();
        customButtonsHolder = FindObjectOfType<LoadoutCustomButtonsHolder>();
    }

    void Start()
    {
        SetEquipmentName(equipmentData.itemName);
        SetEquipmentIcon(equipmentData.itemIcon);
    }

    public void OnClickButton()
    {
        for (int i = 0; i < GlobalDatabase.singleton.allEquipmentDatas.Count; i++)
        {
            if (GlobalDatabase.singleton.allEquipmentDatas[i] == equipmentData)
            {
                loadoutSelection.loadoutPreviewUI.SetEquipmentSlotInfo(loadoutSelection.forSelectedSlot, equipmentData);
            }
        }
        loadoutSelection.EnablePreview();
        loadoutSelection.DisableEquipmentSelection();
        loadoutSelection.OpenLoadoutButtonsVisual();
        Launcher.Instance.SetLoadoutValuesToPlayer();
        MenuManager.instance.AddNotification("Weapon Selection", "You've selected " + equipmentData.itemName + " as your " + (loadoutSelection.forSelectedSlot == 0 ? "first equipment." : "second equipment."));
    }

    public void SetEquipmentIcon(Sprite iconSprite)
    {
        equipmentIcon.sprite = iconSprite;
    }
    public void SetEquipmentName(string name)
    {
        equipmentName.text = name;
    }
}
