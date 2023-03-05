using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItemHolder : MonoBehaviour
{
    public delegate void SelectMapItem(bool active, int id);
    public event SelectMapItem OnSelectMapItem;
    public static MapListItemHolder Instance;
    public int selectedMapIndex;

    [SerializeField] List<MapListItem> items = new List<MapListItem>();
    [SerializeField] GameObject mapListItemPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        selectedMapIndex = -1;
        if (Launcher.Instance.mapItemInfo.Count != 0)
        {
            for (int i = 0; i < Launcher.Instance.mapItemInfo.Count; i++)
            {
                MapListItem temp = Instantiate(mapListItemPrefab, transform).GetComponent<MapListItem>();
                temp.SetInfo(Launcher.Instance.mapItemInfo[i], i + 1);
            }
        }
        OnSelectMapItem?.Invoke(false, -1);
        Debug.Log("Instantiated Map Choices");
    }
    public void SetSelectedMap(int index)
    {
        selectedMapIndex = index;
        OnSelectMapItem?.Invoke(false, -1);
        OnSelectMapItem?.Invoke(true, index);
    }
}
