using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDatabase : MonoBehaviour
{
    
    public List<WeaponData> allWeaponDatas = new List<WeaponData>();
    public static GlobalDatabase singleton;
    public CurrentMatchManager matchManager;
    private void Awake()
    {
        singleton = this;
        matchManager = FindObjectOfType<CurrentMatchManager>();
    }
}
