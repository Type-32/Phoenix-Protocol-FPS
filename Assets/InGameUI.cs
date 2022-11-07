using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public Transform killMSGHolder;
    public GameObject killMSGPrefab;
    public GameObject FreeForAllUI;
    public GameObject TeamDeathMatchUI;
    public GameObject KingOfTheHillUI;
    public GameObject DropZonesUI;
    public GameObject MatchFinishUI;

    [Space]
    [Header("Main References")]
    public Text endMatchMessage;

    [Space]
    [Header("FFA References")]
    public Text topPlayerName;
    public Text topPlayerScore;
    public Text timeText;
    public Text requirementText;

    private void Awake()
    {
        instance = this;
    }
    public Sprite FindWeaponIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.singleton.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.singleton.allWeaponDatas[i].itemIcon;
        }
        return null;
    }
    public void SetMatchEndMessage(string msg)
    {
        endMatchMessage.text = msg;
    }
    public void ToggleMatchEndUI(bool toggle)
    {
        MatchFinishUI.SetActive(toggle);
    }
    public void ToggleTDM_UI(bool toggle)
    {
        TeamDeathMatchUI.SetActive(toggle);
    }
    public void ToggleFFA_UI(bool toggle)
    {
        FreeForAllUI.SetActive(toggle);
    }
    public void ToggleKOTH_UI(bool toggle)
    {
        KingOfTheHillUI.SetActive(toggle);
    }
    public void ToggleDZ_UI(bool toggle)
    {
        DropZonesUI.SetActive(toggle);
    }
    public void SetFFAMaxKillRequirement(int amount)
    {
        requirementText.text = "Get " + amount + " kills to win the game!";
    }
}
