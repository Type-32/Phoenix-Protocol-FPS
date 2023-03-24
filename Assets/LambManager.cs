using UnityEngine;
using PrototypeLib.Modules.FileOperations.IO;
using UserConfiguration;
using System;
using System.Threading.Tasks;
using PrototypeLib.OnlineServices.LambConnector;
using PrototypeLib.OnlineServices.Authentication;

public class OnlineServicesManager : Singleton<OnlineServicesManager>
{
    public static string AccessToken;
    public static bool RetrievedOnlineData = false;
    protected override void Awake()
    {
        //TODO Recheck Cloud Saves Code For Later
        return;

        base.Awake();
        RetrievedOnlineData = false;
        AccessToken = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath).AccessToken;
    }
    public static async Task<T?> LoginAndRetrieveData<T>(string username, string password, string path)
    {
        UserDataJSON udj = FileOps<UserDataJSON>.ReadFile(path);
        try
        {
            var retrieve = await OAuth2.GetAccessToken(username, password);
            Debug.Log(retrieve);
            if (retrieve != null)
            {
                udj.AccessToken = OnlineServicesManager.AccessToken = Configuration.APIToken = retrieve;
                FileOps<UserDataJSON>.WriteFile(udj, path);

                T oudj = await Identities.ReadIdentity<T>(retrieve);
                if (oudj != null) return oudj;
                else
                {
                    //await Identities.SaveIdentity<>()
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Login Action Returned the following Exception: {e.Message}");
        }
        return default(T);
    }
}