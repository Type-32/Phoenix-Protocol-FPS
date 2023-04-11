using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrototypeLib.Modules.FileOperations.IO;
using PrototypeLib.OnlineServices.PUNMultiplayer.ConfigurationKeys;
using UserConfiguration;

public class RespawnLoadoutItemScript : MonoBehaviour
{
    [SerializeField] Text primaryText, secondaryText, eqText1, eqText2;
    [SerializeField] Image primaryImage, secondaryImage, eqImage1, eqImage2;
    public void SetInfo(int w1, int w2, int e1, int e2)
    {
        primaryText.text = GlobalDatabase.Instance.allWeaponDatas[w1].itemName;
        secondaryText.text = GlobalDatabase.Instance.allWeaponDatas[w2].itemName;
        primaryImage.sprite = GlobalDatabase.Instance.allWeaponDatas[w1].itemIcon;
        secondaryImage.sprite = GlobalDatabase.Instance.allWeaponDatas[w2].itemIcon;
    }
}
