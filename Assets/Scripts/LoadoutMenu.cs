using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoadoutMenu : MonoBehaviour
{
    public PlayerManager playerManager;
    public UIManager ui;
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
        playerManager.choiceHolderScript.RefreshChoices(index);
        playerManager.choiceHolderScript.FindElementInSelection(playerManager.slotHolderScript.slotWeaponData[index]).interactable = false;
        selectionMenu.SetActive(true);
    }
    public void CloseSelectionMenu()
    {
        openedSelectionMenu = false;
        playerManager.choiceHolderScript.ClearChoiceSlot();
        for (int i = 0; i < playerManager.choiceHolderScript.slotScripts.Count; i++)
        {
            playerManager.choiceHolderScript.slotScripts[i].gameObject.GetComponent<Button>().interactable = true;
        }
        selectionMenu.SetActive(false);
    }
}
