using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionIndicatorScript : MonoBehaviour
{
    public TMP_Text keyHint;
    public TMP_Text hintText;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void IndicatorHint(IndicatorKeyType keyType, IndicatorType hintType)
    {

    }
}
