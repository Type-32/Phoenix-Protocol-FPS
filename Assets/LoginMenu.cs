using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.MieServices;
using PrototypeLib.OnlineServices.OAuthentication;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] InputField username, password;
    [SerializeField] Text passwordDisplay;
    private int lastCount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    public async Task<bool> TryLogin()
    {
        MieClient client = new();
        OAuth2.ClientId = username.text;
        OAuth2.ClientSecret = password.text;
        var retrieve = await OAuth2.GetLambBridgeAccessTokenAsync();
        if (retrieve != null) { client.SetAccessToken(retrieve); return true; }
        return false;
    }
    public async void OnClickLogin()
    {
        var state = await TryLogin();
        Debug.Log(state ? "Success!" : "Failure.");
    }
    public void OnChangedPasswordCharacter(string content)
    {
        //Debug.Log(content);
        passwordDisplay.text = "";
        for (int i = 0; i < content.Length; i++) passwordDisplay.text += "*";
    }
}
