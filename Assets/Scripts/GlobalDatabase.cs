using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDatabase : MonoBehaviour
{

    public List<WeaponData> allWeaponDatas = new();
    public List<WeaponAppearanceData> allWeaponAppearanceDatas = new();
    public List<WeaponAttachmentData> allWeaponAttachmentDatas = new();
    public List<LoadoutData> allLoadoutDatas = new();
    public List<UpdateLogData> allLogDatas = new();
    public static GlobalDatabase singleton;
    private void Awake()
    {
        if (singleton)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        singleton = this;
    }
}
