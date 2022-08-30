using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Animator backgroundMask;
    public BoxCollider animEnd;
    
    public void BackgroundMaskFadeIn()
    {
        backgroundMask.SetTrigger("Fadein");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Camera>() != null) BackgroundMaskFadeIn();
    }
}
