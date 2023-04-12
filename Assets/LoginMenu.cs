using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.Authentication;
using PrototypeLib.OnlineServices.LambConnector;
using PrototypeLib.Modules.FileOperations.IO;
using UserConfiguration;
using System;

public class LoginMenu : MonoBehaviour
{
    public Animator loginMenuAnimator;
    [SerializeField] InputField username, password;
    [SerializeField] Text passwordDisplay, exceptionText;
    // Start is called before the first frame update
    async void Start()
    {
        //TODO Recheck Cloud Saves Code For Later
        return;

        UserDataJSON ludj = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath), oudj;
        if (string.IsNullOrEmpty(ludj.AccessToken))
        {
            gameObject.SetActive(true);
        }
        else
        {
            try
            {
                oudj = await Identities.ReadIdentity<UserDataJSON>(ludj.AccessToken);
            }
            catch (Exception e)
            {

            }
        }
    }

    public async Task<bool> TryLogin()
    {
        UserDataJSON udj = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        UserDataJSON oudj = await OnlineServicesManager.LoginAndRetrieveData<UserDataJSON>(username.text, password.text, UserSystem.UserDataPath);
        if (udj.Equals(oudj))
        {
            Debug.Log("User Data JSON is the same as Cloud Save User Data JSON.");
            return true;
        }
        else
        {
            Debug.Log("User Data JSON is NOT same as Cloud Save User Data JSON.");
            return true;
        }
        // TODO: Add SDK impl
        return false;
    }
    public async void OnClickLogin()
    {
        bool state = await TryLogin();
        Debug.Log(state ? "Success!" : "Failure.");
        if (state)
        {
            await Identities.ReadIdentity<UserDataJSON>(OnlineServicesManager.AccessToken);
        }
    }
    public void OnChangedPasswordCharacter(string content)
    {
        //Debug.Log(content);
        passwordDisplay.text = "";
        for (int i = 0; i < content.Length; i++) passwordDisplay.text += "*";
    }
}
