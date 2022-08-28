using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDatabase : MonoBehaviour
{
    
    public List<WeaponData> allWeaponDatas = new List<WeaponData>();
    public static GlobalDatabase globalDatabase;
    private void Awake()
    {
        globalDatabase = this;
    }

}
