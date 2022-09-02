using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionIndicatorScript : MonoBehaviour
{
    public TMP_Text keyHint;
    public Text hintText;
    public enum IndicatorType
    {
        PickUpItem,
        Interact,
        OpenDoor,
        PressButton,
        UseItem,
        Corrupt,
        None
    };
    public enum IndicatorKeyType
    {
        F,
        G,
        Space,
        Corrupt,
        None
    };
    public void IndicatorHint(IndicatorKeyType keyType, IndicatorType hintType)
    {

    }
}
