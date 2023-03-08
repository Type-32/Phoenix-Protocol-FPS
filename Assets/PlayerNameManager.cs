using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("Username"))
        {
            SetPlayerFieldName(PlayerPrefs.GetString("Username"));
        }
        else
        {
            string temp = "Player " + Random.Range(1000, 9999).ToString("0000");
            SetPlayerFieldName(temp);
            OnUsernameInputValueChanged();
        }
        SetPlayerName();
    }
    public void OnUsernameInputValueChanged()
    {
        SetPlayerName();
        PlayerPrefs.SetString("Username", GetPlayerFieldName());
    }
    public void SetPlayerName()
    {
        PhotonNetwork.NickName = GetPlayerFieldName();
    }


    public string GetPlayerFieldName()
    {
        return MenuManager.Instance.playerNameInputField.text;
    }
    public void SetPlayerFieldName(string value)
    {
        MenuManager.Instance.playerNameInputField.text = value;
    }
}
