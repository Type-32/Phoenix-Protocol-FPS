using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnpointUIPoint : MonoBehaviour
{
    public SpawnpointHolder spawnpointHolder;
    public GameObject spawnpointRef;
    public Text spawnpointText;
    public Image selectionBox;
    // Start is called before the first frame update
    void Start()
    {
        spawnpointHolder = FindObjectOfType<SpawnpointHolder>();
    }
    void Awake()
    {
        spawnpointHolder = FindObjectOfType<SpawnpointHolder>();
    }
    private void Test()
    {

        var tempPos = spawnpointHolder.spawnpointCamera.WorldToScreenPoint(spawnpointRef.transform.position);

        // ?????????????§³
        var rect = transform.parent.GetComponent<RectTransform>().rect;
        tempPos.x *= (rect.width / 1024);//1024 ???????????????
        tempPos.y *= (rect.height / 1024);
        tempPos.z = 0;

        transform.localPosition = tempPos + transform.parent.localPosition; // ????????????????
    }
    // Update is called once per frame
    void Update()
    {
        Test();
    }
    public void SetPlayerSpawnpoint()
    {
        spawnpointHolder.ClearAllSpawnpointSelection();
        selectionBox.enabled = true;
        spawnpointHolder.SetSpecificSpawnpoint(spawnpointRef);
    }
    public void OffselectPlayerSpawnpoint()
    {
        selectionBox.enabled = false;
        spawnpointHolder.SetSpecificSpawnpoint(null);
    }
}
