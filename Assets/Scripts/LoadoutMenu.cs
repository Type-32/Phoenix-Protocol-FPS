using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutMenu : MonoBehaviour
{
    public UIManager ui;
    public LoadoutSlotHolder slotHolderScript;
    public ChoiceHolderScript choiceHolderScript;
    public GameObject primarySlot;
    public GameObject secondarySlot;
    public GameObject equipmentSlot1;
    public GameObject equipmentSlot2;
    public GameObject selectionMenu;
    public bool openedSelectionMenu = false;
    private void Awake()
    {
        selectionMenu.SetActive(openedSelectionMenu);
    }
    public void OpenSelectionMenu(int index)
    {
        openedSelectionMenu = true;
        choiceHolderScript.RefreshChoices(index);
        choiceHolderScript.FindElementInSelection(slotHolderScript.slotWeaponData[index]).interactable = false;
        selectionMenu.SetActive(true);
    }
    public void CloseSelectionMenu()
    {
        openedSelectionMenu = false;
        choiceHolderScript.ClearChoiceSlot();
        for (int i = 0; i < choiceHolderScript.slotScripts.Count; i++)
        {
            choiceHolderScript.slotScripts[i].gameObject.GetComponent<Button>().interactable = true;
        }
        selectionMenu.SetActive(false);
    }
    public void AssignPlayerWeapon()
    {

    }
}
