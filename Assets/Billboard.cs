using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    Camera cam;
    private void Update()
    {
        if(cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
        if (cam == null) return;
        transform.LookAt(cam.transform.position);
        transform.Rotate(Vector3.up * 180);
    }
}
