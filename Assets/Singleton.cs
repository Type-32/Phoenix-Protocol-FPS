using UnityEngine;
using Photon.Pun;
using System;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static int AuthorityNumber = 0;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            if (AuthorityNumber == 0)
            {
                Destroy(gameObject);
                return;
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
    protected virtual void Start()
    {
        AuthorityNumber++;
    }
}
public class PunCallbackSingleton<T> : MonoBehaviourPunCallbacks where T : PunCallbackSingleton<T>
{
    private static T instance;
    public static int AuthorityNumber = 0;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            if (AuthorityNumber == 0)
            {
                Destroy(gameObject);
                return;
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
    protected virtual void Start()
    {
        AuthorityNumber++;
    }
}