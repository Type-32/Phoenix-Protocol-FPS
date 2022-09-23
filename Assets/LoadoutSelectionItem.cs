using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelectionItem : MonoBehaviour
{
    public LoadoutSelectionScript loadoutSelection;

    public Text loadoutSelectionItemName;
    public GameObject selectionVisual;
    public LoadoutData itemLoadoutData;
    public int loadoutIndex = 0;
    private void Awake()
    {
        loadoutSelection = GetComponentInParent<LoadoutSelectionScript>();
    }
    void Start()
    {
        DeselectLoadout();
    }
    public void ToggleSelectVisual(bool value)
    {
        selectionVisual.SetActive(value);
    }
    // Update is called once per frame
    public void DeselectLoadout()
    {
        selectionVisual.SetActive(false);
    }
    public void SelectLoadout()
    {
        loadoutSelection.OnSelectLoadoutCallback(loadoutIndex);
        selectionVisual.SetActive(true);
    }
    public void SetLoadoutName(string content)
    {
        loadoutSelectionItemName.text = content;
    }
}
