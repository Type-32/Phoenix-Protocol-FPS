using UnityEngine;
using UnityEngine.UI;

public class LAPreview : MonoBehaviour
{
    [SerializeField] Image icon;
    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }
    public void SetIcon(Image i)
    {
        icon = i;
    }
}
