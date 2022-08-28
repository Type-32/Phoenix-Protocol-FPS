using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnpointUIPoint : MonoBehaviour
{
    public SpawnpointHolder spawnpointHolder;
    public GameObject spawnpointRef;
    public Text spawnpointText;
    // Start is called before the first frame update
    void Start()
    {
        spawnpointHolder = FindObjectOfType<SpawnpointHolder>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = spawnpointHolder.spawnpointCamera.WorldToScreenPoint(spawnpointRef.transform.position);
    }
}
