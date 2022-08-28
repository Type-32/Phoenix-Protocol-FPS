using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunction : MonoBehaviour
{
    public ItemManager item;

    [Space]
    [Header("Togglable Item Type Parameters")]
    public GameObject togglableGameObject;
    public bool isToggled = true;
    public AudioClip toggledOnAudioClip;
    public AudioClip toggledOffAudioClip;

    [Space]
    [Header("Interactable Item Type Parameters")]
    public bool isInteracting = false;
    public AudioClip interactingAudioClip;
    void Start()
    {
        togglableGameObject.SetActive(isToggled);
    }

    // Update is called once per frame
    void Update()
    {
        if(item.stats.itemData.itemType == ItemData.ItemType.Togglable && togglableGameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isToggled)
                {
                    isToggled = false;
                    togglableGameObject.SetActive(false);

                }
                else
                {
                    isToggled = true;
                    togglableGameObject.SetActive(true);
                }
            }
        }else if(item.stats.itemData.itemType == ItemData.ItemType.Interactable)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isInteracting)
                {
                    isInteracting = false;
                }
                else
                {
                    isInteracting = true;
                }
            }
            item.anim.animate.SetBool("interactingObject", isInteracting);
        }
    }
}
