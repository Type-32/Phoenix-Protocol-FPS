using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelectionItem : MonoBehaviour
{
    public MaterialSelection materialSelectionMain;
    public Image image;
    public GameObject frame;
    public MaterialSelection.MaterialParts parts;
    private void Awake()
    {
        materialSelectionMain = FindObjectOfType<MaterialSelection>();
    }
    public void OnSelectColor()
    {
        materialSelectionMain.ClearPartsSelections(parts);
        switch (parts)
        {
            case MaterialSelection.MaterialParts.Head:
                materialSelectionMain.headColor = image.color;
                break;
            case MaterialSelection.MaterialParts.Body:
                materialSelectionMain.bodyColor = image.color;
                break;
            case MaterialSelection.MaterialParts.Feet:
                materialSelectionMain.feetColor = image.color;
                break;
        }
        frame.SetActive(true);
    }
    public void OnDeselect()
    {
        frame.SetActive(false);
    }
}
