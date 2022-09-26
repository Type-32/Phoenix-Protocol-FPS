using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDatabase : MonoBehaviour
{

    public List<WeaponData> allWeaponDatas = new List<WeaponData>();
    public List<WeaponAppearanceData> allWeaponAppearanceDatas = new List<WeaponAppearanceData>();
    public List<WeaponAttachmentData> allWeaponAttachmentDatas = new List<WeaponAttachmentData>();
    public List<LoadoutData> allLoadoutDatas = new List<LoadoutData>();
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
