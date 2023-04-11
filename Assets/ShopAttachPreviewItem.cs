using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopAttachPreviewItem : MonoBehaviour
{
    ShopMenuScript script;
    [SerializeField] Image icon;
    public void SetInfo(Sprite icon, ShopMenuScript sms)
    {
        script = sms;
        this.icon.sprite = icon;
    }
}
