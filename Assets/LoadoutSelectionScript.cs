using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionScript : MonoBehaviour
{
    public GameObject loadoutSelectionItemPrefab;
    [Space]
    public GameObject loadoutButtonsUI;
    public Transform loadoutButtonsHolder;
    public LoadoutPreviewUI loadoutPreviewUI;
    public List<LoadoutSelectionItem> loadoutItems = new List<LoadoutSelectionItem>();
    public int selectedLoadoutIndex;

    [Space]
    [Header("More References")]
    public List<LoadoutData> loadoutDataList = new List<LoadoutData>();
    // Start is called before the first frame update
    void Start()
    {
        LoadoutSelectionItem[] tempItems = loadoutPreviewUI.GetComponentsInChildren<LoadoutSelectionItem>();
        for(int i = 0; i < tempItems.Length; i++)
        {
            loadoutItems.Add(tempItems[i]);
        }
        DisablePreview();
        InstantiateLoadoutSelections();
    }

    public void InstantiateLoadoutSelections()
    {
        for(int i = 0; i < loadoutDataList.Count; i++)
        {
            LoadoutSelectionItem temp = Instantiate(loadoutSelectionItemPrefab, loadoutButtonsHolder).GetComponent<LoadoutSelectionItem>();
            temp.itemLoadoutData = loadoutDataList[i];
            temp.DeselectLoadout();
            temp.loadoutIndex = i;
            loadoutDataList[i].loadoutIndex = i;
            temp.SetLoadoutName(loadoutDataList[i].loadoutName);
            loadoutItems.Add(temp);
            if (loadoutDataList[i].isDefault)
            {
                temp.SelectLoadout();
                temp.ToggleSelectVisual(true);
            }
        }
    }
 
    public void OnSelectLoadoutCallback(int index)
    {
        selectedLoadoutIndex = index;
        loadoutPreviewUI.SetPreviewInfo(loadoutDataList[selectedLoadoutIndex]);
        DisableAllSelectedVisuals();
        loadoutItems[selectedLoadoutIndex].ToggleSelectVisual(true);
        EnablePreview();
    }
    public void DisableAllSelectedVisuals()
    {
        for (int i = 0; i < loadoutItems.Count; i++)
        {
            loadoutItems[i].DeselectLoadout();
        }
    }
    public void DisablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(false);
    }
    public void EnablePreview()
    {
        loadoutPreviewUI.gameObject.SetActive(true);
    }
}
