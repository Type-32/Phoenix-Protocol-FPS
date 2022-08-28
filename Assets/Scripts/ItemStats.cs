using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
    public ItemData itemData;

    public float swayIntensity = 0.2f;
    public float maxSwayIntensity = 1f;
    public float rotSwayIntensity = 0.3f;
    public float maxRotSwayIntensity = 2f;

    public float swayIntensityValve = 0.3f;
    public float maxSwayIntensityValve = 1f;
    public float rotSwayIntensityValve = 0.5f;
    public float maxRotSwayIntensityValve = 2f;

    public float smoothness = 8f;
    public float rotSmoothness = 6f;

}
