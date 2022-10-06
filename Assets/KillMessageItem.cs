using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillMessageItem : MonoBehaviour
{
    [SerializeField] Text killed;
    [SerializeField] Text killer;
    [SerializeField] Image icon;
    public void SetInfo(string killedName, string killerName, Sprite img)
    {
        killed.text = killedName;
        killer.text = killerName;
        icon.sprite = img;
    }
}
