using UnityEngine;
using Photon.Pun;

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
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = (T)this;
        }
    }

}