using System.Security.AccessControl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototypeLib
{
    namespace OnlineServices
    {
        using Photon.Pun;
        using System.Net.Http;
        using System.Text;
        using System.Text.Json;
        using System.Text.Json.Nodes;
        using Hashtable = ExitGames.Client.Photon.Hashtable;
        using UnityEngine;
        namespace PUNMultiplayer
        {
            namespace ConfigurationKeys
            {
                public static class LoadoutKeys
                {
                    public static string SelectedWeaponIndex(int slot) => $"selectedWeaponIndex{slot}";
                    public static string SelectedEquipmentIndex(int slot) => $"selectedEquipmentIndex{slot}";
                    public static string SelectedWeaponCustomization(AttachmentTypes type, int slot) => $"SMWA_{type.ToString()}Index{slot}";
                    public static string SelectedWeaponAppearance(int slot) => $"SMWA_AppearanceIndex{slot}";
                }
                public static class RoomKeys
                {
                    //"roomName", "roomHostName", "mapInfoIndex", "maxPlayer", "gameStarted", "randomRespawn", "roomMode", "roomMapIndex", "roomVisibility", "roomCode", "maxKillLimit", "allowDownedState"
                    public static string RoomName => "roomName";
                    public static string RoomHostName => "roomHostName";
                    public static string MapInfoIndex => "mapInfoIndex";
                    public static string MaxPlayer => "maxPlayer";
                    public static string GameStarted => "gameStarted";
                    public static string RandomRespawn => "randomRespawn";
                    public static string RoomMode => "roomMode";
                    public static string RoomMapIndex => "roomMapIndex";
                    public static string RoomVisibility => "roomVisibility";
                    public static string RoomCode => "roomCode";
                    public static string MaxKillLimit => "maxKillLimit";
                    public static string AllowDownedState => "allowDownedState";
                }
                public static class SynchronizationKeys
                {
                    public static string WeaponDataChangedMode => "weaponDataChangedMode";
                    public static string WeaponDataChanged => "weaponDataChanged";
                }
            }
            public static class PlayerManipulaton
            {
                public static bool Save(Hashtable h)
                {
                    return PhotonNetwork.LocalPlayer.SetCustomProperties(h);
                }
                public static bool SaveParameters(string[] keys, object[] parameters = null)
                {
                    if (parameters == null) return false;
                    if (keys.Length == 0) return false;
                    Hashtable hash = new();
                    for (int i = 0; i < keys.Length; i++)
                    {
                        hash.Add(keys[i], parameters[i]);
                    }
                    return Save(hash);
                }
                public static bool Read(Hashtable hash)
                {
                    return false;
                }
                public static bool ReadParameters(string[] keys)
                {
                    return false;
                }
            }
        }
        namespace LambConnector
        {
            using PrototypeLib.Modules.FileOperations.IO;
            using UserConfiguration;
            public static class Configuration
            {
                public const string APIUrl = "https://cloud.smartsheep.studio/api";
                public static string APIToken = "";
                public static string ProjectId = "3";
            }
            public static class Identities
            {
                public static async Task<T> ReadIdentity<T>(string AccessToken)
                {
                    var url = Configuration.APIUrl + "/users";

                    using (var client = new HttpClient())
                    {
                        var message = new HttpRequestMessage(HttpMethod.Get, url);
                        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                        var response = await client.SendAsync(message);
                        response.EnsureSuccessStatusCode();

                        var identity = (await response.Content.ReadAsAsync<JsonObject>())["identity"];
                        Debug.Log(identity);
                        return JsonSerializer.Deserialize<T>(identity?.ToString());
                    }
                }

                public static async Task SaveIdentity<T>(int identityId, T data)
                {
                    var url = Configuration.APIUrl + $"/projects/{Configuration.ProjectId}/oauth-clients/{Authentication.OAuth2.ClientId}/identities/{identityId}";

                    using (var client = new HttpClient())
                    {
                        UserDataJSON localCache = FileOps<UserDataJSON>.ReadFile(UserSystem.UserDataPath);
                        var jsonContent = new JsonObject();
                        jsonContent.Add("nickname", localCache.username);
                        jsonContent.Add("permissions", new JsonArray());
                        jsonContent.Add("data", JsonUtility.ToJson(data));

                        var message = new HttpRequestMessage(HttpMethod.Put, url);
                        message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer ", Configuration.APIToken);
                        message.Content = new StringContent(jsonContent.ToString(), Encoding.UTF8, "application/json");

                        var response = await client.SendAsync(message);
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }
        namespace Authentication
        {
            using System.Net.Http;
            using System.Threading.Tasks;
            public static class OAuth2
            {
                public const string APIUrl = "https://cloud.smartsheep.studio/o/oauth/token";
                public static string ClientId = "2";
                public static string ClientSecret = "75013fe4961749d4";
                public class TokenResponse
                {
                    public string access_token { get; set; }
                    public string refresh_token { get; set; }
                }
                public static async Task<string> GetAccessToken(string username, string password)
                {
                    using (var client = new HttpClient())
                    {
                        var formContent = new FormUrlEncodedContent(new[] {
                            new KeyValuePair<string, string>("grant_type", "password"),
                            new KeyValuePair<string, string>("username", username),
                            new KeyValuePair<string, string>("password", password),
                            new KeyValuePair<string, string>("scope", "all"),
                            new KeyValuePair<string, string>("client_id", ClientId),
                            new KeyValuePair<string, string>("client_secret", ClientSecret)
                        });

                        var response = await client.PostAsync(APIUrl, formContent);
                        response.EnsureSuccessStatusCode();

                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        return tokenResponse.access_token;
                    }
                }
            }
        }
    }
    namespace Modules
    {
        namespace FileOperations
        {
            namespace IO
            {
                using System;
                using System.IO;
                using System.Text;
                using System.Threading.Tasks;
                using UnityEngine;
                public class WritingData
                {
                    public string filePath;
                    public bool initializeIfEmpty;
                    public bool jsonFormat;
                    public bool cleanJson;
                    public bool overwriteExisted;
                    public Encoding encode;
                    public WritingData()
                    {
                        filePath = "";
                        initializeIfEmpty = true;
                        jsonFormat = true;
                        cleanJson = true;
                        overwriteExisted = true;
                        encode = Encoding.Default;
                    }
                    public WritingData(string fp, Encoding ec)
                    {
                        filePath = fp;
                        initializeIfEmpty = true;
                        jsonFormat = true;
                        cleanJson = true;
                        overwriteExisted = true;
                        encode = ec ?? Encoding.Default;
                    }
                    public WritingData(string fp, bool iie, bool jf, bool cj, bool oe, Encoding ec)
                    {
                        filePath = fp;
                        initializeIfEmpty = iie;
                        jsonFormat = jf;
                        cleanJson = cj;
                        overwriteExisted = oe;
                        encode = ec ?? Encoding.Default;
                    }
                }
                public class ReadingData
                {
                    public string filePath;
                    public bool initializeIfEmpty;
                    public bool convertFromJson;
                    public Encoding encode;
                    public ReadingData()
                    {
                        filePath = "";
                        initializeIfEmpty = true;
                        convertFromJson = true;
                        encode = Encoding.Default;
                    }
                    public ReadingData(string fp, Encoding ec)
                    {
                        filePath = fp;
                        initializeIfEmpty = true;
                        convertFromJson = true;
                        encode = ec ?? Encoding.Default;
                    }
                    public ReadingData(string filePath, bool initializeIfEmpty, bool convertFromJson, Encoding encode)
                    {
                        this.filePath = filePath;
                        this.initializeIfEmpty = initializeIfEmpty;
                        this.convertFromJson = convertFromJson;
                        this.encode = encode ?? Encoding.Default;
                    }
                }
                public static class FileOps<T> where T : new()
                {
                    public delegate void FileOperate(string strPath, T content);
                    public delegate void FileOperateAsync(string strPath, T content);
                    public static event FileOperate OperatedFile;
                    public static event FileOperateAsync OperatedFileAsync;
                    public async static Task<bool> WriteFileAsync(T content, WritingData data) { return await WriteFileAsync(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode ?? Encoding.Default); }
                    public async static Task<bool> WriteFileAsync(T content, string filePath, bool initializeIfEmpty = true, bool jsonFormat = true, bool cleanJson = true, bool overwriteExisted = true, Encoding encode = null)
                    {
                        bool success = false;
                        if (typeof(T) == null)
                        {
                            Debug.LogWarning($"The Data Type {typeof(T).FullName} is null. Please make sure the type is not null.");
                            return false;
                        }
                        if (!typeof(T).IsSerializable)
                        {
                            Debug.LogWarning($"The Data Type {typeof(T).FullName} is not serialized. Please make sure to serialize the data type before performing any writing operations regarding the type.");
                            return false;
                        }
                        if (File.Exists(filePath))
                        {
                            if (overwriteExisted)
                                success = await ImprintToFileAsync(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                        }
                        else
                        {
                            if (initializeIfEmpty) ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                            Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                        }
                        return success;
                    }
                    public static bool WriteFile(T content, WritingData data) { return WriteFile(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode ?? Encoding.Default); }
                    public static bool WriteFile(T content, string filePath, bool initializeIfEmpty = true, bool jsonFormat = true, bool cleanJson = true, bool overwriteExisted = true, Encoding encode = null)
                    {
                        bool success = false;
                        if (typeof(T) == null)
                        {
                            Debug.LogWarning($"The Data Type {typeof(T).FullName} is null. Please make sure the type is not null.");
                            return success;
                        }
                        if (!typeof(T).IsSerializable)
                        {
                            Debug.LogWarning($"The Data Type {typeof(T).FullName} is not serialized. Please make sure to serialize the data type before performing any writing operations regarding the type.");
                            return success;
                        }
                        if (File.Exists(filePath))
                        {
                            if (overwriteExisted)
                                success = ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                        }
                        else
                        {
                            if (initializeIfEmpty) ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode ?? Encoding.Default));
                            Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                        }
                        return success;
                    }
                    public async static Task<T> ReadFileAsync(ReadingData data) { return await ReadFileAsync(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode ?? Encoding.Default); }
                    public async static Task<T> ReadFileAsync(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                    {
                        object obj = null;
                        if (typeof(T) == null) return default;
                        if (File.Exists(filePath))
                        {
                            obj = await File.ReadAllTextAsync(filePath, encode ?? Encoding.Default);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return default;
                        }
                        else
                        {
                            if (initializeIfEmpty)
                            {
                                WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode ?? Encoding.Default));
                                obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                                if (convertFromJson)
                                    obj = JsonUtility.FromJson((string)obj, typeof(T));
                                else
                                    return default;
                            }
                        }
                        OperatedFileAsync?.Invoke(filePath, (T)obj);
                        return (T)obj;
                    }
                    public static T ReadFile(ReadingData data) { return ReadFile(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode ?? Encoding.Default); }
                    public static T ReadFile(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                    {
                        object obj = null;
                        if (typeof(T) == null) return default;
                        if (File.Exists(filePath))
                        {
                            obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return default;
                        }
                        else
                        {
                            if (initializeIfEmpty)
                            {
                                WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode ?? Encoding.Default));
                                obj = File.ReadAllText(filePath, encode ?? Encoding.Default);
                                if (convertFromJson)
                                    obj = JsonUtility.FromJson((string)obj, typeof(T));
                                else
                                    return default;
                            }
                        }
                        OperatedFile?.Invoke(filePath, (T)obj);
                        return (T)obj;
                    }
                    private static bool ImprintToFile(T content, WritingData data)
                    {
                        try
                        {
                            if (File.Exists(data.filePath))
                            {
                                File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                            }
                            else
                            {
                                if (data.initializeIfEmpty)
                                {
                                    File.Create(data.filePath).Close();
                                    File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode ?? Encoding.Default);
                                }
                                else
                                {
                                    return false;
                                }
                                File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                            }
                            OperatedFile?.Invoke(data.filePath, (T)content);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        return true;
                    }
                    private async static Task<bool> ImprintToFileAsync(T content, WritingData data)
                    {
                        try
                        {
                            if (File.Exists(data.filePath))
                            {
                                await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                            }
                            else
                            {
                                if (data.initializeIfEmpty)
                                {
                                    File.Create(data.filePath).Close();
                                    await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode ?? Encoding.Default);
                                }
                                else
                                {
                                    return false;
                                }
                                await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode ?? Encoding.Default);
                            }
                            OperatedFileAsync?.Invoke(data.filePath, (T)content);
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            namespace Stream
            {
                using PrototypeLib.Modules.FileOperations.IO;
                public class FileOpsClient<T> where T : new()
                {
                    public FileOpsClient(ReadingData readData, WritingData writeData)
                    {
                        ReadData = readData;
                        WriteData = writeData;
                        content = FileOps<T>.ReadFile(ReadData);
                    }
                    public T content;
                    private readonly string FilePath;
                    private readonly ReadingData ReadData;
                    private readonly WritingData WriteData;
                    public void Write()
                    {
                        FileOps<T>.WriteFile(content, WriteData);
                    }
                    public void Write(T content)
                    {
                        FileOps<T>.WriteFile(content, WriteData);
                    }
                    public void Read()
                    {
                        FileOps<T>.ReadFile(ReadData);
                    }
                }
            }
        }
    }
}