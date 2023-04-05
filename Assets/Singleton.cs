using UnityEngine;
using Photon.Pun;
using System;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = (T)this;
        }
    }

}
public class PunCallbackSingleton<T> : MonoBehaviourPunCallbacks where T : PunCallbackSingleton<T>
{
    private static T instance;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            T[] tmp = FindObjectsOfType<T>();
            for (int i = 0; i < tmp.Length; i++)
            {
                if (tmp[i].gameObject != gameObject)
                {
                    Destroy(tmp[i].gameObject);
                }
            }
            DontDestroyOnLoad(gameObject);
            instance = (T)this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = (T)this;
        }
    }

}