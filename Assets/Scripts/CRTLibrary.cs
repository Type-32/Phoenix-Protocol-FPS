using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using Unity.Mathematics;

namespace PrototypeLib
{
    namespace Multiplayer
    {
        using Photon;
        using Photon.Pun;
        using Photon.Realtime;
        using Hashtable = ExitGames.Client.Photon.Hashtable;
        namespace LocalPlayerIO
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
                    encode = null;
                }
                public WritingData(string fp, bool iie, bool jf, bool cj, bool oe, Encoding ec)
                {
                    filePath = fp;
                    initializeIfEmpty = iie;
                    jsonFormat = jf;
                    cleanJson = cj;
                    overwriteExisted = oe;
                    encode = ec;
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
                    encode = null;
                }
                public ReadingData(string filePath, bool initializeIfEmpty, bool convertFromJson, Encoding encode)
                {
                    this.filePath = filePath;
                    this.initializeIfEmpty = initializeIfEmpty;
                    this.convertFromJson = convertFromJson;
                    this.encode = encode;
                }
            }
            public static class FileOps<T> where T : new()
            {
                public delegate void FileOperate();
                public delegate void FileOperateAsync();
                public static event FileOperate OperatedFile;
                public static event FileOperateAsync OperatedFileAsync;
                public async static Task<bool> WriteFileAsync(T content, WritingData data) { return await WriteFileAsync(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode); }
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
                            success = await ImprintToFileAsync(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode));
                    }
                    else
                    {
                        Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                    }
                    return success;
                }
                public static bool WriteFile(T content, WritingData data) { return WriteFile(content, data.filePath, data.initializeIfEmpty, data.jsonFormat, data.cleanJson, data.overwriteExisted, data.encode); }
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
                            success = ImprintToFile(content, new WritingData(filePath, initializeIfEmpty, jsonFormat, cleanJson, overwriteExisted, encode));
                    }
                    else
                    {
                        Debug.LogWarning($"The File Path {filePath} does not exist. Please make sure the file is created and initialized.");
                    }
                    return success;
                }
                public async static Task<object> ReadFileAsync(ReadingData data) { return await ReadFileAsync(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode); }
                public async static Task<object> ReadFileAsync(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                {
                    object obj = null;
                    if (typeof(T) == null) return false;
                    if (File.Exists(filePath))
                    {
                        obj = await File.ReadAllTextAsync(filePath, encode);
                        if (convertFromJson)
                            obj = JsonUtility.FromJson((string)obj, typeof(T));
                        else
                            return (string)obj;
                    }
                    else
                    {
                        if (initializeIfEmpty)
                        {
                            WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode));
                            obj = File.ReadAllText(filePath, encode);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return (string)obj;
                        }
                    }
                    OperatedFileAsync.Invoke();
                    return (T)obj;
                }
                public static object ReadFile(ReadingData data) { return ReadFile(data.filePath, data.initializeIfEmpty, data.convertFromJson, data.encode); }
                public static object ReadFile(string filePath, bool initializeIfEmpty = true, bool convertFromJson = true, Encoding encode = null)
                {
                    object obj = null;
                    if (typeof(T) == null) return false;
                    if (File.Exists(filePath))
                    {
                        obj = File.ReadAllText(filePath, encode);
                        if (convertFromJson)
                            obj = JsonUtility.FromJson((string)obj, typeof(T));
                        else
                            return (string)obj;
                    }
                    else
                    {
                        if (initializeIfEmpty)
                        {
                            WriteFile(new T(), new WritingData(filePath, initializeIfEmpty, convertFromJson, convertFromJson, convertFromJson, encode));
                            obj = File.ReadAllText(filePath, encode);
                            if (convertFromJson)
                                obj = JsonUtility.FromJson((string)obj, typeof(T));
                            else
                                return (string)obj;
                        }
                    }
                    OperatedFile.Invoke();
                    return (T)obj;
                }
                private static bool ImprintToFile(T content, WritingData data)
                {
                    try
                    {
                        if (File.Exists(data.filePath))
                        {
                            File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode);
                        }
                        else
                        {
                            if (data.initializeIfEmpty)
                            {
                                File.Create(data.filePath).Close();
                                File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode);
                            }
                            else
                            {
                                return false;
                            }
                            File.WriteAllText(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode);
                        }
                        OperatedFile.Invoke();
                    }
                    catch (Exception e)
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
                            await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode);
                        }
                        else
                        {
                            if (data.initializeIfEmpty)
                            {
                                File.Create(data.filePath).Close();
                                await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(new T(), true) : JsonUtility.ToJson(new T(), false) : new T().ToString(), data.encode);
                            }
                            else
                            {
                                return false;
                            }
                            await File.WriteAllTextAsync(data.filePath, data.jsonFormat ? data.cleanJson ? JsonUtility.ToJson(content, true) : JsonUtility.ToJson(content, false) : content.ToString(), data.encode);
                        }
                        OperatedFileAsync.Invoke();
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
    }
}