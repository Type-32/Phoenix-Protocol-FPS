using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[System.Serializable]
public class UserDataJSON
{
    public bool hasInitialized;
    public string username;
    public int userLevel;
    public int userLevelXP;
    public int userCoins;
    public ShopDataJSON shopData;
    public UserDataJSON()
    {
        username = "Player " + (2023).ToString();
        hasInitialized = false;
        userLevel = 1;
        userLevelXP = 0;
        userCoins = 1000;
    }
}
[System.Serializable]
public class UserProfileDataJSON
{
    public string description;
    public Sprite userPFP;
    public int totalKills, totalDeaths, totalXPGained;
    public UserProfileDataJSON()
    {
        description = "Legend!";
        userPFP = null;
        totalKills = 0;
        totalDeaths = 0;
        totalXPGained = 0;
    }
}
