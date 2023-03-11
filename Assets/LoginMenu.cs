using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.Authentication;

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
        var retrieve = await OAuth2.GetAccessToken(username.text, password.text);
        if (retrieve != null) { return true; }
        // TODO: Add SDK impl
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
