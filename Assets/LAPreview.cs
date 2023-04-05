using UnityEngine;
using UnityEngine.UI;

public class LAPreview : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Text txt;
    public void SetInfo(WeaponAttachmentData data)
    {
        icon.sprite = data.attachmentIcon;
        txt.text = data.attachmentName;
    }
    public void SetIcon(Image i)
    {
        icon = i;
    }
}
