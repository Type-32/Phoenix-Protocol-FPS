using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindowItem : MonoBehaviour
{
    public Text title;
    public Text content;
    public void SetInfo(string t, string c)
    {
        title.text = t;
        content.text = c;
    }
    public void OnClickClose()
    {
        //MainMenuUIManager.instance.RemovePopup(this);
    }
}
