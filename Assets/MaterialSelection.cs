using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSelection : MonoBehaviour
{
    public List<MaterialSelectionItem> materialSelectionList = new List<MaterialSelectionItem>();
    public enum MaterialParts{
        Head,
        Body,
        Feet
    }

    public Color bodyColor;
    public Color headColor;
    public Color feetColor;
    public void ClearPartsSelections(MaterialParts check)
    {
        if(check == MaterialParts.Head)
        {
            headColor = Color.white;
        }else if (check == MaterialParts.Body)
        {
            bodyColor = Color.white;
        }
        else
        {
            feetColor = Color.white;
        }
        for (int i = 0; i < materialSelectionList.Count; i++)
        {
            if (materialSelectionList[i].parts == check)
            {
                materialSelectionList[i].OnDeselect();
            }
        }
    }
    public void SelectAndApplyMaterial(Color color, MaterialParts part)
    {
        for(int i = 0; i < materialSelectionList.Count; i++)
        {
            if(color == materialSelectionList[i].image.color)
            {
                materialSelectionList[i].OnSelectColor();
            }
        }
        if (part == MaterialParts.Head)
        {
            headColor = color;
        }
        else if (part == MaterialParts.Body)
        {
            bodyColor = color;
        }
        else
        {
            feetColor = color;
        }
    }
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
