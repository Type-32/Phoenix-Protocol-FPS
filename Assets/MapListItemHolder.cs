using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapListItemHolder : MonoBehaviour
{
    public static MapListItemHolder Instance;
    public int selectedMapIndex;

    [SerializeField] List<MapListItem> items = new List<MapListItem>();
    [SerializeField] GameObject mapListItemPrefab;
    [SerializeField] Text selectedMapName;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (Launcher.Instance.mapItemInfo.Count != 0)
        {
            for (int i = 0; i < Launcher.Instance.mapItemInfo.Count; i++)
            {
                MapListItem temp = Instantiate(mapListItemPrefab, transform).GetComponent<MapListItem>();
                temp.SetInfo(Launcher.Instance.mapItemInfo[i], i + 1);
            }
        }
    }
    public void SetSelectedMap(int index, string mapName)
    {
        selectedMapIndex = index;
        MainMenuUIManager.instance.OnSelectedMap(mapName);
    }
}
