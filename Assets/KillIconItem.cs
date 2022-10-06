using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillIconItem : MonoBehaviour
{
    public Image killIcon;
    public Image killCross;
    public void SetInfo(Sprite icon, Color col, Color crl)
    {
        killIcon.sprite = icon;
        killIcon.color = col;
        killCross.color = crl;
    }
}
