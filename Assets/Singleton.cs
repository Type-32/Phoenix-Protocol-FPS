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
            Destroy(gameObject);
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
    public int AuthorityNumber = 0;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        photonView.ViewID = 999;
        if (instance != null)
        {
            //* Code works but due to the object is attached with the photon view, the older entry object will be destroyed and replaced by the new one.
            //if (gameObject.scene.name != "DontDestroyOnLoad") Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
    protected virtual void Start()
    {
        AuthorityNumber++;
        Debug.Log($"{gameObject.name} PunCallbackSingleton has Authority of {AuthorityNumber}");
    }
}