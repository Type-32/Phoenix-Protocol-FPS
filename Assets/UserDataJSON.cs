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
    /*
    internal UserDataJSON()
    {
        username = "Player " + (2022).ToString();
        hasInitialized = false;
        userLevel = 1;
        userLevelXP = 0;
        userCoins = 1000;
    }*/
}
