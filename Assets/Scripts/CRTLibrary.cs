using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Unity.Mathematics;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace PrototypeLib
{
    namespace OnlineServices
    {
        using Photon;
        using Photon.Pun;
        using Photon.Realtime;
        using Hashtable = ExitGames.Client.Photon.Hashtable;
        namespace UnityCloudServices
        {
            /*
            using Unity.Services.CloudSave;
            using Unity.Services.Authentication;
            using Unity.Services.Core;
            public static class CloudKeyConfig
            {
                public static string UserDataConfigKey { get { return "UserDataConfig"; } }
                public static string LoadoutDataConfigKey { get { return "LoadoutDataConfig"; } }
                public static string RewardDataConfigKey { get { return "RewardConfig"; } }
                public static string SettingsOptionsKey { get { return "SettingsOptions"; } }
                public static string AppearancesConfigKey { get { return "AppearancesConfig"; } }
                public static string GunsmithConfigKey { get { return "GunsmithConfig"; } }
            }
            public class CloudSavesManager<T> where T : new()
            {
                public void Save(T content, string slotName)
                {
                    // Serialize the custom class to a JSON string
                    string data = JsonUtility.ToJson(content);

                    // Save the data to the cloud
                    CloudSaveService.Instance.SaveAsync(slotName, data, OnSaveSuccess, OnSaveFailure);
                }
            }
            */
        }
        namespace PUNMultiplayer
        {
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
        namespace MieServices
        {
            using Flurl;
            using Flurl.Http;
            public struct OauthExchangeResponse
            {
                public string access_token;
                public string refresh_token;
                public long expires_in;
                public string token_type;
                public string scope;
                public string uid;
            }
            public class MieClient
            {
                private readonly Uri endpoint;
                private readonly Uri bridge;
                private readonly int project;
                private string accessToken;

                public MieClient(Uri endpoint, Uri bridge, int project)
                {
                    this.endpoint = endpoint;
                    this.bridge = bridge;
                    this.project = project;
                }
                public MieClient(int project)
                {
                    this.endpoint = new Uri("https://cloud.smartsheep.studio");
                    this.bridge = new Uri("https://lamb.smartsheep.studio");
                    this.project = project;
                }
                public MieClient()
                {
                    this.endpoint = new Uri("https://cloud.smartsheep.studio");
                    this.bridge = new Uri("https://lamb.smartsheep.studio");
                    this.project = 0;
                }

                public void SetAccessToken(string token)
                {
                    accessToken = token;
                }

                public async Task<string> ExchangeAccessToken(string id, string secret, string code)
                {
                    var res = await new Uri(bridge, "api/oauth/token")
                        .PostJsonAsync(new
                        {
                            code,
                            grant_type = "authorization_code",
                            redirect = new Uri(endpoint, "oauth/callback").ToString(),
                            client_id = id,
                            client_secret = secret,
                        })
                        .ReceiveJson<OauthExchangeResponse>();

                    return res.access_token;
                }

                public async Task<int> CountNoSQLRecords(int id)
                {
                    return await new Uri(endpoint, $"api/nosql/{id}/records/count").GetAsync().ReceiveJson<int>();
                }

                public async Task<T> GetNoSQLRecords<T>(int id, int limit, int offset, string order, string query)
                {
                    return await new Uri(endpoint, $"api/nosql/{id}/records")
                        .SetQueryParam("limit", limit)
                        .SetQueryParam("offset", offset)
                        .SetQueryParam("order", order)
                        .SetQueryParam("query", query)
                        .GetAsync()
                        .ReceiveJson<T>();
                }

                public async Task<T> GetNoSQLRecord<T>(int table, int id)
                {
                    return await new Uri(endpoint, $"api/nosql/{table}/records/{id}").GetAsync().ReceiveJson<T>();
                }

                public async Task<T> ExecuteFunction<T>(int id, object arguments)
                {
                    return await new Uri(endpoint, $"serverless-function/{id}/execute")
                        .WithOAuthBearerToken(accessToken)
                        .PostJsonAsync(arguments)
                        .ReceiveJson<T>();
                }
            }

        }
        namespace OAuthentication
        {
            using System.Net.Http;
            using System.Net.Http.Headers;
            using System.Net.Http.Formatting;
            using System.Threading.Tasks;
            public static class OAuth2
            {
                private const string LambBridgeTokenUrl = "https://lamb.smartsheep.studio/api/oauth/token";
                private const string MieCloudTokenUrl = "https://cloud.smartsheep.studio/api/oauth/token";
                public static string ClientId = "your-client-id";
                public static string ClientSecret = "your-client-secret";
                public class TokenResponse
                {
                    public string AccessToken { get; set; }
                }
                public static async Task<string> GetLambBridgeAccessTokenAsync()
                {
                    using (var client = new HttpClient())
                    {
                        var authenticationHeader = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationHeader);

                        var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

                        var response = await client.PostAsync(LambBridgeTokenUrl, formContent);
                        response.EnsureSuccessStatusCode();

                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        return tokenResponse.AccessToken;
                    }
                }
                public static async Task<string> GetMieCloudAccessTokenAsync()
                {
                    using (var client = new HttpClient())
                    {
                        var authenticationHeader = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authenticationHeader);

                        var formContent = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

                        var response = await client.PostAsync(LambBridgeTokenUrl, formContent);
                        response.EnsureSuccessStatusCode();

                        var tokenResponse = await response.Content.ReadAsAsync<TokenResponse>();
                        return tokenResponse.AccessToken;
                    }
                }
            }
        }
    }
    namespace Modules
    {
        namespace FileOpsIO
        {
            using System;
            using System.Threading.Tasks;
            using System.Collections;
            using System.Collections.Generic;
            using UnityEngine;
            using Unity.Mathematics;
            using System.Text;
            using System.IO;
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
    }
}