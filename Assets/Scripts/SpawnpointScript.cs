using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointScript : MonoBehaviour
{
    [SerializeField] GameObject visuals;
    private void Awake()
    {
        visuals.SetActive(false);
    }
}
