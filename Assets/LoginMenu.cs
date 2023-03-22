using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.Authentication;
using PrototypeLib.OnlineServices.LambConnector;
using PrototypeLib.Modules.FileOperations.IO;
using UserConfiguration;

public class LoginMenu : MonoBehaviour
{
    public Animator loginMenuAnimator;
    [SerializeField] InputField username, password;
    [SerializeField] Text passwordDisplay;
    private int lastCount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    public async Task<bool> TryLogin()
    {
        UserDataJSON udj = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
        var retrieve = await OAuth2.GetAccessToken(username.text, password.text);
        Debug.Log(retrieve);
        if (retrieve != null)
        {
            udj.accessToken = retrieve;
            FileOps<UserDataJSON>.WriteFile(udj, UserSystem.UserDataPath);
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

        }
    }
    public void OnChangedPasswordCharacter(string content)
    {
        //Debug.Log(content);
        passwordDisplay.text = "";
        for (int i = 0; i < content.Length; i++) passwordDisplay.text += "*";
    }
}
