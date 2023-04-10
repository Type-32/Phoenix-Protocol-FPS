using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;
    public CurrentMatchManager matchManager;
    public Transform killMSGHolder;
    public GameObject killMSGPrefab;
    public GameObject FreeForAllUI;
    public GameObject TeamDeathMatchUI;
    public GameObject KingOfTheHillUI;
    public GameObject DropZonesUI;
    public GameObject MatchFinishUI;
    public GameObject MatchFinishStatsUI;
    public Animator UIAnimator;

    [Space]
    [Header("Main References")]
    public Text endMatchMessage;
    public Text KDRatio;
    public Text playerName;
    public Text totalKills;
    public Text totalDeaths;
    public Text totalGainedXP;
    public Text totalGainedCoins;
    public Slider XPSlider;
    public Text levelText;

    [Space]
    [Header("FFA References")]
    public Text topPlayerName;
    public Text topPlayerScore;
    public Text timeText;
    public Text requirementText;
    float sliderXPTemp;

    [Space, Header("TDM References")]
    public GameObject TDMEndMatchUI;
    public GameObject TDMEndMatchUI_Win;
    public GameObject TDMEndMatchUI_Lose;
    public Text blueTeamKillsText;
    public Text redTeamKillsText;
    public Slider blueTeamKillsSlider;
    public Slider redTeamKillsSlider;

    bool endMatchMenuEnabled = false;

    private void Awake()
    {
        instance = this;
        matchManager = FindObjectOfType<CurrentMatchManager>();
        requirementText.text = "Get " + ((int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.MaxKillLimit]).ToString() + " kills to win the game!";
        redTeamKillsSlider.maxValue = blueTeamKillsSlider.maxValue = (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomKeys.MaxKillLimit];
        redTeamKillsSlider.minValue = blueTeamKillsSlider.minValue = 0;
    }
    private void Update()
    {
        if (endMatchMenuEnabled)
        {
            XPSlider.value = Mathf.Lerp(XPSlider.value, sliderXPTemp, Time.deltaTime * 0.8f);
        }
    }
    public Sprite FindWeaponIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allWeaponDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.Instance.allWeaponDatas[i].itemIcon;
        }
        return null;
    }
    public Sprite FindEquipmentIcon(int index)
    {
        for (int i = 0; i < GlobalDatabase.Instance.allEquipmentDatas.Count; i++)
        {
            if (i == index) return GlobalDatabase.Instance.allEquipmentDatas[i].itemIcon;
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
    public void TDM_ToggleMatchEndUI(bool toggle, bool winnerIsTeam, bool localIsTeam)
    {
        TDMEndMatchUI.SetActive(toggle);
        if (winnerIsTeam == localIsTeam)
        {
            TDMEndMatchUI_Win.SetActive(toggle);
        }
        else
        {
            TDMEndMatchUI_Lose.SetActive(toggle);
        }
    }
    public void ToggleTDM_UI(bool toggle)
    {
        TeamDeathMatchUI.SetActive(toggle);
    }
    public void ToggleFFA_UI(bool toggle)
    {
        FreeForAllUI.SetActive(toggle);
    }
    public void ToggleCTF_UI(bool toggle)
    {
        KingOfTheHillUI.SetActive(toggle);
    }
    public void ToggleDZ_UI(bool toggle)
    {
        DropZonesUI.SetActive(toggle);
    }
    public void ToggleMatchEndStats(bool toggle, float delay)
    {
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(TGL_MatchEndStats(toggle, delay));
        endMatchMenuEnabled = toggle;
    }
    IEnumerator TGL_MatchEndStats(bool toggle, float delay)
    {
        yield return new WaitForSeconds(delay);
        MatchFinishStatsUI.SetActive(toggle);
    }
    public void SetFFAMaxKillRequirement(int amount)
    {
        requirementText.text = "Get " + amount + " kills to win the game!";
    }
    public void SetMatchEndStats(string playerName, int totalKills, int totalDeaths, int totalGainedXP, int totalGainedCoins, int level, int xp)
    {
        this.totalKills.text = "Total Kills: " + totalKills.ToString();
        this.totalDeaths.text = "Total Deaths: " + totalDeaths.ToString();
        KDRatio.text = "K/D: " + ((float)totalKills / (float)totalDeaths).ToString();
        this.totalGainedCoins.text = "Resulting Money Gained: " + totalGainedCoins.ToString();
        this.totalGainedXP.text = "Gained XP in match: " + totalGainedXP.ToString();
        this.XPSlider.value = (float)xp / (float)(level * UserDatabase.Instance.levelLimiter);
        this.levelText.text = (totalGainedXP + xp >= level * UserDatabase.Instance.levelLimiter) ? ("Level " + level.ToString() + " > " + UserDatabase.Instance.GetUserXPLevelValue().ToString()) : ("Level " + level.ToString());
        sliderXPTemp = ((float)(totalGainedXP + xp) / ((float)level * UserDatabase.Instance.levelLimiter));
    }
    public void OnLeaveButtonClick()
    {
        matchManager.QuitFromMatchEnd();
    }
    public void UpdateTDMData(int blueKills, int redKills, int maxKills)
    {
        blueTeamKillsText.text = blueKills.ToString();
        blueTeamKillsSlider.value = blueKills;
        redTeamKillsText.text = redKills.ToString();
        redTeamKillsSlider.value = redKills;
        redTeamKillsSlider.maxValue = blueTeamKillsSlider.maxValue = maxKills;
        redTeamKillsSlider.minValue = blueTeamKillsSlider.minValue = 0;
    }
}
