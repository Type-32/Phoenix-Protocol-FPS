using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopAttachPreviewItem : MonoBehaviour
{
    ShopMenuScript script;
    [SerializeField] Image icon;
    void Start()
    {
        script = FindObjectOfType<ShopMenuScript>();
    }
    public void SetInfo(Sprite icon)
    {
        this.icon.sprite = icon;
    }
}
