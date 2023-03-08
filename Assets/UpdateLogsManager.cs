using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLogsManager : MonoBehaviour
{
    public Transform updateLogsItemHolder;
    public GameObject updateLogsItemPrefab;
    private void Start()
    {
        for (int i = 0; i < GlobalDatabase.Instance.allLogDatas.Count; i++)
        {
            LogsItem temp = Instantiate(updateLogsItemPrefab, updateLogsItemHolder).GetComponent<LogsItem>();
            temp.SetInfo(GlobalDatabase.Instance.allLogDatas[i].description, GlobalDatabase.Instance.allLogDatas[i].version);
        }
    }
}
